using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AvatarConfigTemplate : VisualElement
    {
        private const string XML_PATH = "AvatarConfigTemplate";
        private const string AVATAR_CONFIG_FIELD = "AvatarConfigField";
        private const string AVATAR_CONFIG_TOOLTIP = "Assign an avatar configuration to include Avatar API request parameters.";
        private const string AVATAR_CONFIG_DOCS_LINK = "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/optimize/avatar-configuration";

        public new class UxmlFactory : UxmlFactory<AvatarConfigTemplate, UxmlTraits>
        {
        }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }

        public AvatarConfigTemplate()
        {
            var visualTree = Resources.Load<VisualTreeAsset>(XML_PATH);
            visualTree.CloneTree(this);

            this.Q<Label>("AvatarConfigLabel").tooltip = AVATAR_CONFIG_TOOLTIP;
            this.Q<Button>("AvatarConfigHelpButton").clicked += OnHelpButtonClicked;

            var avatarConfigField = this.Q<ObjectField>(AVATAR_CONFIG_FIELD);
            avatarConfigField.value = AvatarLoaderSettingsHelper.AvatarLoaderSettings.AvatarConfig;
            avatarConfigField.RegisterValueChangedCallback(OnAvatarConfigChanged);
        }

        private void OnHelpButtonClicked()
        {
            Application.OpenURL(AVATAR_CONFIG_DOCS_LINK);
        }

        private void OnAvatarConfigChanged(ChangeEvent<Object> evt)
        {
            AvatarLoaderSettingsHelper.SaveAvatarConfig(evt.newValue as AvatarConfig);
        }
    }
}
