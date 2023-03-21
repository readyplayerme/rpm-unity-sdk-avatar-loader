using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class UtilsTests
    {
        private const string PARAM_HASH = "12345";

        private string defaultAvatarFolder;
        private GameObject multiMeshAvatar;
        private GameObject singleMeshAvatar;

        [OneTimeSetUp]
        public void Initialize()
        {
            var multiMeshAvatarPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(TestUtils.MULTI_MESH_MALE_PROCESSED_AVATAR_PATH);
            multiMeshAvatar = Object.Instantiate(multiMeshAvatarPrefab);

            var singleMeshAvatarPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(TestUtils.SINGLE_MESH_FEMALE_PROCESSED_AVATAR_PATH);
            singleMeshAvatar = Object.Instantiate(singleMeshAvatarPrefab);

            defaultAvatarFolder = DirectoryUtility.DefaultAvatarFolder;
        }

        #region Coroutine Runner Tests

        private IEnumerator TestCoroutine()
        {
            yield return null;
        }

        [Test, Order(0)]
        public void Coroutine_Runner_Spawned_In_Scene()
        {
            TestCoroutine().Run();
            ExtensionMethods.CoroutineRunner[] operation = Resources.FindObjectsOfTypeAll<ExtensionMethods.CoroutineRunner>();
            Assert.IsNotNull(operation);
        }

        [UnityTest, Order(1)]
        public IEnumerator Coroutine_Runner_Spawned_In_Scene_Only_Once()
        {
            TestCoroutine().Run();
            TestCoroutine().Run();
            yield return null;
            ExtensionMethods.CoroutineRunner[] operation = Resources.FindObjectsOfTypeAll<ExtensionMethods.CoroutineRunner>();
            Assert.AreEqual(1, operation.Length);
        }

        [Test, Order(2)]
        public void Coroutine_Runner_Has_Hide_Flags()
        {
            TestCoroutine().Run();
            ExtensionMethods.CoroutineRunner[] operation = Resources.FindObjectsOfTypeAll<ExtensionMethods.CoroutineRunner>();
            Assert.AreEqual(31, (int) operation[0].hideFlags);
        }

        #endregion

        #region Mesh Picker Tests

        [Test]
        public void Get_Head_Mesh()
        {
            SkinnedMeshRenderer mesh = multiMeshAvatar.GetMeshRenderer(MeshType.HeadMesh);

            Assert.IsNotNull(mesh);
        }

        [Test]
        public void Get_Beard_Mesh()
        {
            SkinnedMeshRenderer mesh = multiMeshAvatar.GetMeshRenderer(MeshType.BeardMesh);

            Assert.IsNotNull(mesh);
        }

        [Test]
        public void Get_Teeth_Mesh()
        {
            SkinnedMeshRenderer mesh = multiMeshAvatar.GetMeshRenderer(MeshType.TeethMesh);

            Assert.IsNotNull(mesh);
        }

        [Test]
        public void Get_Avatar_Mesh()
        {
            SkinnedMeshRenderer mesh = singleMeshAvatar.GetMeshRenderer(MeshType.HeadMesh);

            Assert.IsNotNull(mesh);
        }

        [Test]
        public void Fail_Get_Beard_Mesh_On_Single_Mesh_Avatar()
        {
            SkinnedMeshRenderer mesh = singleMeshAvatar.GetMeshRenderer(MeshType.BeardMesh);

            Assert.IsNull(mesh);
        }

        [Test]
        public void Fail_Get_Teeth_Mesh_On_Single_Mesh_Avatar()
        {
            SkinnedMeshRenderer mesh = singleMeshAvatar.GetMeshRenderer(MeshType.TeethMesh);

            Assert.IsNull(mesh);
        }

        [Test]
        public void Fail_Get_Mesh_Game_Object_With_No_SkinnedMeshRenderer()
        {
            var gameObject = new GameObject();
            SkinnedMeshRenderer mesh = gameObject.GetMeshRenderer(MeshType.HeadMesh);

            Assert.IsNull(mesh);
        }

        #endregion

        #region Custom Event Arguments Tests

        private event EventHandler<FailureEventArgs> OnFailed;
        private event EventHandler<ProgressChangeEventArgs> OnProgressChanged;
        private event EventHandler<CompletionEventArgs> OnCompleted;

        [Test]
        public void Custom_Event_Args_Failure_Test()
        {
            object sender = null;
            FailureEventArgs args = null;

            OnFailed += (s, a) =>
            {
                sender = s;
                args = a;
            };
            OnFailed?.Invoke(this, new FailureEventArgs
            {
                Url = "url",
                Message = "message",
                Type = FailureType.ModelImportError
            });

            Assert.AreEqual(this, sender);
            Assert.AreEqual("url", args.Url);
            Assert.AreEqual("message", args.Message);
            Assert.AreEqual(FailureType.ModelImportError, args.Type);
        }

        [Test]
        public void Custom_Event_Args_Progress_Change_Test()
        {
            object sender = null;
            ProgressChangeEventArgs args = null;

            OnProgressChanged += (s, a) =>
            {
                sender = s;
                args = a;
            };

            // TODO Fix this 
            OnProgressChanged?.Invoke(this, new ProgressChangeEventArgs
            {
                Url = "url",
                Progress = 0.5f,
                Operation = nameof(AvatarObjectLoader)
            });

            Assert.AreEqual(this, sender);
            Assert.AreEqual("url", args.Url);
            Assert.AreEqual(0.5f, args.Progress);
            Assert.AreEqual(nameof(AvatarObjectLoader), args.Operation);
        }

        [Test]
        public void Custom_Event_Args_Completion_Test()
        {
            object sender = null;
            CompletionEventArgs args = null;

            OnCompleted += (s, a) =>
            {
                sender = s;
                args = a;
            };
            OnCompleted?.Invoke(this, new CompletionEventArgs
            {
                Url = "url",
                Avatar = new GameObject { name = "avatar" }
            });

            Assert.AreEqual(this, sender);
            Assert.AreEqual("url", args.Url);
            Assert.AreEqual("avatar", args.Avatar.name);
        }

        #endregion

        #region Directory Utility Tests

        private void CleanupDirectory()
        {
            TestUtils.DeleteDirectoryIfExists($"{Application.persistentDataPath}/{DirectoryUtility.DefaultAvatarFolder}", true);
            DirectoryUtility.DefaultAvatarFolder = defaultAvatarFolder;
        }

        [Test]
        public void Get_Avatar_Save_Directory()
        {
            DirectoryUtility.DefaultAvatarFolder = "TestAvatarFolder";
            var directory = DirectoryUtility.GetAvatarSaveDirectory(TestUtils.CloudfrontUri.Guid, false, PARAM_HASH);

            Assert.AreEqual($"{Application.persistentDataPath}/{DirectoryUtility.DefaultAvatarFolder}/{TestUtils.CloudfrontUri.Guid}/{PARAM_HASH}", directory);

            CleanupDirectory();
        }

        [Test]
        public void Get_Avatar_Save_Directory_In_Project_Folder()
        {
            DirectoryUtility.DefaultAvatarFolder = "TestAvatarFolder";

            var directory = DirectoryUtility.GetAvatarSaveDirectory(TestUtils.CloudfrontUri.Guid, true);

            Assert.AreEqual($"{Application.dataPath}/{DirectoryUtility.DefaultAvatarFolder}/{TestUtils.CloudfrontUri.Guid}", directory);

            CleanupDirectory();
        }

        [Test]
        public void Validate_Avatar_Save_Directory()
        {
            DirectoryUtility.DefaultAvatarFolder = "TestAvatarFolder";
            var path = $"{Application.persistentDataPath}/{DirectoryUtility.DefaultAvatarFolder}/{TestUtils.CloudfrontUri.Guid}";

            Assert.IsFalse(Directory.Exists(path));

            DirectoryUtility.ValidateAvatarSaveDirectory(TestUtils.CloudfrontUri.Guid);

            Assert.IsTrue(Directory.Exists(path));

            CleanupDirectory();
        }

        #endregion
    }
}
