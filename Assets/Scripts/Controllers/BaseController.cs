using UnityEngine;

namespace Controllers
{
    public abstract class BaseController : MonoBehaviour
    {
        [Header("Stats")]
        public Health health;
        public int Resistance;
        
        
        private void Awake()
        {
            health = GetComponent<Health>();
        }

    }
}