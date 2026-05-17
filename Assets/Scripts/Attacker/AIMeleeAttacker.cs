using UnityEngine;
using Interfaces;
using System.Collections.Generic;

namespace Attacker
{
    public class AIMeleeAttacker : BaseMeleeAttacker, IAttacker
    {
        public float timeToAttack = 1;
        private float _timeInZone = 0f;

        private Vector2 _patrolDirection;
        private float _patrolTimer = 0f;
        private Transform _playerTransform;
        public LayerMask obstacleLayer;

        private List<Vector2> _currentPath;
        private float _pathUpdateTimer = 0f;

        void Start()
        {
            _patrolDirection = Random.insideUnitCircle.normalized;
        }

        void Update()
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

            _pathUpdateTimer -= Time.deltaTime;
            if (_pathUpdateTimer <= 0f)
            {
                _currentPath = App.DijkstraPathfinder.FindPath(transform.position, _playerTransform.position, obstacleLayer);
                _pathUpdateTimer = 0.5f;
            }

            if (_currentPath != null && _currentPath.Count > 0)
            {
                Vector2 targetPos = _currentPath[0];
                transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, targetPos) < 0.1f)
                {
                    _currentPath.RemoveAt(0);
                }
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