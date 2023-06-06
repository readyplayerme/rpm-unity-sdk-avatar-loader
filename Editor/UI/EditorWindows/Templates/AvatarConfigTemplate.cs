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

        public AvatarConfigTemplate()
        {

            var visualTree = Resources.Load<VisualTreeAsset>(XML_PATH);
            visualTree.CloneTree(this);

            var avatarConfigField = this.Q<ObjectField>(AVATAR_CONFIG_FIELD);
            avatarConfigField.value = AvatarLoaderSettingsHelper.AvatarLoaderSettings.AvatarConfig;
            avatarConfigField.RegisterValueChangedCallback(OnAvatarConfigChanged);
        }

        private void OnAvatarConfigChanged(ChangeEvent<Object> evt)
        {
            AvatarLoaderSettingsHelper.SaveAvatarConfig(evt.newValue as AvatarConfig);
        }
    }
}
