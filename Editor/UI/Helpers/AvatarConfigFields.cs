using System.IO;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AvatarConfigFields
    {
        private const string AVATAR_CONFIG_TOOLTIP = "Assign an avatar configuration to include Avatar API request parameters.";
        private const string DEFER_AGENT_TOOLTIP = "Assign a defer agent which decides how the glTF will be loaded.";
        private const string CACHING_TOOLTIP =
            "Enable caching to improve avatar loading performance at runtime.";

#if UNITY_EDITOR_LINUX
        private const string SHOW_CACHING_FOLDER_BUTTON_TEXT = "Show in file manager";
#elif UNITY_EDITOR_OSX
        private const string SHOW_CACHING_FOLDER_BUTTON_TEXT = "Reveal in finder";
#else
        private const string SHOW_CACHING_FOLDER_BUTTON_TEXT = "Show in explorer";
#endif

        private const float BUTTON_HEIGHT = 30f;
        private const string AVATAR_CONFIG = "Avatar Config";

        private readonly GUILayoutOption inputFieldWidth = GUILayout.Width(178);
        private readonly GUILayoutOption objectFieldWidth = GUILayout.Width(253);

        private readonly AvatarLoaderSettings avatarLoaderSettings;
        public bool IsAvatarConfigFieldEmpty => avatarConfig == null;
        
        private GUIStyle avatarCachingButtonStyle;
        private AvatarConfig avatarConfig;
        private GLTFDeferAgent gltfDeferAgent;

        private bool avatarCachingEnabled;
        private bool isCacheEmpty;

        public AvatarConfigFields()
        {
            avatarLoaderSettings = AvatarLoaderSettings.LoadSettings();

            avatarCachingEnabled = avatarLoaderSettings != null && avatarLoaderSettings.AvatarCachingEnabled;
            isCacheEmpty = AvatarCache.IsCacheEmpty();
            if (avatarLoaderSettings != null)
            {
                avatarConfig = avatarLoaderSettings.AvatarConfig;
                gltfDeferAgent = avatarLoaderSettings.GLTFDeferAgent;
            }
        }

        public void DrawAvatarConfig(string label = AVATAR_CONFIG)
        {
            Layout.Horizontal(() =>
            {
                GUILayout.Space(3);
                EditorGUILayout.LabelField(new GUIContent(label, AVATAR_CONFIG_TOOLTIP), inputFieldWidth);
                avatarConfig = EditorGUILayout.ObjectField(avatarConfig, typeof(AvatarConfig), false, objectFieldWidth) as AvatarConfig;
                if (avatarLoaderSettings != null && avatarLoaderSettings.AvatarConfig != avatarConfig)
                {
                    avatarLoaderSettings.AvatarConfig = avatarConfig;
                    SaveAvatarLoaderSettings();
                }
            });
        }

        public void DrawGltfDeferAgent()
        {
            Layout.Horizontal(() =>
            {
                GUILayout.Space(3);
                EditorGUILayout.LabelField(new GUIContent("GLTF defer agent", DEFER_AGENT_TOOLTIP), inputFieldWidth);
                gltfDeferAgent = EditorGUILayout.ObjectField(gltfDeferAgent, typeof(GLTFDeferAgent), false, objectFieldWidth) as GLTFDeferAgent;
                if (avatarLoaderSettings != null && avatarLoaderSettings.GLTFDeferAgent != gltfDeferAgent)
                {
                    avatarLoaderSettings.GLTFDeferAgent = gltfDeferAgent;
                    SaveAvatarLoaderSettings();
                }
            });
        }

        public void DrawAvatarCaching()
        {
            LoadStyles();
            Layout.Horizontal(() =>
            {
                GUILayout.Space(2);
                var cachingEnabled = avatarCachingEnabled;
                avatarCachingEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Avatar caching enabled", CACHING_TOOLTIP), avatarCachingEnabled);

                if (cachingEnabled != avatarCachingEnabled && avatarLoaderSettings != null)
                {
                    avatarLoaderSettings.AvatarCachingEnabled = avatarCachingEnabled;
                    SaveAvatarLoaderSettings();
                }
            });

            GUILayout.Space(4);

            Layout.Horizontal(() =>
            {
                GUI.enabled = !isCacheEmpty;
                if (GUILayout.Button("Clear local avatar cache", avatarCachingButtonStyle))
                {
                    TryClearCache();
                    isCacheEmpty = AvatarCache.IsCacheEmpty();
                }
                GUI.enabled = true;

                if (GUILayout.Button(SHOW_CACHING_FOLDER_BUTTON_TEXT, avatarCachingButtonStyle))
                {
                    var path = DirectoryUtility.GetAvatarsDirectoryPath();
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    EditorUtility.RevealInFinder(path);
                }
            });
        }

        public void SetIsCacheEmpty()
        {
            isCacheEmpty = AvatarCache.IsCacheEmpty();
        }

        private void LoadStyles()
        {
            avatarCachingButtonStyle ??= new GUIStyle(GUI.skin.button);
            avatarCachingButtonStyle.fontStyle = FontStyle.Bold;
            avatarCachingButtonStyle.fontSize = 12;
            avatarCachingButtonStyle.padding = new RectOffset(5, 5, 5, 5);
            avatarCachingButtonStyle.fixedHeight = BUTTON_HEIGHT;
            avatarCachingButtonStyle.fixedWidth = 225;
        }

        private static void TryClearCache()
        {
            if (AvatarCache.IsCacheEmpty())
            {
                EditorUtility.DisplayDialog("Clear Cache", "Cache is already empty", "OK");
                return;
            }
            var size = (AvatarCache.GetCacheSize() / (1024f * 1024)).ToString("F2");
            var avatarCount = AvatarCache.GetAvatarCount();
            if (EditorUtility.DisplayDialog("Clear Cache", $"Do you want to clear all the Avatars cache from persistent data path, {size} MB and {avatarCount} avatars?", "Ok", "Cancel"))
            {
                AvatarCache.Clear();
            }
        }

        private void SaveAvatarLoaderSettings()
        {
            EditorUtility.SetDirty(avatarLoaderSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
