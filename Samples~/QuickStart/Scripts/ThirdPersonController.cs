using UnityEngine;

namespace ReadyPlayerMe.QuickStart
{
    [RequireComponent(typeof(ThirdPersonMovement),typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        private Transform playerCamera;
        private Animator animator;
        private Vector2 inputVector;
        private Vector3 moveVector;
        private GameObject avatar;
        private ThirdPersonMovement thirdPersonMovement;
        private PlayerInput playerInput;
        
        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int JumpTriggerHash = Animator.StringToHash("JumpTrigger");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        
        [SerializeField] private bool inputEnabled = true;

        private void Start()
        {
            thirdPersonMovement = GetComponent<ThirdPersonMovement>();
            playerInput = GetComponent<PlayerInput>();
            playerInput.OnJumpPress += OnJump;
        }

        public void Setup(GameObject target, RuntimeAnimatorController runtimeAnimatorController)
        {
            avatar = target;
            thirdPersonMovement.Setup(avatar);
            animator = avatar.GetComponent<Animator>();
            animator.runtimeAnimatorController = runtimeAnimatorController;
            animator.applyRootMotion = false;
            
        }
        
        private void Update()
        {
            if (avatar == null)
            {
                return;
            }
            if (inputEnabled)
            {
                playerInput.CheckInput();
                var xAxisInput = playerInput.AxisHorizontal;
                var yAxisInput = playerInput.AxisVertical;
                thirdPersonMovement.Move(xAxisInput, yAxisInput);
                thirdPersonMovement.SetIsRunning(playerInput.IsHoldingLeftShift);
            }
            
            animator.SetFloat(MoveSpeedHash, thirdPersonMovement.CurrentMoveSpeed);
            animator.SetBool(IsGroundedHash, thirdPersonMovement.IsGrounded());
        }

        private void OnJump()
        {
            thirdPersonMovement.Jump();
            if (thirdPersonMovement.IsGrounded())
            {
                animator.SetTrigger(JumpTriggerHash);
            }
        }
    }
}
