using UnityEngine;
using Interfaces;
using Controllers;

namespace Attacker
{
    public class UserMeleeAttacker : BaseMeleeAttacker, IAttacker
    {
        private PlayerController _playerController;
        [SerializeField] private int baseDamage = 5;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0) && !IsOnCooldown())
            {
                Attack();
            }
        }

        public int CalculateDamage()
        {
            int weaponDamage = _playerController != null ? _playerController.CurrentWeaponDamage : 0;
            return baseDamage + weaponDamage;
        }

        public void Attack()
        {
            StartCooldown();
            foreach (var target in targetsInRange)
            {
                target.GetComponent<IDamageable>()?.TakeDamage(CalculateDamage());
            }
        }
    }
}