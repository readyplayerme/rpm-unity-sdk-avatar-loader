using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class AvatarAPITests
    {
        private const string AVATAR_API_AVATAR_URL = "https://api.readyplayer.me/v1/avatars/638df693d72bffc6fa17943c.glb";
        private const string AVATAR_CONFIG_PATH_LOW = "Avatar Config Low";
        private const string AVATAR_CONFIG_PATH_MED = "Avatar Config Medium";
        private const string AVATAR_CONFIG_PATH_HIGH = "Avatar Config High";
        private const int TEXTURE_SIZE_LOW = 256;
        private const int TEXTURE_SIZE_MED = 512;
        private const int TEXTURE_SIZE_HIGH = 1024;
        private const int AVATAR_CONFIG_BLEND_SHAPE_COUNT_MED = 15;
#if DISABLE_AUTO_INSTALLER
        private const string CONFIG_FOLDER_PATH = "Assets/Ready Player Me/Avatar Loader/Configurations";
#else
        private const string CONFIG_FOLDER_PATH = "Packages/com.readyplayerme.avatarloader/Configurations";
#endif
        
        private AvatarConfig avatarConfigHigh;
        private AvatarConfig avatarConfigLow;
        private AvatarConfig avatarConfigMed;
        private AvatarLoaderSettings settings;

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(Object.FindObjectOfType(typeof(Animator)));
            AvatarCache.Clear();
        }

        [OneTimeSetUp]
        public void Init()
        {
            avatarConfigLow = AssetDatabase.LoadAssetAtPath<AvatarConfig>($"{CONFIG_FOLDER_PATH}/{AVATAR_CONFIG_PATH_LOW}.asset");
            avatarConfigMed = AssetDatabase.LoadAssetAtPath<AvatarConfig>($"{CONFIG_FOLDER_PATH}/{AVATAR_CONFIG_PATH_MED}.asset");
            avatarConfigHigh = AssetDatabase.LoadAssetAtPath<AvatarConfig>($"{CONFIG_FOLDER_PATH}/{AVATAR_CONFIG_PATH_HIGH}.asset");
            settings = AvatarLoaderSettings.LoadSettings();
            settings.AvatarCachingEnabled = false;
        }

        [UnityTest]
        public IEnumerator AvatarLoader_Avatar_API_Mesh_LOD()
        {
            var avatarConfigs = new Queue<AvatarConfig>();
            avatarConfigs.Enqueue(avatarConfigLow);
            avatarConfigs.Enqueue(avatarConfigMed);
            avatarConfigs.Enqueue(avatarConfigHigh);

            var vertexCounts = new List<int>();

            var failureType = FailureType.None;
            var loader = new AvatarObjectLoader();

            loader.OnCompleted += (sender, args) =>
            {
                GameObject avatar = args.Avatar;
                vertexCounts.Add(avatar.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.vertexCount);
                if (avatarConfigs.Count == 0) return;
                loader.AvatarConfig = avatarConfigs.Dequeue();
                loader.LoadAvatar(AVATAR_API_AVATAR_URL);
            };
            loader.OnFailed += (sender, args) => { failureType = args.Type; };

            loader.AvatarConfig = avatarConfigs.Dequeue();
            loader.LoadAvatar(AVATAR_API_AVATAR_URL);

            yield return new WaitUntil(() => vertexCounts.Count == 3 || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.Less(vertexCounts[0], vertexCounts[1]);
            Assert.Less(vertexCounts[1], vertexCounts[2]);
        }

        [UnityTest]
        public IEnumerator AvatarLoader_Avatar_API_TextureSize()
        {
            var avatarConfigs = new Queue<AvatarConfig>();
            avatarConfigs.Enqueue(avatarConfigLow);
            avatarConfigs.Enqueue(avatarConfigMed);
            avatarConfigs.Enqueue(avatarConfigHigh);

            var textureSizes = new List<int>();

            var failureType = FailureType.None;
            var loader = new AvatarObjectLoader();

            loader.OnCompleted += (sender, args) =>
            {
                GameObject avatar = args.Avatar;
                textureSizes.Add(avatar.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.mainTexture.width);
                if (avatarConfigs.Count == 0) return;
                loader.AvatarConfig = avatarConfigs.Dequeue();
                loader.LoadAvatar(AVATAR_API_AVATAR_URL);
            };
            loader.OnFailed += (sender, args) => { failureType = args.Type; };

            loader.AvatarConfig = avatarConfigs.Dequeue();
            loader.LoadAvatar(AVATAR_API_AVATAR_URL);

            yield return new WaitUntil(() => textureSizes.Count == 3 || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.AreEqual(TEXTURE_SIZE_LOW, textureSizes[0]);
            Assert.AreEqual(TEXTURE_SIZE_MED, textureSizes[1]);
            Assert.AreEqual(TEXTURE_SIZE_HIGH, textureSizes[2]);
        }

        [UnityTest]
        public IEnumerator AvatarLoader_Avatar_API_MorphTargets_None()
        {
            GameObject avatar = null;
            var failureType = FailureType.None;
            var loader = new AvatarObjectLoader();
            loader.OnCompleted += (sender, args) =>
            {
                avatar = args.Avatar;
            };
            loader.OnFailed += (sender, args) => { failureType = args.Type; };
            loader.AvatarConfig = avatarConfigLow;
            loader.LoadAvatar(AVATAR_API_AVATAR_URL);

            yield return new WaitUntil(() => avatar != null || failureType != FailureType.None);

            var blendShapeCount = avatar.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.blendShapeCount;
            Debug.Log(blendShapeCount);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(avatar);
            Assert.Zero(blendShapeCount);
        }

        [UnityTest]
        public IEnumerator AvatarLoader_Avatar_API_MorphTargets_Oculus()
        {
            GameObject avatar = null;
            var failureType = FailureType.None;
            var loader = new AvatarObjectLoader();
            loader.OnCompleted += (sender, args) =>
            {
                avatar = args.Avatar;
            };
            loader.OnFailed += (sender, args) => { failureType = args.Type; };
            loader.AvatarConfig = avatarConfigMed;
            loader.LoadAvatar(AVATAR_API_AVATAR_URL);

            yield return new WaitUntil(() => avatar != null || failureType != FailureType.None);

            var blendShapeCount = avatar.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.blendShapeCount;

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(avatar);
            Assert.AreEqual(AVATAR_CONFIG_BLEND_SHAPE_COUNT_MED, blendShapeCount);
        }
    }
}
