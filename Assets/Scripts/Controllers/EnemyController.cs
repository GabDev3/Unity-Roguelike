using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Controllers
{
    public class EnemyController : BaseController, IDamageable
    {
        [Header("Loot Settings")]
        [Tooltip("Drag your money/coin prefab here")]
        [SerializeField] private GameObject coinPrefab;

        private bool _isDead = false;

        public void TakeDamage(int damage)
        {
            if (_isDead) return;

            if (health)
            {
                health.DecreaseHealth(damage);
                
                if (health.CurrentHealth <= 0)
                {
                    Die();
                }
            }
        }

        public bool WillDie(int damage)
        {
            return health.CurrentHealth - damage <= 0;
        }

        private void Die()
        {
            _isDead = true;
            SpawnLoot();
            // Health.cs will handle destroying the gameObject in FixedUpdate
        }

        private void SpawnLoot()
        {
            if (coinPrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
                GameObject spawnedItem = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

                // Start the pop out animation on the spawned item
                // We use a separate CoroutineRunner or attach a script to the coin because this gameObject will be destroyed
                ItemPopOut effect = spawnedItem.AddComponent<ItemPopOut>();
                effect.StartEffect();
            }
        }
    }

    public class ItemPopOut : MonoBehaviour
    {
        public void StartEffect()
        {
            StartCoroutine(PopOutAnimation());
        }

        private IEnumerator PopOutAnimation()
        {
            float duration = 0.5f;
            float elapsed = 0f;

            Vector3 startPos = transform.position;
            Vector3 targetOffset = new Vector3(Random.Range(-1f, 1f), -0.5f, 0);
            Vector3 endPos = startPos + targetOffset;

            while (elapsed < duration)
            {
                if (this == null || gameObject == null) yield break;

                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float height = Mathf.Sin(t * Mathf.PI) * 1.5f;

                Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
                currentPos.y += height;

                transform.position = currentPos;

                yield return null;
            }
        }
    }
}