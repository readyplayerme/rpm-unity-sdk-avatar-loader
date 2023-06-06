using UnityEditor;
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

        private readonly AvatarLoaderSettings avatarLoaderSettings;
        private GLTFDeferAgent deferAgent;

        public GltfDeferAgentTemplate()
        {
            avatarLoaderSettings = AvatarLoaderSettings.LoadSettings();
            
            var visualTree = Resources.Load<VisualTreeAsset>(XML_PATH);
            visualTree.CloneTree(this);
            
            var deferAgentField = this.Q<ObjectField>(DEFER_AGENT_FIELD);
            deferAgentField.value = avatarLoaderSettings.GLTFDeferAgent;
            deferAgentField.RegisterValueChangedCallback(OnAvatarConfigChanged);
        }

        private void OnAvatarConfigChanged(ChangeEvent<Object> evt)
        {
            deferAgent = evt.newValue as GLTFDeferAgent;
            SaveDeferAgent();
        }
        
        private void SaveDeferAgent()
        {
            if (avatarLoaderSettings != null && avatarLoaderSettings.GLTFDeferAgent != deferAgent)
            {
                avatarLoaderSettings.GLTFDeferAgent = deferAgent;
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
