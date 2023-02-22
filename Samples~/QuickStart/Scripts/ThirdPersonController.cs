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

        private void Start()
        {
            thirdPersonMovement = GetComponent<ThirdPersonMovement>();
            playerInput = GetComponent<PlayerInput>();
            playerInput.OnJumpPress += () => thirdPersonMovement.Jump();
        }

        public void Setup(GameObject target, RuntimeAnimatorController runtimeAnimatorController)
        {
            avatar = target;
            thirdPersonMovement.Setup(avatar);
            animator = avatar.GetComponent<Animator>();
            animator.runtimeAnimatorController = runtimeAnimatorController;
            animator.applyRootMotion = false;
            
        }
        
        void Update()
        {
            if (avatar == null)
            {
                return;
            }
            
            var xAxisInput = playerInput.AxisHorizontal;
            var yAxisInput = playerInput.AxisVertical;
            thirdPersonMovement.Move(xAxisInput, yAxisInput, animator);
        }
    }
}
