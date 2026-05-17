using UnityEngine;

namespace Controllers
{
    public class EnemyController : BaseController, IDamageable
    {
        public void TakeDamage(int damage)
        {
            if (health)
            {
                health.DecreaseHealth(damage);
            }
        }

        public bool WillDie(int damage)
        {
            return health.CurrentHealth - damage <= 0;
        }
    }
}