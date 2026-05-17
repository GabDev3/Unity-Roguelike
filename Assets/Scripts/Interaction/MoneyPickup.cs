using UnityEngine;
using Interaction;

namespace Interaction
{
    public class MoneyPickup : InteractableItem
    {
        public int Amount = 1;

        protected override void OnInteraction()
        {
            if (_targetPlayer != null)
            {
                _targetPlayer.AddMoney(Amount);
                Destroy(gameObject);
            }
        }
    }
}