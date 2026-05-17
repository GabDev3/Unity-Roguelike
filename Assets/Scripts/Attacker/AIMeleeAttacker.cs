using UnityEngine;
using Interfaces;

namespace Attacker
{
    public class AIMeleeAttacker : BaseMeleeAttacker, IAttacker
    {
        public float timeToAttack = 1;
        private float _timeInZone = 0f;

        void Update()
        {
            if (targetsInRange.Count > 0)
            {
                _timeInZone += Time.deltaTime;
                if (_timeInZone >= timeToAttack && !IsOnCooldown())
                {
                    Attack();
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
            StartCooldown();
            _timeInZone = 0f;

            foreach (var target in targetsInRange)
            {
                if (target)
                {
                    target.GetComponent<IDamageable>()?.TakeDamage(CalculateDamage());
                }
            }
        }
    }
}