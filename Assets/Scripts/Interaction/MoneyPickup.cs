using System;
using UnityEngine;
using Interaction;
using Random = System.Random;

namespace Interaction
{
    public class MoneyPickup : InteractableItem
    {
        public int Amount = 1;


        protected void Awake()
        {
            Amount = new Random().Next(5, 21); // Random amount between 5 and 20
        }


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