using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    [InitializeOnLoad]
    public class EditorAssetGenerator
    {
        private const string SETTINGS_SAVE_FOLDER = "Ready Player Me/Resources/Settings";
        private const string AVATAR_LOADER_ASSET_NAME = "AvatarLoaderSettings.asset";

        private static readonly string UniqueProjectPref = $"rpm-sdk-avatar-loader-settings{Application.dataPath.GetHashCode()}";

        static EditorAssetGenerator()
        {
            if (!EditorPrefs.GetBool(UniqueProjectPref, false))
            {
                CreateSettingsAssets();
                EditorPrefs.SetBool(UniqueProjectPref, true);
            }
        }

        private static void CreateSettingsAssets()
        {
            if (AvatarLoaderSettings.LoadSettings() != null)
            {
                return;
            }
            DirectoryUtility.ValidateDirectory($"{Application.dataPath}/{SETTINGS_SAVE_FOLDER}");
            AssetDatabase.Refresh();
            CreateAvatarLoaderSettings();
        }

        private static void CreateAvatarLoaderSettings()
        {
            var newSettings = ScriptableObject.CreateInstance<AvatarLoaderSettings>();
            newSettings.AvatarConfig = null;
            newSettings.gltFastDeferAgent = null;
            newSettings.AvatarCachingEnabled = DefaultSettings.AvatarCachingEnabled;

            AssetDatabase.CreateAsset(newSettings, $"Assets/{SETTINGS_SAVE_FOLDER}/{AVATAR_LOADER_ASSET_NAME}");
            AssetDatabase.SaveAssets();
        }
    }
}
