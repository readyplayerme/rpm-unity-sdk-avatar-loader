using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReadyPlayerMe.AvatarLoader.Editor.Templates
{
    public class AvatarUrlTemplate : VisualElement
    {
        private const string XML_PATH = "AvatarUrlTemplate";
        private const string URL_ELEMENT = "UrlField";
        private const string ERROR_LABEL = "ErrorLabel";
        private const string URL_SAVE_KEY = "UrlSaveKey";

        public new class UxmlFactory : UxmlFactory<AvatarUrlTemplate, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }

        private readonly TextField urlField;
        private readonly Label errorLabel;

        public AvatarUrlTemplate()
        {
            var visualTree = Resources.Load<VisualTreeAsset>(XML_PATH);
            visualTree.CloneTree(this);

            var url = EditorPrefs.GetString(URL_SAVE_KEY);

            urlField = this.Q<TextField>(URL_ELEMENT);
            urlField.value = url;
            urlField.RegisterValueChangedCallback(OnValueChanged);

            errorLabel = this.Q<Label>(ERROR_LABEL);
            errorLabel.visible = !EditorUtilities.IsUrlShortcodeValid(url);
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            errorLabel.visible = !(!string.IsNullOrEmpty(evt.newValue) && EditorUtilities.IsUrlShortcodeValid(evt.newValue));
            EditorPrefs.SetString(URL_SAVE_KEY, evt.newValue);
        }

        public bool TryGetUrl(out string url)
        {
            if (string.IsNullOrEmpty(urlField.text))
            {
                url = string.Empty;
                return false;
            }

            url = urlField.text;
            return true;
        }
    }
}
