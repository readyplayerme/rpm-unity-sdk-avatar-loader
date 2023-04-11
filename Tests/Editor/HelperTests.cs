using NUnit.Framework;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class HelperTests
    {
        [Test]
        public void Setup_Animator_Fullbody()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<Animator>();
            AvatarAnimatorHelper.SetupAnimator(BodyType.FullBody, gameObject);
            var animator = gameObject.GetComponent<Animator>();
            Assert.True(animator.runtimeAnimatorController != null);
        }

        [Test]
        public void Setup_Animator_Halfbody()
        {
            var gameObject = new GameObject();
            AvatarAnimatorHelper.SetupAnimator(BodyType.HalfBody, gameObject);
            var animator = gameObject.GetComponent<Animator>();
            Assert.True(animator == null);
        }
        
        [Test]
        public void Setup_Animator_Null_Avatar()
        {
            AvatarAnimatorHelper.SetupAnimator(BodyType.FullBody, null);
            Assert.Pass();
        }
        
    }
}
