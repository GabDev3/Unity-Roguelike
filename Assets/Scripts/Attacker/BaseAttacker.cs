using UnityEngine;
using Interfaces;
using System.Collections.Generic;

namespace Attacker
{
    public enum AIState { PATROL, CHASE, ATTACK }

    public abstract class BaseAttacker : MonoBehaviour
    {
        public int Damage;
        public float AttackCooldown;
        private float _nextAttackTime = 0f;
        public float attackRadius;

        public AIState currentState = AIState.PATROL;
        public CircleCollider2D rangeOfSight;

        [Tooltip("Speed matrix for movement")]
        public float moveSpeed = 2f;

        [Tooltip("Which layers should be targeted?")]
        public LayerMask targetLayers;

        protected readonly List<Collider2D> targetsInRange = new List<Collider2D>();

        protected bool IsOnCooldown()
        {
            return Time.time < _nextAttackTime;
        }

        protected void StartCooldown()
        {
            _nextAttackTime = Time.time + AttackCooldown;
        }

        public void AddTarget(Collider2D target)
        {
            if (!targetsInRange.Contains(target))
            {
                targetsInRange.Add(target);
            }
        }

        public void RemoveTarget(Collider2D target)
        {
            targetsInRange.Remove(target);
        }
    }
}