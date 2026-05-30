using Interfaces;
using UnityEngine;
using Weapons;
using Movement;
using System.Linq;

namespace Attacker
{
    public class AIRangeAttacker : BaseRangeAttacker, IAttacker
    {
        [Tooltip("How long a target must be in range before attacking.")]
        public float timeToAttack = 1f;

        private float _timeInZone = 0f;
        private Vector2 _patrolDirection;
        private float _patrolTimer = 0f;
        private Transform _playerTransform;

        private AstarMover _mover;

        void Start()
        {
            _patrolDirection = Random.insideUnitCircle.normalized;
            _mover = GetComponent<AstarMover>();
        }

        public void Update()
        {
            SearchForPlayer();

            switch (currentState)
            {
                case AIState.PATROL:
                    HandlePatrol();
                    break;
                case AIState.CHASE:
                    HandleChase();
                    break;
                case AIState.ATTACK:
                    HandleAttackState();
                    break;
            }
        }

        private void SearchForPlayer()
        {
            if (currentState == AIState.PATROL && rangeOfSight != null)
            {
                Collider2D playerCollider = Physics2D.OverlapCircle(rangeOfSight.bounds.center, rangeOfSight.radius, targetLayers);
                if (playerCollider != null && playerCollider.CompareTag("Player"))
                {
                    _playerTransform = playerCollider.transform;
                    currentState = AIState.CHASE;
                }
            }
        }

        private void HandlePatrol()
        {
            _patrolTimer += Time.deltaTime;
            if (_patrolTimer >= 2f)
            {
                _patrolTimer = 0f;
                if (Random.value < 0.2f)
                {
                    _patrolDirection = Random.insideUnitCircle.normalized;
                }
            }
            
            transform.Translate(_patrolDirection * (moveSpeed * 0.5f * Time.deltaTime));
        }

        private void HandleChase()
        {
            if (_playerTransform == null) return;

            if (targetsInRange.Count > 0 && targetsInRange.Contains(_playerTransform.GetComponent<Collider2D>()))
            {
                currentState = AIState.ATTACK;
                return;
            }

            if (_mover != null)
            {
                _mover.MoveTowards(_playerTransform.position, moveSpeed);
            }
            else
            {
                // No AstarMover attached -> move straight at the player so chasing still works.
                transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, moveSpeed * Time.deltaTime);
            }
        }

        private void HandleAttackState()
        {
            if (targetsInRange.Count == 0 || !_playerTransform || !targetsInRange.Contains(_playerTransform.GetComponent<Collider2D>()))
            {
                currentState = AIState.CHASE;
                _timeInZone = 0f;
                return;
            }

            _timeInZone += Time.deltaTime;
            if (_timeInZone >= timeToAttack && !IsOnCooldown())
            {
                Attack();
                currentState = AIState.CHASE; // Go back to chase mode after attack
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (currentState == AIState.PATROL)
            {
                // If patrolling and we hit a wall, pick a new random direction immediately
                _patrolDirection = Random.insideUnitCircle.normalized; 
            }
        }

        // Removed OnCollisionStay2D to fix severe lag
    }
}