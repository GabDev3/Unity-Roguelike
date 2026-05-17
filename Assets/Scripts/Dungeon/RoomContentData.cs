using UnityEngine;
using System.Collections.Generic;

namespace Dungeon
{
    /// <summary>
    /// Defines what content can spawn in a room.
    /// Create different RoomContentData assets for different room types (e.g., combat room, treasure room, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "New Room Content", menuName = "Dungeon/Room Content Data")]
    public class RoomContentData : ScriptableObject
    {
        [Header("Room Type")]
        [Tooltip("Name to identify this room type")]
        public string roomTypeName = "Generic Room";
        
        [Header("Enemy Settings")]
        [Tooltip("List of enemy prefabs that can spawn in this room")]
        public List<SpawnableObject> enemies = new List<SpawnableObject>();
        
        [Tooltip("Min and max number of enemies to spawn")]
        public Vector2Int enemyCountRange = new Vector2Int(1, 3);
        
        [Header("Item Settings")]
        [Tooltip("List of item prefabs that can spawn in this room (weapons, armor, money, etc.)")]
        public List<SpawnableObject> items = new List<SpawnableObject>();
        
        [Tooltip("Min and max number of items to spawn")]
        public Vector2Int itemCountRange = new Vector2Int(0, 2);
        
        [Header("Interactable Settings")]
        [Tooltip("List of interactable prefabs that can spawn (chests, etc.)")]
        public List<SpawnableObject> interactables = new List<SpawnableObject>();
        
        [Tooltip("Min and max number of interactables to spawn")]
        public Vector2Int interactableCountRange = new Vector2Int(0, 1);
        
        [Header("Obstacle Settings")]
        [Tooltip("List of obstacle prefabs that can spawn (decorations, barriers, etc.)")]
        public List<SpawnableObject> obstacles = new List<SpawnableObject>();
        
        [Tooltip("Min and max number of obstacles to spawn")]
        public Vector2Int obstacleCountRange = new Vector2Int(0, 3);
        
        [Header("Spawn Probability")]
        [Tooltip("Overall chance that this room will have any content spawned (0-1)")]
        [Range(0f, 1f)]
        public float spawnChance = 1f;
    }
    
    /// <summary>
    /// Represents a prefab that can be spawned with a specific weight/probability
    /// </summary>
    [System.Serializable]
    public class SpawnableObject
    {
        [Tooltip("The prefab to spawn")]
        public GameObject prefab;
        
        [Tooltip("Spawn weight - higher values mean more likely to be selected")]
        [Range(1, 100)]
        public int weight = 10;
        
        [Tooltip("Minimum instances of this specific object in a room")]
        public int minCount = 0;
        
        [Tooltip("Maximum instances of this specific object in a room")]
        public int maxCount = 3;
    }
}

