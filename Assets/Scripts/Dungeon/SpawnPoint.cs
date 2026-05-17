using UnityEngine;

namespace Dungeon
{
    /// <summary>
    /// Marks a position in a room template where objects can be spawned.
    /// Add this component to empty GameObjects placed in your room templates.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [Header("Spawn Point Configuration")]
        [Tooltip("What type of object can spawn at this point")]
        public SpawnPointType spawnType = SpawnPointType.Any;
        
        [Tooltip("Radius within which objects can randomly spawn around this point")]
        [Range(0f, 2f)]
        public float spawnRadius = 0.5f;
        
        [Tooltip("If true, only one object can be spawned at this point per generation")]
        public bool singleUse = true;
        
        [Tooltip("Priority - higher priority spawn points are used first")]
        public int priority = 0;
        
        /// <summary>
        /// Whether this spawn point has been used in current generation
        /// </summary>
        [HideInInspector]
        public bool isUsed = false;
        
        /// <summary>
        /// Get a random position within the spawn radius
        /// </summary>
        public Vector3 GetRandomSpawnPosition()
        {
            if (spawnRadius <= 0)
                return transform.position;
                
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            return transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        }
        
        /// <summary>
        /// Reset the spawn point for new generation
        /// </summary>
        public void ResetSpawnPoint()
        {
            isUsed = false;
        }
        
        private void OnDrawGizmos()
        {
            // Draw different colored gizmos based on spawn type
            switch (spawnType)
            {
                case SpawnPointType.Enemy:
                    Gizmos.color = Color.red;
                    break;
                case SpawnPointType.Item:
                    Gizmos.color = Color.yellow;
                    break;
                case SpawnPointType.Interactable:
                    Gizmos.color = Color.cyan;
                    break;
                case SpawnPointType.Obstacle:
                    Gizmos.color = Color.gray;
                    break;
                case SpawnPointType.PlayerSpawn:
                    Gizmos.color = Color.green;
                    break;
                default:
                    Gizmos.color = Color.white;
                    break;
            }
            
            Gizmos.DrawWireSphere(transform.position, spawnRadius > 0 ? spawnRadius : 0.3f);
            Gizmos.DrawIcon(transform.position, "d_Prefab Icon", true);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, spawnRadius > 0 ? spawnRadius : 0.3f);
        }
    }
    
    /// <summary>
    /// Types of objects that can be spawned at a spawn point
    /// </summary>
    public enum SpawnPointType
    {
        Any,            // Can spawn anything
        Enemy,          // Only enemies
        Item,           // Only items (weapons, armor, money)
        Interactable,   // Chests, switches, etc.
        Obstacle,       // Decorations, barriers
        PlayerSpawn     // Player starting position
    }
}

