using UnityEngine;
using Interaction;
using Controllers;

namespace Interaction
{
    public class MeleeWeapon : InteractableItem
    {
        public int Damage = 10;
        public string WeaponName = "Iron Sword";

        // Implementation of the abstract OnPickup
        protected override void OnInteraction()
        {
            if (_targetPlayer != null)
            {
                _targetPlayer.EquipWeapon(this);
                
            }
        }
    }
}