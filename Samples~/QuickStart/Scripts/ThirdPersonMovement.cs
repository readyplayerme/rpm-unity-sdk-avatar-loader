using System;
using UnityEngine;

namespace ReadyPlayerMe.QuickStart
{
    [RequireComponent(typeof(CharacterController), typeof(GroundCheck))]
    public class ThirdPersonMovement : MonoBehaviour
    {
        private const float TURN_SMOOTH_TIME = 0.05f;

        [SerializeField] private Transform playerCamera;
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float gravity = -18f;
        [SerializeField] private float jumpHeight = 3f;

        private CharacterController controller;
        private GameObject avatar;
        
        private Vector3 velocity;
        private float turnSmoothVelocity;

        private bool jumpTrigger;
        public float CurrentMoveSpeed { get; private set; }
        private bool isRunning;

        private GroundCheck groundCheck;
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            groundCheck = GetComponent<GroundCheck>();
        }

        public void Setup(GameObject target)
        {
            avatar = target;
            if (playerCamera == null)
            {
                playerCamera = Camera.main.transform;
            }
        }

        public void Move(float inputX, float inputY)
        {
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            
            if (jumpTrigger && controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpTrigger = false;
            }
            
            var move = playerCamera.right * inputX + playerCamera.forward * inputY;
            var moveSpeed = isRunning ? runSpeed: walkSpeed;

            controller.Move(move * moveSpeed * Time.deltaTime);

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            
            
            var moveMagnitude = move.magnitude;
            CurrentMoveSpeed = isRunning ? runSpeed * moveMagnitude : walkSpeed * moveMagnitude;
            
            // Apply rotation if moving
            if (moveMagnitude > 0)
            {
                float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + transform.rotation.y;
                float angle = Mathf.SmoothDampAngle(avatar.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TURN_SMOOTH_TIME);
                avatar.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }

        public void SetIsRunning(bool running)
        {
            isRunning = running;
        }
        
        public bool TryJump()
        {
            jumpTrigger = false;
            if (controller.isGrounded)
            {
                jumpTrigger = true;
            }
            return jumpTrigger;
        }

        public bool IsGrounded()
        {
            if (velocity.y > 0)
            {
                return false;
            }
            return groundCheck.IsGrounded();
        }
    }
}
