// ReSharper disable RedundantUsingDirective

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ReadyPlayerMe.Core;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif


namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This enumeration describes the options for AudioProviderType.
    /// </summary>
    public enum AudioProviderType
    {
        Microphone = 0,
        AudioClip = 1
    }

    /// <summary>
    /// This class is responsible for adding basic facial animations at runtime using microphone input and facial blendshape
    /// manipulation
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Ready Player Me/Voice Handler", 0)]
    public class VoiceHandler : MonoBehaviour
    {
        private const string MOUTH_OPEN_BLEND_SHAPE_NAME = "mouthOpen";
        private const int AMPLITUDE_MULTIPLIER = 10;
        private const int AUDIO_SAMPLE_LENGTH = 4096;
        private const int MICROPHONE_FREQUENCY = 44100;

        private float[] audioSample = new float[AUDIO_SAMPLE_LENGTH];

        // ReSharper disable InconsistentNaming
        public AudioClip AudioClip;
        public AudioSource AudioSource;
        public AudioProviderType AudioProvider = AudioProviderType.Microphone;
        private Dictionary<SkinnedMeshRenderer, int> blendshapeMeshIndexMap;

        private readonly MeshType[] faceMeshTypes = { MeshType.HeadMesh, MeshType.BeardMesh, MeshType.TeethMesh };

        private void Start()
        {
            CreateBlendshapeMeshMap();
            if (!HasMouthOpenBlendshape())
            {
                Debug.LogWarning("No mouthOpen blendshape found on Avatar mesh, disabling VoiceHandler.");
                enabled = false;
                return;
            }
#if UNITY_IOS
            CheckIOSMicrophonePermission().Run();
#elif UNITY_ANDROID
            CheckAndroidMicrophonePermission().Run();
#elif UNITY_STANDALONE || UNITY_EDITOR
            InitializeAudio();
#endif
        }

        private bool HasMouthOpenBlendshape()
        {
            foreach (var blendshapeMeshIndex in blendshapeMeshIndexMap)
            {
                if (blendshapeMeshIndex.Value >= 0)
                {
                    return true;
                }

            }
            return false;
        }

        private void CreateBlendshapeMeshMap()
        {
            blendshapeMeshIndexMap = new Dictionary<SkinnedMeshRenderer, int>();
            foreach (var faceMeshType in faceMeshTypes)
            {
                var faceMesh = gameObject.GetMeshRenderer(faceMeshType);
                if (faceMesh)
                {
                    TryAddSkinMesh(faceMesh);
                }
            }
        }

        private void TryAddSkinMesh(SkinnedMeshRenderer skinMesh)
        {
            if (skinMesh != null)
            {
                var index = skinMesh.sharedMesh.GetBlendShapeIndex(MOUTH_OPEN_BLEND_SHAPE_NAME);
                blendshapeMeshIndexMap.Add(skinMesh, index);
            }
        }

        private void Update()
        {
            var value = GetAmplitude();
            SetBlendShapeWeights(value);
        }

        public void InitializeAudio()
        {
            try
            {
                if (AudioSource == null)
                {
                    AudioSource = gameObject.AddComponent<AudioSource>();
                }

                switch (AudioProvider)
                {
                    case AudioProviderType.Microphone:
                        SetMicrophoneSource();
                        break;
                    case AudioProviderType.AudioClip:
                        SetAudioClipSource();
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("VoiceHandler.Initialize:/n" + e);
            }
        }

        private void SetMicrophoneSource()
        {
#if UNITY_WEBGL
            Debug.LogWarning("Microphone is not supported in WebGL.");
#else
            AudioSource.clip = Microphone.Start(null, true, 1, MICROPHONE_FREQUENCY);
            AudioSource.loop = true;
            AudioSource.mute = true;
            AudioSource.Play();
#endif
        }

        private void SetAudioClipSource()
        {
            AudioSource.clip = AudioClip;
            AudioSource.loop = false;
            AudioSource.mute = false;
            AudioSource.Stop();
        }

        public void PlayCurrentAudioClip()
        {
            AudioSource.Play();
        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            AudioClip = AudioSource.clip = audioClip;
            PlayCurrentAudioClip();
        }

        private float GetAmplitude()
        {
            if (CanGetAmplitude())
            {
                var amplitude = 0f;
                AudioSource.clip.GetData(audioSample, AudioSource.timeSamples);

                foreach (var sample in audioSample)
                {
                    amplitude += Mathf.Abs(sample);
                }

                return Mathf.Clamp01(amplitude / audioSample.Length * AMPLITUDE_MULTIPLIER);
            }

            return 0;
        }

        private bool CanGetAmplitude()
        {
            return AudioSource != null && AudioSource.clip != null && AudioSource.isPlaying;
        }

        #region Blend Shape Movement

        private void SetBlendShapeWeights(float weight)
        {
            foreach (var blendshapeMeshIndex in blendshapeMeshIndexMap)
            {
                if (blendshapeMeshIndex.Value >= 0)
                {
                    blendshapeMeshIndex.Key.SetBlendShapeWeight(blendshapeMeshIndex.Value, weight);
                }
            }
        }

        #endregion

        #region Permissions

#if UNITY_IOS
        private IEnumerator CheckIOSMicrophonePermission()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                InitializeAudio();
            }
            else
            {
                StartCoroutine(CheckIOSMicrophonePermission());
            }
        }
#endif

#if UNITY_ANDROID
        private IEnumerator CheckAndroidMicrophonePermission()
        {
            var wait = new WaitUntil(() =>
            {
                Permission.RequestUserPermission(Permission.Microphone);

                return Permission.HasUserAuthorizedPermission(Permission.Microphone);
            });

            yield return wait;

            InitializeAudio();
        }
#endif

        #endregion

        private void OnDestroy()
        {
            audioSample = null;
        }
    }
}
