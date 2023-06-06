using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AvatarConfigTemplate : VisualElement
    {
        private const string XML_PATH = "AvatarConfigTemplate";
        private const string AVATAR_CONFIG_FIELD = "AvatarConfigField";

        public new class UxmlFactory : UxmlFactory<AvatarConfigTemplate, UxmlTraits>
        {
        }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }

        private readonly AvatarLoaderSettings avatarLoaderSettings;
        private AvatarConfig avatarConfig;

        public AvatarConfigTemplate()
        {
            avatarLoaderSettings = AvatarLoaderSettings.LoadSettings();

            var visualTree = Resources.Load<VisualTreeAsset>(XML_PATH);
            visualTree.CloneTree(this);

            var avatarConfigField = this.Q<ObjectField>(AVATAR_CONFIG_FIELD);
            avatarConfigField.value = avatarLoaderSettings.AvatarConfig;
            avatarConfigField.RegisterValueChangedCallback(OnAvatarConfigChanged);
        }

        private void OnAvatarConfigChanged(ChangeEvent<Object> evt)
        {
            avatarConfig = evt.newValue as AvatarConfig;
            SaveAvatarConfig();
        }

        private void SaveAvatarConfig()
        {
            if (avatarLoaderSettings != null && avatarLoaderSettings.AvatarConfig != avatarConfig)
            {
                avatarLoaderSettings.AvatarConfig = avatarConfig;
                SaveAvatarLoaderSettings();
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
