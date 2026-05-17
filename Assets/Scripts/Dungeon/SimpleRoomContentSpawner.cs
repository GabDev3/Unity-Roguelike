using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon
{
    /// <summary>
    /// Simple runtime room content spawner that works independently of Edgar.
    /// Attach this to your DungeonGenerator or a separate GameObject in the scene.
    /// This spawns content after Edgar generates the dungeon.
    /// </summary>
    public class SimpleRoomContentSpawner : MonoBehaviour
    {
        [Header("Content Configuration")]
        [Tooltip("Default content to spawn in rooms")]
        public RoomContentData defaultRoomContent;
        
        [Tooltip("Content for the first/spawn room (usually fewer or no enemies)")]
        public RoomContentData startRoomContent;
        
        [Header("Prefab References")]
        [Tooltip("Player prefab to spawn")]
        public GameObject playerPrefab;
        
        [Header("Settings")]
        [Tooltip("Tag to identify room GameObjects after generation")]
        public string roomTag = "Room";
        
        [Tooltip("Layer name for floor tiles (used for spawn position validation)")]
        public string floorLayerName = "Floor";
        
        [Tooltip("Delay before spawning content (to ensure dungeon is fully generated)")]
        public float spawnDelay = 0.5f;
        
        [Header("Spawn Ranges")]
        public Vector2Int enemiesPerRoom = new Vector2Int(1, 3);
        public Vector2Int itemsPerRoom = new Vector2Int(0, 2);
        public Vector2Int interactablesPerRoom = new Vector2Int(0, 1);
        
        [Header("Enemy Prefabs")]
        public List<GameObject> enemyPrefabs = new List<GameObject>();
        
        [Header("Item Prefabs")]
        public List<GameObject> itemPrefabs = new List<GameObject>();
        
        [Header("Interactable Prefabs")]
        public List<GameObject> interactablePrefabs = new List<GameObject>();
        
        private System.Random random;
        private bool hasSpawnedPlayer = false;
        
        private void Start()
        {
            // Initialize random with seed from GameManager or system time
            int seed = GameManager.Instance != null ? GameManager.Instance.CurrentSeed : System.Environment.TickCount;
            random = new System.Random(seed);
            
            // Subscribe to dungeon generation complete event
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDungeonGenerationCompleted += OnDungeonGenerated;
            }
            
            // Also try to spawn after delay (fallback)
            Invoke(nameof(SpawnAllContent), spawnDelay);
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDungeonGenerationCompleted -= OnDungeonGenerated;
            }
        }
        
        private void OnDungeonGenerated()
        {
            CancelInvoke(nameof(SpawnAllContent));
            SpawnAllContent();
        }
        
        /// <summary>
        /// Main method to spawn content in all rooms
        /// </summary>
        public void SpawnAllContent()
        {
            Debug.Log("SimpleRoomContentSpawner: Starting content spawn...");
            
            // Find all room instances in the scene
            // Edgar creates room instances as children of the generated level
            var levelRoot = FindGeneratedLevel();
            
            if (levelRoot == null)
            {
                Debug.LogWarning("SimpleRoomContentSpawner: Could not find generated level!");
                return;
            }
            
            // Set tilemap cell size to 0.25
            var tilemaps = levelRoot.GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>();
            foreach (var tilemap in tilemaps)
            {
                var grid = tilemap.layoutGrid;
                if (grid != null)
                {
                    grid.cellSize = new Vector3(0.25f, 0.25f, grid.cellSize.z);
                }
            }
            
            // Get all room template instances
            var rooms = GetRoomInstances(levelRoot);
            
            Debug.Log($"SimpleRoomContentSpawner: Found {rooms.Count} rooms");
            
            bool isFirstRoom = true;
            
            foreach (var room in rooms)
            {
                if (IsCorridorRoom(room))
                {
                    Debug.Log($"Skipping corridor: {room.name}");
                    continue;
                }
                
                SpawnContentInRoom(room, isFirstRoom);
                isFirstRoom = false;
            }
            
            Debug.Log("SimpleRoomContentSpawner: Content spawn complete!");
        }
        
        /// <summary>
        /// Find the root GameObject of the generated level
        /// </summary>
        private GameObject FindGeneratedLevel()
        {
            // Edgar typically creates a "Generated Level" object or similar
            var levelRoot = GameObject.Find("Generated Level");
            if (levelRoot != null) return levelRoot;
            
            // Try to find by component
            var dungeonGenerator = FindFirstObjectByType<Edgar.Unity.DungeonGeneratorGrid2D>();
            if (dungeonGenerator != null)
            {
                // Check for generated rooms as children
                foreach (Transform child in dungeonGenerator.transform)
                {
                    if (child.name.Contains("Level") || child.name.Contains("Dungeon"))
                    {
                        return child.gameObject;
                    }
                }
                
                // Return the generator itself as fallback
                return dungeonGenerator.gameObject;
            }
            
            return null;
        }
        
        /// <summary>
        /// Get all room instances from the level
        /// </summary>
        private List<GameObject> GetRoomInstances(GameObject levelRoot)
        {
            var rooms = new List<GameObject>();
            
            // Get all room template instances (they typically have tilemaps)
            var tilemaps = levelRoot.GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>(true);
            
            foreach (var tilemap in tilemaps)
            {
                // Get the parent that represents the room
                Transform roomTransform = tilemap.transform.parent;
                
                if (roomTransform != null && !rooms.Contains(roomTransform.gameObject))
                {
                    // Check if this is a room (not just a tilemap layer)
                    if (roomTransform.GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>().Length > 0)
                    {
                        rooms.Add(roomTransform.gameObject);
                    }
                }
            }
            
            // Remove duplicates and sort
            return rooms.Distinct().ToList();
        }
        
        /// <summary>
        /// Check if a room is a corridor
        /// </summary>
        private bool IsCorridorRoom(GameObject room)
        {
            string lowerName = room.name.ToLower();
            return lowerName.Contains("corridor") || lowerName.Contains("hallway");
        }
        
        /// <summary>
        /// Spawn content in a single room
        /// </summary>
        private void SpawnContentInRoom(GameObject room, bool isStartRoom)
        {
            Debug.Log($"Spawning content in room: {room.name} (Start room: {isStartRoom})");
            
            // Calculate room bounds
            Bounds roomBounds = CalculateRoomBounds(room);
            
            if (roomBounds.size == Vector3.zero)
            {
                Debug.LogWarning($"Could not calculate bounds for room: {room.name}");
                return;
            }
            
            // Spawn player in start room
            if (isStartRoom && !hasSpawnedPlayer)
            {
                SpawnPlayer(roomBounds.center);
                hasSpawnedPlayer = true;
                
                SpawnAllItemsExplicitly(room, roomBounds);
                
                // Spawn fewer enemies in start room
                if (startRoomContent != null)
                {
                    SpawnFromContentData(room, roomBounds, startRoomContent);
                    return;
                }
            }
            
            // Spawn using content data if available
            if (defaultRoomContent != null)
            {
                SpawnFromContentData(room, roomBounds, defaultRoomContent);
                return;
            }
            
            // Fallback: spawn using direct prefab lists
            SpawnEnemies(room, roomBounds, isStartRoom);
            SpawnItems(room, roomBounds);
            SpawnInteractables(room, roomBounds);
        }
        
        private void SpawnFromContentData(GameObject room, Bounds roomBounds, RoomContentData contentData)
        {
            // Check spawn chance
            if (random.NextDouble() > contentData.spawnChance)
                return;
            
            // Spawn enemies
            int enemyCount = random.Next(contentData.enemyCountRange.x, contentData.enemyCountRange.y + 1);
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnRandomFromList(room, roomBounds, contentData.enemies);
            }
            
            // Spawn items
            int itemCount = random.Next(contentData.itemCountRange.x, contentData.itemCountRange.y + 1);
            for (int i = 0; i < itemCount; i++)
            {
                SpawnRandomFromList(room, roomBounds, contentData.items);
            }
            
            // Spawn interactables
            int interactableCount = random.Next(contentData.interactableCountRange.x, contentData.interactableCountRange.y + 1);
            for (int i = 0; i < interactableCount; i++)
            {
                SpawnRandomFromList(room, roomBounds, contentData.interactables);
            }
            
            // Spawn obstacles
            int obstacleCount = random.Next(contentData.obstacleCountRange.x, contentData.obstacleCountRange.y + 1);
            for (int i = 0; i < obstacleCount; i++)
            {
                SpawnRandomFromList(room, roomBounds, contentData.obstacles);
            }
        }
        
        private void SpawnRandomFromList(GameObject room, Bounds roomBounds, List<SpawnableObject> objects)
        {
            if (objects == null || objects.Count == 0)
                return;
            
            // Select weighted random
            int totalWeight = objects.Sum(o => o.weight);
            int randomValue = random.Next(0, totalWeight);
            
            int currentWeight = 0;
            SpawnableObject selected = null;
            
            foreach (var obj in objects)
            {
                currentWeight += obj.weight;
                if (randomValue < currentWeight)
                {
                    selected = obj;
                    break;
                }
            }
            
            if (selected?.prefab != null)
            {
                Vector3 spawnPos = GetRandomPositionInBounds(roomBounds);
                Instantiate(selected.prefab, spawnPos, Quaternion.identity, room.transform);
            }
        }
        
        private void SpawnEnemies(GameObject room, Bounds roomBounds, bool isStartRoom)
        {
            if (enemyPrefabs.Count == 0)
                return;
            
            int count = isStartRoom ? 0 : random.Next(enemiesPerRoom.x, enemiesPerRoom.y + 1);
            
            for (int i = 0; i < count; i++)
            {
                int prefabIndex = random.Next(0, enemyPrefabs.Count);
                Vector3 spawnPos = GetRandomPositionInBounds(roomBounds);
                
                Instantiate(enemyPrefabs[prefabIndex], spawnPos, Quaternion.identity, room.transform);
            }
        }
        
        private void SpawnItems(GameObject room, Bounds roomBounds)
        {
            if (itemPrefabs.Count == 0)
                return;
            
            int count = random.Next(itemsPerRoom.x, itemsPerRoom.y + 1);
            
            for (int i = 0; i < count; i++)
            {
                int prefabIndex = random.Next(0, itemPrefabs.Count);
                Vector3 spawnPos = GetRandomPositionInBounds(roomBounds);
                
                Instantiate(itemPrefabs[prefabIndex], spawnPos, Quaternion.identity, room.transform);
            }
        }
        
        private void SpawnInteractables(GameObject room, Bounds roomBounds)
        {
            if (interactablePrefabs.Count == 0)
                return;
            
            int count = random.Next(interactablesPerRoom.x, interactablesPerRoom.y + 1);
            
            for (int i = 0; i < count; i++)
            {
                int prefabIndex = random.Next(0, interactablePrefabs.Count);
                Vector3 spawnPos = GetRandomPositionInBounds(roomBounds);
                
                Instantiate(interactablePrefabs[prefabIndex], spawnPos, Quaternion.identity, room.transform);
            }
        }
        
        private void SpawnAllItemsExplicitly(GameObject room, Bounds roomBounds)
        {
            if (itemPrefabs == null || itemPrefabs.Count == 0)
                return;
            
            foreach (var prefab in itemPrefabs)
            {
                if (prefab != null)
                {
                    Vector3 spawnPos = GetRandomPositionInBounds(roomBounds);
                    Instantiate(prefab, spawnPos, Quaternion.identity, room.transform);
                }
            }
        }
        
        private void SpawnPlayer(Vector3 position)
        {
            // Try to find existing player first
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
            
            if (existingPlayer != null)
            {
                existingPlayer.transform.position = position;
                Debug.Log($"Moved existing player to {position}");
                return;
            }
            
            // Spawn new player
            if (playerPrefab != null)
            {
                Instantiate(playerPrefab, position, Quaternion.identity);
                Debug.Log($"Spawned player at {position}");
            }
            else
            {
                Debug.LogWarning("SimpleRoomContentSpawner: No player prefab assigned!");
            }
        }
        
        private Bounds CalculateRoomBounds(GameObject room)
        {
            var tilemaps = room.GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>();
            
            if (tilemaps.Length == 0)
                return new Bounds();
            
            Bounds bounds = new Bounds();
            bool boundsInitialized = false;
            
            foreach (var tilemap in tilemaps)
            {
                tilemap.CompressBounds();
                
                if (tilemap.cellBounds.size == Vector3Int.zero)
                    continue;
                
                // Convert to proper world bounds
                Vector3 min = tilemap.CellToWorld(tilemap.cellBounds.min);
                Vector3 max = tilemap.CellToWorld(tilemap.cellBounds.max);
                Bounds worldBounds = new Bounds((min + max) / 2f, max - min);
                
                if (!boundsInitialized)
                {
                    bounds = worldBounds;
                    boundsInitialized = true;
                }
                else
                {
                    bounds.Encapsulate(worldBounds);
                }
            }
            
            return bounds;
        }
        
        private Vector3 GetRandomPositionInBounds(Bounds bounds)
        {
            float padding = 1.5f; // Keep away from walls
            
            float x = (float)(random.NextDouble() * (bounds.size.x - padding * 2) + bounds.min.x + padding);
            float y = (float)(random.NextDouble() * (bounds.size.y - padding * 2) + bounds.min.y + padding);
            
            return new Vector3(x, y, 0);
        }
    }
}

