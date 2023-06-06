using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class GltfDeferAgentTemplate : VisualElement
    {
        private const string XML_PATH = "GltfDeferAgentTemplate";
        private const string DEFER_AGENT_FIELD = "DeferAgentField";

        public new class UxmlFactory : UxmlFactory<GltfDeferAgentTemplate, UxmlTraits>
        {
        }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }

        public GltfDeferAgentTemplate()
        {
            var visualTree = Resources.Load<VisualTreeAsset>(XML_PATH);
            visualTree.CloneTree(this);

            var deferAgentField = this.Q<ObjectField>(DEFER_AGENT_FIELD);
            deferAgentField.value = AvatarLoaderSettingsHelper.AvatarLoaderSettings.GLTFDeferAgent;
            deferAgentField.RegisterValueChangedCallback(OnAvatarConfigChanged);
        }

        private void OnAvatarConfigChanged(ChangeEvent<Object> evt)
        {
            AvatarLoaderSettingsHelper.SaveDeferAgent(evt.newValue as GLTFDeferAgent);
        }
    }
}
