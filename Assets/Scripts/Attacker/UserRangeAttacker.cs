using Interfaces;
using UnityEngine;

namespace Attacker
{
    public class UserRangeAttacker: BaseRangeAttacker, IAttacker
    {

        public int CalculateDamage()
        {
            return Damage;
        }

        public void Update()
        {
            if(!IsOnCooldown() && Input.GetKeyDown(KeyCode.Space))
            {
                Attack();
            }
            
            
            // Debug.Log(targetLayers.value + "Is User Range attacker's target layer");
        }
        public void Attack()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 shootDirection = (Vector2)mouseWorldPosition - (Vector2)firePoint.position;
            Shoot(shootDirection, targetLayers);
        }
    }
}