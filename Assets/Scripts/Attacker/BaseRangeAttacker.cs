﻿using UnityEditor.Experimental.GraphView;
 using UnityEngine;
using Weapons;

namespace Attacker
{
    public abstract class BaseRangeAttacker : BaseAttacker
    {
        [Header("Ranged Attacker Config")]
        [SerializeField] protected SpellData currentSpell;
        [SerializeField] protected Transform firePoint;
        
        /// <summary>
        /// Shoots a projectile based on the assigned SpellData.
        /// This can be overridden by child classes for different behaviors.
        /// </summary>
        /// <param name="direction">The direction to shoot in.</param>
        /// <param name="targetLayer">The layer to be targeted.</param>
        public virtual void Shoot(Vector2 direction, LayerMask targetLayer)
        {
            if (IsOnCooldown() || currentSpell == null || currentSpell.ProjectilePrefab == null) return;
 
            GameObject spellInstance = Instantiate(currentSpell.ProjectilePrefab, firePoint.position, Quaternion.identity);
 
            int finalDamage = Damage + currentSpell.BaseDamage;
            if (spellInstance.TryGetComponent<SpellProjectile>(out SpellProjectile projectile))
            {
                projectile.Launch(direction, currentSpell.ProjectileSpeed, finalDamage, targetLayer);
            }
 
            StartCooldown();
        }
    }
}