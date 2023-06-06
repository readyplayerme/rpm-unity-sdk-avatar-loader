using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AvatarUrlTemplate : VisualElement
    {
        private const string XML_PATH = "AvatarUrlTemplate";
        private const string URL_SAVE_KEY = "UrlSaveKey";
        private const string URL_ELEMENT = "UrlField";
        private const string ERROR_LABEL = "ErrorLabel";
        private const string HELP_BUTTON = "HelpButton";

        private const string ERROR_HELP_URL = "https://docs.readyplayer.me/ready-player-me/avatars/avatar-creator#avatar-url-and-data-format";
        private const string LOAD_AVATAR_DOCS = "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/load-avatars#save-avatars-as-npcs-in-your-project";

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
            errorLabel.RegisterCallback<MouseDownEvent>(_ =>
            {
                Application.OpenURL(ERROR_HELP_URL);
            });
         
            var helpButton = this.Q<Button>(HELP_BUTTON);
            helpButton.clicked += () =>
            {
                Application.OpenURL(LOAD_AVATAR_DOCS);
            };
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
