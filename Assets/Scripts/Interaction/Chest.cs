using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Interaction
{
    public class Chest : InteractableItem
    {
        [Header("Loot Settings")] [Tooltip("Drag your Weapon, Armor, and Money prefabs here")] [SerializeField]
        private List<GameObject> lootTable;

        public Animator animator;

        // [Header("Visuals")] [SerializeField] private Sprite openChestSprite;
        // [SerializeField] private SpriteRenderer myRenderer;

        private bool _isOpen;

        private void Awake()
        {
            _isOpen = false;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Controllers.PlayerController player) && !_isOpen)
            {
                _isPlayerInRange = true;
                _targetPlayer = player;

                _targetPlayer.TogglePickItemPrompt(true, "Press 'E' to open the chest");
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isOpen)
            {
                if (_targetPlayer)
                {
                    _targetPlayer.TogglePickItemPrompt(false);

                    _isPlayerInRange = false;
                    _targetPlayer = null;
                }
            }
        }

        protected override void OnInteraction()
        {
            if (_isOpen) return;
            OpenChest();
        }

        private void OpenChest()
        {
            _isOpen = true;

            if (animator)
            {
                animator.SetTrigger("Open");
            }

            if (_targetPlayer)
            {
                _targetPlayer.TogglePickItemPrompt(false);
            }

            StartCoroutine(SpawnLootDelayed(1.3f));
        }

        private IEnumerator SpawnLootDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (lootTable != null && lootTable.Count > 0)
            {
                int randomIndex = Random.Range(0, lootTable.Count);
                GameObject selectedPrefab = lootTable[randomIndex];

                Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
                GameObject spawnedItem = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

                StartCoroutine(PopOutAnimation(spawnedItem.transform));
            }
        }

        private IEnumerator PopOutAnimation(Transform itemTrans)
        {
            float duration = 0.5f;
            float elapsed = 0f;

            Vector3 startPos = itemTrans.position;
            Vector3 targetOffset = new Vector3(Random.Range(-1f, 1f), -0.5f, 0);
            Vector3 endPos = startPos + targetOffset;

            while (elapsed < duration)
            {
                if (itemTrans == null) yield break;

                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float height = Mathf.Sin(t * Mathf.PI) * 1.5f;

                Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
                currentPos.y += height;

                itemTrans.position = currentPos;

                yield return null;
            }
        }
    }
}