using UnityEngine;

namespace ReadyPlayerMe.QuickStart
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private float speed = 6f;
        [SerializeField] private float gravity = -18f;
        [SerializeField] private Vector3 velocity;
        [SerializeField] private float jumpHeight = 3f;
        private GameObject avatar;
        [SerializeField] private Transform playerCamera;
        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        
        private static readonly int JumpTriggerHash = Animator.StringToHash("JumpTrigger");
        private float turnSmoothVelocity;
        private const float TURN_SMOOTH_TIME = 0.05f;
        private bool jumpTrigger;

        public void Setup(GameObject target)
        {
            avatar = target;
            if (playerCamera == null)
            {
                playerCamera = Camera.main.transform;
            }
        }

        public void Move(float inputX, float inputY, Animator animator)
        {
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            
            if (jumpTrigger && controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger(JumpTriggerHash);
                jumpTrigger = false;
            }

            var move = playerCamera.right * inputX + playerCamera.forward * inputY;
            controller.Move(move * speed * Time.deltaTime);

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            
            
            var moveMagnitude = move.magnitude;
            // Apply rotation if moving
            if (moveMagnitude > 0)
            {
                float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + transform.rotation.y;
                float angle = Mathf.SmoothDampAngle(avatar.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TURN_SMOOTH_TIME);
                avatar.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
            // set animation move speed property
            animator.SetFloat(MoveSpeedHash, moveMagnitude);
        }

        public void Jump()
        {
            if (controller.isGrounded)
            {
                jumpTrigger = true;
                return;
            }
            jumpTrigger = false;
        }
    }
}
