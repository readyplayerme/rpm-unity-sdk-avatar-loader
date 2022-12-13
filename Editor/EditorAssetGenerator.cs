﻿using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class EditorAssetGenerator
    {
        private const string SETTINGS_SAVE_FOLDER = "Ready Player Me/Resources/Settings";
        private const string AVATAR_LOADER_ASSET_NAME = "AvatarLoaderSettings.asset";

        static EditorAssetGenerator()
        {
            ModuleInstaller.RegisteredPackage += CreateSettingsAssets;
        }
        
        public static void CreateSettingsAssets()
        {
            DirectoryUtility.ValidateDirectory($"{Application.dataPath}/{SETTINGS_SAVE_FOLDER}");
            AssetDatabase.Refresh();
            CreateAvatarLoaderSettings();
            ModuleInstaller.RegisteredPackage -= CreateSettingsAssets;
        }

        private static void CreateAvatarLoaderSettings()
        {
            var newSettings = ScriptableObject.CreateInstance<AvatarLoaderSettings>();
            newSettings.AvatarConfig = null;
            newSettings.AvatarCachingEnabled = DefaultSettings.AvatarCachingEnabled;

            AssetDatabase.CreateAsset(newSettings, $"Assets/{SETTINGS_SAVE_FOLDER}/{AVATAR_LOADER_ASSET_NAME}");
            AssetDatabase.SaveAssets();
        }
    }
}

