using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon
{
    /// <summary>
    /// Interactive door that can be opened/closed with the E key.
    /// Doors block passage until opened.
    /// </summary>
    public class Door : MonoBehaviour
    {
        [Header("Door State")]
        [SerializeField] private bool isOpen = false;
        
        [Header("Visual Settings")]
        [Tooltip("Sprite to show when door is closed")]
        public Sprite closedSprite;
        
        [Tooltip("Sprite to show when door is open")]
        public Sprite openSprite;
        
        [Header("Interaction")]
        [Tooltip("Range within which player can interact with the door")]
        public float interactionRange = 1.5f;
        
        [Tooltip("Key to open/close the door")]
        public KeyCode interactKey = KeyCode.E;
        
        [Header("References")]
        private SpriteRenderer spriteRenderer;
        private Collider2D doorCollider;
        private bool playerInRange = false;
        private Transform playerTransform;
        
        // UI prompt reference
        private Controllers.PlayerController playerController;
        
        // Door position info
        public Vector3Int DoorPosition { get; set; }
        public bool IsHorizontal { get; set; }
        public int DoorLength { get; set; } = 1;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            doorCollider = GetComponent<Collider2D>();
            
            // Create collider if not present
            if (doorCollider == null)
            {
                var boxCollider = gameObject.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = false;
                doorCollider = boxCollider;
            }
            
            // Set initial state
            UpdateDoorState();
        }
        
        private void Update()
        {
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                bool wasInRange = playerInRange;
                playerInRange = distance <= interactionRange;
                
                // Show/hide prompt when entering/leaving range
                if (playerInRange != wasInRange && playerController != null)
                {
                    string promptText = isOpen ? "Press E to Close" : "Press E to Open";
                    playerController.TogglePickItemPrompt(playerInRange, promptText);
                }
                
                // Handle interaction
                if (playerInRange && Input.GetKeyDown(interactKey))
                {
                    ToggleDoor();
                }
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerTransform = other.transform;
                playerController = other.GetComponent<Controllers.PlayerController>();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (playerController != null)
                {
                    playerController.TogglePickItemPrompt(false);
                }
                playerTransform = null;
                playerController = null;
                playerInRange = false;
            }
        }
        
        /// <summary>
        /// Toggle the door open/closed
        /// </summary>
        public void ToggleDoor()
        {
            isOpen = !isOpen;
            UpdateDoorState();
            
            // Update prompt text
            if (playerInRange && playerController != null)
            {
                string promptText = isOpen ? "Press E to Close" : "Press E to Open";
                playerController.TogglePickItemPrompt(true, promptText);
            }
            
            Debug.Log($"Door {(isOpen ? "opened" : "closed")}");
        }
        
        /// <summary>
        /// Open the door
        /// </summary>
        public void Open()
        {
            if (!isOpen)
            {
                isOpen = true;
                UpdateDoorState();
            }
        }
        
        /// <summary>
        /// Close the door
        /// </summary>
        public void Close()
        {
            if (isOpen)
            {
                isOpen = false;
                UpdateDoorState();
            }
        }
        
        private void UpdateDoorState()
        {
            // Update visual
            if (spriteRenderer != null)
            {
                if (isOpen && openSprite != null)
                {
                    spriteRenderer.sprite = openSprite;
                }
                else if (!isOpen && closedSprite != null)
                {
                    spriteRenderer.sprite = closedSprite;
                }
                
                // If no sprites, just hide/show the renderer
                if (closedSprite == null && openSprite == null)
                {
                    spriteRenderer.enabled = !isOpen;
                }
            }
            
            // Update collision
            if (doorCollider != null)
            {
                // When open, the door should not block movement
                doorCollider.enabled = !isOpen;
            }
        }
        
        /// <summary>
        /// Check if door is currently open
        /// </summary>
        public bool IsOpen => isOpen;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isOpen ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}

