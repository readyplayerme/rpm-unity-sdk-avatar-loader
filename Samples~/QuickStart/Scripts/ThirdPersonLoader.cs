using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace ReadyPlayerMe.QuickStart
{
    public class ThirdPersonLoader : MonoBehaviour
    {
        [SerializeField] private string avatarUrl;
        private GameObject avatar;
        private AvatarObjectLoader avatarObjectLoader;
        [SerializeField] private RuntimeAnimatorController animatorController;
        [SerializeField] private bool loadOnStart = true;
        private readonly Vector3 avatarPositionOffset = new Vector3(0, -0.08f, 0);
        [SerializeField] private GameObject previewAvatar;

        void Start()
        {
            avatarObjectLoader = new AvatarObjectLoader();
            avatarObjectLoader.OnCompleted += OnLoadCompleted;
            avatarObjectLoader.OnFailed += OnLoadFailed;

            if (loadOnStart)
            {
                LoadAvatar();
            }
        }

        private void OnLoadFailed(object sender, FailureEventArgs args)
        {

        }

        private void OnLoadCompleted(object sender, CompletionEventArgs args)
        {
            if (previewAvatar != null)
            {
                Destroy(previewAvatar);
                previewAvatar = null;
            }
            SetupAvatar(args.Avatar);
        }

        private void SetupAvatar(GameObject  targetAvatar)
        {
            avatar = targetAvatar;
            // Re-parent and reset transforms
            avatar.transform.parent = transform;
            avatar.transform.localPosition = avatarPositionOffset;
            avatar.transform.localRotation = Quaternion.Euler(0, 0, 0);
            
            var controller = GetComponent<ThirdPersonController>();
            if (controller != null)
            {
                controller.Setup(avatar, animatorController);
            }
        }

        public void LoadAvatar()
        {
            avatarObjectLoader.LoadAvatar(avatarUrl);
        }

    }
}
