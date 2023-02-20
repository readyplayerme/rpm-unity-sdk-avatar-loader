using System;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class AvatarSkeletonCreator : IOperation<AvatarContext>
    {
        public int Timeout { get; set; }
        public Action<float> ProgressChanged { get; set; }

        public Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (context.Metadata.BodyType != BodyType.FullBody)
            {
                return Task.FromResult(context);
            }

            var avatar = (GameObject) context.Data;

            var description = AvatarUtils.CreateHumanDescription(avatar, context.AvatarConfig.Pose == Pose.TPose);
            var unityAvatar = AvatarBuilder.BuildHumanAvatar(avatar, description);
            if (!unityAvatar.isValid)
            {
                Debug.Log("Create humanoid avatar not valid");
            }
            var animator = avatar.AddComponent<Animator>();
            avatar.name = avatar.name;
            animator.avatar = unityAvatar;
            animator.applyRootMotion = true;

            context.Data = avatar;
            return Task.FromResult(context);
        }
    }
}
