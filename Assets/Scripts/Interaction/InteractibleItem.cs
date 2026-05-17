using UnityEngine;

namespace Interaction
{
    public abstract class InteractableItem : MonoBehaviour
    {
        protected bool _isPlayerInRange;
        protected Controllers.PlayerController _targetPlayer;

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Controllers.PlayerController player))
            {
                _isPlayerInRange = true;
                _targetPlayer = player;

                _targetPlayer.TogglePickItemPrompt(true);
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
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

        private void Update()
        {
            if (_isPlayerInRange && Input.GetKeyDown(KeyCode.E))
            {
                OnInteraction();
            }
        }

        protected abstract void OnInteraction();
    }
}