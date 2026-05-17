using UnityEngine;
using Interfaces;

namespace Weapons
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class SpellProjectile : MonoBehaviour
    {
        private int _damage;
        private Rigidbody2D _rb;
        private LayerMask _targetLayer;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 direction, float speed, int damage, LayerMask targetLayer)
        {
            _damage = damage;
            _targetLayer = targetLayer;
            _rb.linearVelocity = direction.normalized * speed;

            // Rotate the projectile to face the direction of movement.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Debug.Log("intended:" + _targetLayer.value.ToString() + "actual:" + other.gameObject.layer.ToString());
            if((_targetLayer.value & (1 << other.gameObject.layer)) > 0)
            {
                other.GetComponent<IDamageable>()?.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}