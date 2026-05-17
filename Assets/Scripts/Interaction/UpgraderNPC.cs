using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers;

namespace Interaction
{
    public class UpgraderNPC : InteractableItem
    {
        public List<UpgradeLevel> upgradeLevels;

        public int currentArmorLevelIndex = 0;
        public int currentDamageLevelIndex = 0;
        public int currentHealthLevelIndex = 0;

        private void Awake()
        {
            upgradeLevels = UpgraderDataLoader.LoadData();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                _isPlayerInRange = true;
                _targetPlayer = player;
                _targetPlayer.TogglePickItemPrompt(true, "Press 'E' to open upgrade menu");
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (_targetPlayer)
                {
                    _targetPlayer.TogglePickItemPrompt(false);
                    _isPlayerInRange = false;
                    _targetPlayer = null;
                }
            }
        }

        protected override void OnInteraction()
        {
            if (_targetPlayer != null)
            {
                // Open menu
                UI.UpgraderMenuUI.Instance.OpenMenu(this, _targetPlayer);
                _targetPlayer.TogglePickItemPrompt(false);
            }
        }
    }
}

