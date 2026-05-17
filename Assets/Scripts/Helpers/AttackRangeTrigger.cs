using UnityEngine;
using Attacker;

namespace Helpers
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class AttackRangeTrigger : MonoBehaviour
    {
        private BaseAttacker _attacker;

        private void Awake()
        {
            _attacker = GetComponentInParent<BaseAttacker>();
            if (_attacker == null)
            {
                Debug.LogError("AttackRangeTrigger must be a child of a BaseAttacker.", this);
            }
        }

        private void Start()
        {
            var trigger = GetComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = _attacker.attackRadius;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Debug.Log("this:" + _attacker.gameObject.layer + ", other:" + other.gameObject.layer + ", intended: " +
            //           _attacker.targetLayers.value + ", calculated: " + (1 << other.gameObject.layer) + "var: " + (_attacker.targetLayers.value & (1 << other.gameObject.layer)));

            // Debug.Log(_attacker.name + " was triggered by " + other.name);
            if ((_attacker.targetLayers.value & (1 << other.gameObject.layer)) > 0)
            {
                // Debug.Log(_attacker.gameObject.layer + " added target to attack: " + other.gameObject.layer);
                _attacker.AddTarget(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Debug.Log(_attacker.targetLayers.value);
            if ((_attacker.targetLayers.value & (1 << other.gameObject.layer)) > 0)
            {
                // Debug.Log("Lost target");
                _attacker.RemoveTarget(other);
            }
        }
    }
}