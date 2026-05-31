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

        [Tooltip("Extra content variants; a random one (or defaultRoomContent) is picked per normal room for variety")]
        public List<RoomContentData> roomContentVariants = new List<RoomContentData>();

        [Tooltip("Content used for the boss room (room name contains 'Boss')")]
        public RoomContentData bossRoomContent;

        [Header("Prefab References")]
        [Tooltip("Player prefab to spawn (fallback / melee)")]
        public GameObject playerPrefab;

        [Tooltip("Melee player prefab (used when GameManager.SelectedClass == Melee)")]
        public GameObject playerPrefabMelee;

        [Tooltip("Ranged player prefab (used when GameManager.SelectedClass == Ranged)")]
        public GameObject playerPrefabRanged;

        [Header("Chest")]
        [Tooltip("Chest prefab spawned with chestSpawnChance in normal rooms")]
        public GameObject chestPrefab;

        [Tooltip("Probability (0-1) that a normal room contains a chest")]
        [Range(0f, 1f)]
        public float chestSpawnChance = 0.2f;
        
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
                // Go up the hierarchy to find the room root
                Transform roomTransform = tilemap.transform.parent;
                
                // Usually Edgar puts Tilemaps under a "Tilemaps" object, so the room is its parent
                if (roomTransform != null && (roomTransform.name.Contains("Tilemap") || roomTransform.name.Contains("Grid")))
                {
                    roomTransform = roomTransform.parent;
                }
                
                // Another safeguard: if the parent is "Rooms", then roomTransform is the room itself
                while (roomTransform != null && roomTransform.parent != null && roomTransform.parent.name != "Rooms" && roomTransform.parent != levelRoot.transform && !roomTransform.name.Contains("Room"))
                {
                    if (roomTransform.parent.name == "Generated Level" || roomTransform.parent.name == "Rooms")
                        break;
                    roomTransform = roomTransform.parent;
                }
                
                if (roomTransform != null && !rooms.Contains(roomTransform.gameObject))
                {
                    rooms.Add(roomTransform.gameObject);
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

            // Collect all available spawn points in the room (including inactive ones just in case)
            var spawnPoints = room.GetComponentsInChildren<SpawnPoint>(true).ToList();

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
                SpawnPlayer(room, roomBounds, spawnPoints);
                hasSpawnedPlayer = true;

                SpawnAllItemsExplicitly(room, roomBounds, spawnPoints);

                // Spawn fewer enemies in start room
                if (startRoomContent != null)
                {
                    SpawnFromContentData(room, roomBounds, startRoomContent, spawnPoints);
                    return;
                }
            }

            // Boss room: use dedicated boss content
            if (IsBossRoom(room) && bossRoomContent != null)
            {
                SpawnFromContentData(room, roomBounds, bossRoomContent, spawnPoints);
                return;
            }

            // Normal room: pick a random content variant for variety
            RoomContentData content = PickRoomContent();
            if (content != null)
            {
                SpawnFromContentData(room, roomBounds, content, spawnPoints);
                TrySpawnChest(room, roomBounds, spawnPoints);
                return;
            }

            // Fallback: spawn using direct prefab lists
            SpawnEnemies(room, roomBounds, isStartRoom, spawnPoints);
            SpawnItems(room, roomBounds, spawnPoints);
            SpawnInteractables(room, roomBounds, spawnPoints);
            TrySpawnChest(room, roomBounds, spawnPoints);
        }

        private bool IsBossRoom(GameObject room)
        {
            return room.name.ToLower().Contains("boss");
        }

        private RoomContentData PickRoomContent()
        {
            var pool = new List<RoomContentData>();
            if (defaultRoomContent != null) pool.Add(defaultRoomContent);
            if (roomContentVariants != null)
                pool.AddRange(roomContentVariants.Where(c => c != null));

            if (pool.Count == 0) return null;
            return pool[random.Next(pool.Count)];
        }

        private void TrySpawnChest(GameObject room, Bounds roomBounds, List<SpawnPoint> spawnPoints)
        {
            if (chestPrefab == null) return;
            if (random.NextDouble() > chestSpawnChance) return;

            Vector3 pos = GetSpawnPosition(roomBounds, SpawnPointType.Interactable, spawnPoints);
            Instantiate(chestPrefab, pos, Quaternion.identity, room.transform);
        }

        private void SpawnFromContentData(GameObject room, Bounds roomBounds, RoomContentData contentData, List<SpawnPoint> spawnPoints)
        {
            // Check spawn chance
            if (random.NextDouble() > contentData.spawnChance)
                return;

            // Spawn enemies
            int enemyCount = random.Next(contentData.enemyCountRange.x, contentData.enemyCountRange.y + 1);
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnRandomFromList(room, roomBounds, contentData.enemies, SpawnPointType.Enemy, spawnPoints);
            }

            // Spawn items
            int itemCount = random.Next(contentData.itemCountRange.x, contentData.itemCountRange.y + 1);
            for (int i = 0; i < itemCount; i++)
            {
                SpawnRandomFromList(room, roomBounds, contentData.items, SpawnPointType.Item, spawnPoints);
            }

            // Spawn interactables
            int interactableCount = random.Next(contentData.interactableCountRange.x, contentData.interactableCountRange.y + 1);
            for (int i = 0; i < interactableCount; i++)
            {
                SpawnRandomFromList(room, roomBounds, contentData.interactables, SpawnPointType.Interactable, spawnPoints);
            }

            // Spawn obstacles
            int obstacleCount = random.Next(contentData.obstacleCountRange.x, contentData.obstacleCountRange.y + 1);
            for (int i = 0; i < obstacleCount; i++)
            {
                SpawnRandomFromList(room, roomBounds, contentData.obstacles, SpawnPointType.Obstacle, spawnPoints);
            }
        }

        private void SpawnRandomFromList(GameObject room, Bounds roomBounds, List<SpawnableObject> objects, SpawnPointType type, List<SpawnPoint> spawnPoints)
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
                Vector3 spawnPos = GetSpawnPosition(roomBounds, type, spawnPoints);
                Instantiate(selected.prefab, spawnPos, Quaternion.identity, room.transform);
            }
        }

        private void SpawnEnemies(GameObject room, Bounds roomBounds, bool isStartRoom, List<SpawnPoint> spawnPoints)
        {
            if (enemyPrefabs.Count == 0)
                return;

            int count = isStartRoom ? 0 : random.Next(enemiesPerRoom.x, enemiesPerRoom.y + 1);

            for (int i = 0; i < count; i++)
            {
                int prefabIndex = random.Next(0, enemyPrefabs.Count);
                Vector3 spawnPos = GetSpawnPosition(roomBounds, SpawnPointType.Enemy, spawnPoints);

                Instantiate(enemyPrefabs[prefabIndex], spawnPos, Quaternion.identity, room.transform);
            }
        }

        private void SpawnItems(GameObject room, Bounds roomBounds, List<SpawnPoint> spawnPoints)
        {
            if (itemPrefabs.Count == 0)
                return;

            int count = random.Next(itemsPerRoom.x, itemsPerRoom.y + 1);

            for (int i = 0; i < count; i++)
            {
                int prefabIndex = random.Next(0, itemPrefabs.Count);
                Vector3 spawnPos = GetSpawnPosition(roomBounds, SpawnPointType.Item, spawnPoints);

                Instantiate(itemPrefabs[prefabIndex], spawnPos, Quaternion.identity, room.transform);
            }
        }

        private void SpawnInteractables(GameObject room, Bounds roomBounds, List<SpawnPoint> spawnPoints)
        {
            if (interactablePrefabs.Count == 0)
                return;

            int count = random.Next(interactablesPerRoom.x, interactablesPerRoom.y + 1);

            for (int i = 0; i < count; i++)
            {
                int prefabIndex = random.Next(0, interactablePrefabs.Count);
                Vector3 spawnPos = GetSpawnPosition(roomBounds, SpawnPointType.Interactable, spawnPoints);

                Instantiate(interactablePrefabs[prefabIndex], spawnPos, Quaternion.identity, room.transform);
            }
        }

        private void SpawnAllItemsExplicitly(GameObject room, Bounds roomBounds, List<SpawnPoint> spawnPoints)
        {
            if (itemPrefabs == null || itemPrefabs.Count == 0)
                return;

            foreach (var prefab in itemPrefabs)
            {
                if (prefab != null)
                {
                    Vector3 spawnPos = GetSpawnPosition(roomBounds, SpawnPointType.Item, spawnPoints);
                    Instantiate(prefab, spawnPos, Quaternion.identity, room.transform);
                }
            }
        }

        private void SpawnPlayer(GameObject room, Bounds roomBounds, List<SpawnPoint> spawnPoints)
        {
            Vector3 position = GetSpawnPosition(roomBounds, SpawnPointType.PlayerSpawn, spawnPoints);
            // Try to find existing player first
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

            if (existingPlayer != null)
            {
                existingPlayer.transform.position = position;
                Debug.Log($"Moved existing player to {position}");
                return;
            }

            // Pick the prefab matching the chosen class (falls back to playerPrefab)
            GameObject prefab = playerPrefab;
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.SelectedClass == PlayerClass.Ranged && playerPrefabRanged != null)
                    prefab = playerPrefabRanged;
                else if (GameManager.Instance.SelectedClass == PlayerClass.Melee && playerPrefabMelee != null)
                    prefab = playerPrefabMelee;
            }

            // Spawn new player
            if (prefab != null)
            {
                Instantiate(prefab, position, Quaternion.identity);
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
        
        private Vector3 GetSpawnPosition(Bounds bounds, SpawnPointType type, List<SpawnPoint> spawnPoints)
        {
            var validPoints = spawnPoints.Where(p => (!p.singleUse || !p.isUsed) && (p.spawnType == type || p.spawnType == SpawnPointType.Any)).ToList();
            if (validPoints.Any())
            {
                validPoints.Sort((a, b) => b.priority.CompareTo(a.priority));
                var topPriority = validPoints[0].priority;
                var candidates = validPoints.Where(p => p.priority == topPriority).ToList();
                var selected = candidates[random.Next(candidates.Count)];
                selected.isUsed = true;
                return selected.GetRandomSpawnPosition();
            }

            return GetRandomPositionInBounds(bounds);
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

