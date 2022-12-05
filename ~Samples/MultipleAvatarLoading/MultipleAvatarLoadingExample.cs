using System.Collections;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class MultipleAvatarLoadingExample : MonoBehaviour
    {
        private const int RADIUS = 1;
        [SerializeField]
        private string[] avatarUrls =
        {
            "https://api.readyplayer.me/v1/avatars/638df5fc5a7d322604bb3a58.glb",
            "https://api.readyplayer.me/v1/avatars/638df70ed72bffc6fa179596.glb",
            "https://api.readyplayer.me/v1/avatars/638df75e5a7d322604bb3dcd.glb",
            "https://api.readyplayer.me/v1/avatars/638df7d1d72bffc6fa179763.glb"
        };
        private List<GameObject> avatarList;

        private void Start()
        {
            ApplicationData.Log();

            avatarList = new List<GameObject>();
            var urlSet = new HashSet<string>(avatarUrls);

            StartCoroutine(LoadAvatars(urlSet));
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            if (avatarList != null)
            {
                foreach (GameObject avatar in avatarList)
                {
                    Destroy(avatar);
                }
                avatarList.Clear();
                avatarList = null;
            }
        }

        private IEnumerator LoadAvatars(HashSet<string> urlSet)
        {
            var loading = false;

            foreach (var url in urlSet)
            {
                loading = true;
                var loader = new AvatarObjectLoader();
                loader.OnCompleted += (sender, args) =>
                {
                    loading = false;
                    AvatarAnimatorHelper.SetupAnimator(args.Metadata.BodyType, args.Avatar);
                    OnAvatarLoaded(args.Avatar);
                };
                loader.LoadAvatar(url);

                yield return new WaitUntil(() => !loading);
            }
        }

        private void OnAvatarLoaded(GameObject avatar)
        {
            if (avatarList != null)
            {
                avatarList.Add(avatar);
                avatar.transform.position = Quaternion.Euler(90, 0, 0) * Random.insideUnitCircle * RADIUS;
            }
            else
            {
                Destroy(avatar);
            }
        }
    }
}
