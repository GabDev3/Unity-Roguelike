using UnityEngine;

namespace Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 3f;
        public Rigidbody2D rigidBody;
        
        void FixedUpdate()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector2 inputVector = new Vector2(horizontal, vertical).normalized;

            rigidBody.linearVelocity = inputVector * speed;
        }
    }
}