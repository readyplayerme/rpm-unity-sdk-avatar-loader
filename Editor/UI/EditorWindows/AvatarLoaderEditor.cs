using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AvatarLoaderEditor : EditorWindow
{
    private const string AVATAR_LOADER = "Avatar Loader";
    private const string LOAD_AVATAR_BUTTON_ELEMENT = "LoadAvatarButton";
    private const string URL_FIELD_ELEMENT = "UrlField";
    
    [SerializeField] private VisualTreeAsset visualTreeAsset;

    private double startTime;
    private AvatarLoaderSettings avatarLoaderSettings;

    [MenuItem("Ready Player Me/New Avatar Loader")]
    public static void ShowExample()
    {
        var window = GetWindow<AvatarLoaderEditor>();
        window.titleContent = new GUIContent(AVATAR_LOADER);
    }

    public void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);
        var headerLabel = rootVisualElement.Q<Label>("HeaderLabel");
        headerLabel.text =AVATAR_LOADER;
            
        var avatarLoader = rootVisualElement.Q<Button>(LOAD_AVATAR_BUTTON_ELEMENT);
        var urlField = rootVisualElement.Q<TextField>(URL_FIELD_ELEMENT);
        avatarLoader.clicked += () =>
        {
           
        };
    }

    private void LoadAvatar(string url,bool useEyeAnimations, bool useVoiceToAnim)
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
