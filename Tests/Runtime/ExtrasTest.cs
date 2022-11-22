using System.Collections;
using NUnit.Framework;
using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class ExtrasTest
    {
        private GameObject singleMeshAvatarPrefab;
        private AudioClip audioClip;

        private const string EYE_BLINK_LEFT_BLEND_SHAPE_NAME = "eyeBlinkLeft";
        private Transform leftEyeBone;
        private const string FULL_BODY_LEFT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/LeftEye";
        private Transform rightEyeBone;
        private const string FULL_BODY_RIGHT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/RightEye";

        private const string MOUTH_OPEN_BLEND_SHAPE_NAME = "mouthOpen";

        [OneTimeSetUp]
        public void Initialize()
        {
            var o = new GameObject();
            o.AddComponent<AudioListener>();

            singleMeshAvatarPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(TestUtils.SINGLE_MESH_FEMALE_PROCESSED_AVATAR_PATH);
            audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(TestUtils.TestAudioClipPath);
        }

        #region Voice Handler Tests

        [UnityTest]
        public IEnumerator AudioSource_Attached_On_Start()
        {
            var avatar = Object.Instantiate(singleMeshAvatarPrefab);

            yield return new WaitForSeconds(0.5f);

            var source = avatar.GetComponent<AudioSource>();

            Assert.IsNotNull(source);
        }

        [UnityTest]
        public IEnumerator AudioProviderType_AudioClip()
        {
            var avatar = Object.Instantiate(singleMeshAvatarPrefab);
            var handler = avatar.GetComponent<VoiceHandler>();
            handler.AudioProvider = AudioProviderType.AudioClip;

            yield return new WaitForSeconds(0.5f);

            var source = avatar.GetComponent<AudioSource>();

            Assert.IsNull(source.clip);
            Assert.IsFalse(source.loop);
            Assert.IsFalse(source.mute);
        }

#if !UNITY_WEBGL
        [UnityTest]
        public IEnumerator AudioProviderType_Microphone()
        {
            var avatar = Object.Instantiate(singleMeshAvatarPrefab);
            Selection.activeObject = avatar;
            var handler = avatar.GetComponent<VoiceHandler>();
            handler.AudioProvider = AudioProviderType.Microphone;
            handler.InitializeAudio();

            yield return new WaitForSeconds(1f);

            var source = avatar.GetComponent<AudioSource>();

            Assert.AreEqual("Microphone", source.clip.name);
            Assert.IsTrue(source.loop);
            Assert.IsTrue(source.mute);
        }
#endif

        [UnityTest]
        public IEnumerator Play_Current_AudioClip()
        {
            var avatar = Object.Instantiate(singleMeshAvatarPrefab);
            var handler = avatar.GetComponent<VoiceHandler>();
            handler.AudioProvider = AudioProviderType.AudioClip;
            handler.AudioClip = audioClip;

            yield return new WaitForSeconds(0.5f);

            var source = avatar.GetComponent<AudioSource>();
            handler.PlayCurrentAudioClip();

            yield return null;

            Assert.IsTrue(source.isPlaying);
        }

        [UnityTest]
        public IEnumerator Play_Given_AudioClip()
        {
            var avatar = Object.Instantiate(singleMeshAvatarPrefab);
            var handler = avatar.GetComponent<VoiceHandler>();
            handler.AudioProvider = AudioProviderType.AudioClip;

            yield return new WaitForSeconds(0.5f);

            var source = avatar.GetComponent<AudioSource>();
            handler.PlayAudioClip(audioClip);

            yield return null;

            Assert.IsTrue(source.isPlaying);
        }


        [UnityTest]
        public IEnumerator Check_Mouth_Open_Change()
        {
            var avatar = Object.Instantiate(singleMeshAvatarPrefab);
            var handler = avatar.GetComponent<VoiceHandler>();
            handler.AudioProvider = AudioProviderType.AudioClip;
            handler.AudioClip = audioClip;

            yield return new WaitForSeconds(0.5f);

            handler.PlayCurrentAudioClip();

            var headMesh = avatar.GetMeshRenderer(MeshType.HeadMesh);
            var index = headMesh.sharedMesh.GetBlendShapeIndex(MOUTH_OPEN_BLEND_SHAPE_NAME);

            float elapsedTime = 0;
            float valueChange = 0;

            yield return new WaitUntil(() =>
            {
                elapsedTime += Time.deltaTime;
                valueChange += headMesh.GetBlendShapeWeight(index);
                return elapsedTime > 1;
            });

            Assert.GreaterOrEqual(valueChange, 100);
        }

        #endregion

        #region Eye Animation Handler Tests

        [UnityTest]
        public IEnumerator Check_Eye_Rotation_Change()
        {
            var avatar = Object.Instantiate(singleMeshAvatarPrefab);
            leftEyeBone = avatar.transform.Find(FULL_BODY_LEFT_EYE_BONE_NAME);
            rightEyeBone = avatar.transform.Find(FULL_BODY_RIGHT_EYE_BONE_NAME);

            var leftEyeInitRot = leftEyeBone.localRotation.eulerAngles.x;
            var rightEyeInitRot = rightEyeBone.localRotation.eulerAngles.x;

            var timeElapsed = 0f;
            var rotationCaptured = false;

            yield return new WaitUntil(() =>
            {
                timeElapsed += Time.deltaTime;
                var leftEyeRotChange =
                    Mathf.FloorToInt(Mathf.Abs(leftEyeInitRot - leftEyeBone.localRotation.eulerAngles.x));
                var rightEyeRotChange =
                    Mathf.FloorToInt(Mathf.Abs(rightEyeInitRot - rightEyeBone.localRotation.eulerAngles.x));
                rotationCaptured = (leftEyeRotChange > 2 && rightEyeRotChange > 2);

                return timeElapsed > 10 || rotationCaptured;
            });

            Assert.IsTrue(rotationCaptured);
        }

        [UnityTest]
        public IEnumerator Check_Eye_Blink_Change()
        {
            var avatar = Object.Instantiate(singleMeshAvatarPrefab);
            var handler = avatar.GetComponent<EyeAnimationHandler>();
            handler.BlinkSpeed = 0.1f;
            handler.BlinkInterval = 1;

            var headMesh = avatar.GetMeshRenderer(MeshType.HeadMesh);
            var index = headMesh.sharedMesh.GetBlendShapeIndex(EYE_BLINK_LEFT_BLEND_SHAPE_NAME);

            float elapsedTime = 0;
            float valueChange = 0;

            yield return new WaitUntil(() =>
            {
                elapsedTime += Time.deltaTime;
                valueChange += headMesh.GetBlendShapeWeight(index);
                return elapsedTime > 2;
            });

            Assert.GreaterOrEqual(valueChange, 100);
        }

        #endregion
    }
}
