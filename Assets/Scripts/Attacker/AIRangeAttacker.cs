using Interfaces;
using UnityEngine;
using Weapons;
using System.Linq;

namespace Attacker
{
    public class AIRangeAttacker : BaseRangeAttacker, IAttacker
    {
        [Tooltip("How long a target must be in range before attacking.")]
        public float timeToAttack = 1f;

        private float _timeInZone = 0f;

        public void Update()
        {
            // Debug.Log(targetsInRange.Count());
            if (targetsInRange.Count > 0)
            {
                _timeInZone += Time.deltaTime;
                if (_timeInZone >= timeToAttack && !IsOnCooldown())
                {
                    Attack();
                    // Debug.Log("spent sufficient time in zone");
                }
            }
            else
            {
                _timeInZone = 0f;
            }
        }

        public int CalculateDamage()
        {
            return Damage;
        }

        public void Attack()
        {
            Collider2D target = targetsInRange.FirstOrDefault(t => t != null);

            if (target)
            {
                Debug.Log("Attacking " + target.name + " at:" + target.transform.position);
                Shoot(target.transform.position - gameObject.transform.position, targetLayers);
            }
        }
    }
}