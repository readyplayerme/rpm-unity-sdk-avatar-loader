using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AvatarLoaderEditor : EditorWindow
    {
        private const string AVATAR_LOADER = "Avatar Loader";
        private const string LOAD_AVATAR_BUTTON = "LoadAvatarButton";
        private const string HEADER_LABEL = "HeaderLabel";
        private const string USE_EYE_ANIMATIONS_TOGGLE = "UseEyeAnimationsToggle";
        private const string USE_VOICE_TO_ANIMATION_TOGGLE = "UseVoiceToAnimationToggle";

        private const string VOICE_TO_ANIM_SAVE_KEY = "VoiceToAnimSaveKey";
        private const string EYE_ANIMATION_SAVE_KEY = "EyeAnimationSaveKey";
        private const string MODEL_CACHING_SAVE_KEY = "ModelCachingSaveKey";

        [SerializeField] private VisualTreeAsset visualTreeAsset;

        private double startTime;
        private AvatarLoaderSettings avatarLoaderSettings;

        private bool useEyeAnimations;
        private bool useVoiceToAnim;

        [MenuItem("Ready Player Me/New Avatar Loader 2", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<AvatarLoaderEditor>();
            window.titleContent = new GUIContent(AVATAR_LOADER);
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);

            if (EditorPrefs.GetBool(MODEL_CACHING_SAVE_KEY)) EditorPrefs.SetBool(MODEL_CACHING_SAVE_KEY, false);

            var headerLabel = rootVisualElement.Q<Label>(HEADER_LABEL);
            headerLabel.text = AVATAR_LOADER;

            var useEyeAnimationsToggle = rootVisualElement.Q<Toggle>(USE_EYE_ANIMATIONS_TOGGLE);
            useEyeAnimationsToggle.value = EditorPrefs.GetBool(EYE_ANIMATION_SAVE_KEY);
            useEyeAnimationsToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                useEyeAnimations = evt.newValue;
                EditorPrefs.SetBool(EYE_ANIMATION_SAVE_KEY, useEyeAnimations);
            });

            var useVoiceToAnimToggle = rootVisualElement.Q<Toggle>(USE_VOICE_TO_ANIMATION_TOGGLE);
            useVoiceToAnimToggle.value = EditorPrefs.GetBool(VOICE_TO_ANIM_SAVE_KEY);
            useVoiceToAnimToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                useVoiceToAnim = evt.newValue;
                EditorPrefs.SetBool(VOICE_TO_ANIM_SAVE_KEY, useVoiceToAnim);
            });

            var avatarLoader = rootVisualElement.Q<Button>(LOAD_AVATAR_BUTTON);
            var urlField = rootVisualElement.Q<AvatarUrlTemplate>();

            avatarLoader.clicked += () =>
            {
                if (urlField.TryGetUrl(out string url))
                {
                    LoadAvatar(url);
                }
            };
        }

        private void LoadAvatar(string url)
        {
            startTime = EditorApplication.timeSinceStartup;

            AnalyticsEditorLogger.EventLogger.LogLoadAvatarFromDialog(url, useEyeAnimations, useVoiceToAnim);
            if (avatarLoaderSettings == null)
            {
                avatarLoaderSettings = AvatarLoaderSettings.LoadSettings();
            }
            var avatarLoader = new AvatarObjectLoader();
            avatarLoader.SaveInProjectFolder = true;
            avatarLoader.OnFailed += Failed;
            avatarLoader.OnCompleted += Completed;
            avatarLoader.OperationCompleted += OnOperationCompleted;
            if (avatarLoaderSettings != null)
            {
                avatarLoader.AvatarConfig = avatarLoaderSettings.AvatarConfig;
                if (avatarLoaderSettings.GLTFDeferAgent != null)
                {
                    avatarLoader.GLTFDeferAgent = avatarLoaderSettings.GLTFDeferAgent;
                }
            }
            avatarLoader.LoadAvatar(url);
        }

        private void OnOperationCompleted(object sender, IOperation<AvatarContext> e)
        {
            if (e.GetType() == typeof(MetadataDownloader))
            {
                AnalyticsEditorLogger.EventLogger.LogMetadataDownloaded(EditorApplication.timeSinceStartup - startTime);
            }
        }

        private void Failed(object sender, FailureEventArgs args)
        {
            Debug.LogError($"{args.Type} - {args.Message}");
        }

        private void Completed(object sender, CompletionEventArgs args)
        {
            GameObject avatar = args.Avatar;

            if (useEyeAnimations) avatar.AddComponent<EyeAnimationHandler>();
            if (useVoiceToAnim) avatar.AddComponent<VoiceHandler>();

            EditorUtilities.CreatePrefab(avatar, $"{DirectoryUtility.GetRelativeProjectPath(avatar.name)}/{avatar.name}.prefab");

            Selection.activeObject = args.Avatar;
            AnalyticsEditorLogger.EventLogger.LogAvatarLoaded(EditorApplication.timeSinceStartup - startTime);
        }
    }
}
