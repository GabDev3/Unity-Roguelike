using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using Edgar.Unity;

namespace Dungeon
{
    /// <summary>
    /// Door spawner that creates gaps in walls and places door GameObjects.
    /// </summary>
    public class SimpleDoorSpawner : DungeonGeneratorPostProcessingComponentGrid2D
    {
        [Header("Door Prefab")]
        public GameObject doorPrefab;
        
        [Header("Tilemap Settings")]
        public string wallsTilemapName = "Walls";
        public string collideableTilemapName = "Collideable";
        public bool removeFromCollideable = true;
        
        [Header("Door Gap Size")]
        [Tooltip("Extra tiles to remove perpendicular to the door line")]
        public int extraGapSize = 2;
        
        [Header("Debug")]
        public bool showDebugInfo = true;
        
        public override void Run(DungeonGeneratorLevelGrid2D level)
        {
            if (showDebugInfo) Debug.Log("=== SimpleDoorSpawner: Starting ===");
            
            if (doorPrefab == null)
            {
                Debug.LogError("SimpleDoorSpawner: No door prefab assigned!");
                return;
            }
            
            // Get shared tilemaps
            var sharedTilemaps = level.GetSharedTilemaps();
            if (sharedTilemaps == null || sharedTilemaps.Count == 0)
            {
                Debug.LogError("SimpleDoorSpawner: No shared tilemaps found!");
                return;
            }
            
            var wallsTilemap = sharedTilemaps.FirstOrDefault(x => x.name == wallsTilemapName);
            if (wallsTilemap == null)
            {
                Debug.LogError($"SimpleDoorSpawner: Could not find '{wallsTilemapName}' tilemap!");
                return;
            }
            
            Tilemap collideableTilemap = removeFromCollideable 
                ? sharedTilemaps.FirstOrDefault(x => x.name == collideableTilemapName) 
                : null;
            
            // Create doors container
            var doorsContainer = new GameObject("Doors");
            doorsContainer.transform.SetParent(level.RootGameObject.transform);
            doorsContainer.transform.localPosition = Vector3.zero;
            
            HashSet<string> processedDoors = new HashSet<string>();
            int doorCount = 0;
            
            foreach (var roomInstance in level.RoomInstances)
            {
                if (roomInstance.Doors == null || roomInstance.Doors.Count == 0)
                    continue;
                
                // Get the room's transform and its local tilemap
                Transform roomTransform = roomInstance.RoomTemplateInstance.transform;
                
                // Find the walls tilemap in the room template (for cell size reference)
                var roomTilemap = roomInstance.RoomTemplateInstance.GetComponentInChildren<Tilemap>();
                Vector3 cellSize = roomTilemap != null ? roomTilemap.cellSize : new Vector3(0.25f, 0.25f, 0);
                
                if (showDebugInfo)
                    Debug.Log($"Room: {roomInstance.Room?.GetDisplayName() ?? "Unknown"} at {roomTransform.position}, cellSize: {cellSize}");
                
                foreach (var doorInfo in roomInstance.Doors)
                {
                    var doorLine = doorInfo.DoorLine;
                    
                    // Get tile positions (local + room position = level position)
                    List<Vector3Int> tilesToRemove = new List<Vector3Int>();
                    
                    Vector3Int from = doorLine.From + roomInstance.Position;
                    Vector3Int to = doorLine.To + roomInstance.Position;
                    
                    // Walk along the door line
                    int dx = System.Math.Sign(to.x - from.x);
                    int dy = System.Math.Sign(to.y - from.y);
                    if (dx == 0 && dy == 0) { dx = 0; dy = 0; }
                    
                    Vector3Int current = from;
                    while (true)
                    {
                        tilesToRemove.Add(current);
                        if (current.x == to.x && current.y == to.y) break;
                        current = new Vector3Int(current.x + dx, current.y + dy, 0);
                        if (tilesToRemove.Count > 50) break;
                    }
                    
                    // Expand perpendicular to door line
                    int perpDx = doorInfo.IsHorizontal ? 0 : 1;
                    int perpDy = doorInfo.IsHorizontal ? 1 : 0;
                    
                    var expandedTiles = new List<Vector3Int>(tilesToRemove);
                    foreach (var tile in tilesToRemove)
                    {
                        for (int i = 1; i <= extraGapSize; i++)
                        {
                            expandedTiles.Add(new Vector3Int(tile.x + perpDx * i, tile.y + perpDy * i, 0));
                            expandedTiles.Add(new Vector3Int(tile.x - perpDx * i, tile.y - perpDy * i, 0));
                        }
                    }
                    
                    // Calculate center of door line in LOCAL room coordinates (not level)
                    float localCenterX = (doorLine.From.x + doorLine.To.x) / 2f;
                    float localCenterY = (doorLine.From.y + doorLine.To.y) / 2f;
                    
                    // Check for duplicates using level tile position
                    Vector3Int centerLevelTile = new Vector3Int(
                        Mathf.RoundToInt(localCenterX) + roomInstance.Position.x,
                        Mathf.RoundToInt(localCenterY) + roomInstance.Position.y,
                        0
                    );
                    string key = $"{centerLevelTile.x}_{centerLevelTile.y}";
                    if (processedDoors.Contains(key)) continue;
                    processedDoors.Add(key);
                    
                    // Remove tiles from shared tilemaps
                    foreach (var tile in expandedTiles)
                    {
                        wallsTilemap.SetTile(tile, null);
                        if (collideableTilemap != null)
                            collideableTilemap.SetTile(tile, null);
                    }
                    
                    // Calculate world position using room transform
                    // Local position = tile coords * cell size, centered in cell
                    Vector3 localPos = new Vector3(
                        (localCenterX + 0.5f) * cellSize.x,
                        (localCenterY + 0.5f) * cellSize.y,
                        0
                    );
                    Vector3 doorWorldPos = roomTransform.TransformPoint(localPos);
                    
                    if (showDebugInfo)
                        Debug.Log($"Door: localCenter ({localCenterX}, {localCenterY}) -> localPos {localPos} -> worldPos {doorWorldPos}");
                    
                    // Spawn door
                    var doorObj = Instantiate(doorPrefab, doorWorldPos, Quaternion.identity, doorsContainer.transform);
                    doorObj.name = $"Door_{centerLevelTile.x}_{centerLevelTile.y}";
                    
                    var door = doorObj.GetComponent<Door>();
                    if (door != null) door.IsHorizontal = doorInfo.IsHorizontal;
                    
                    doorCount++;
                }
            }
            
            if (showDebugInfo) Debug.Log($"=== SimpleDoorSpawner: Created {doorCount} doors ===");
        }
    }
}

