using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AnalyticsPanel : IEditorWindowComponent
    {
        private const string HEADING = "Help us improve Ready Player Me SDK";
        private const string DESCRIPTION =
            "We are constantly adding new features and improvements to our SDK. Enable analytics and help us in building even better free tools for more developers. This data is used for internal purposes only and is not shared with third parties.";
        private const string ANALYTICS_PRIVACY_TEXT = "Read our Privacy Policy and learn how we use the data <b>here</b>";
        private const string ANALYTICS_PRIVACY_URL =
            "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/help-us-improve-the-unity-sdk";

        private const string ENABLE_ANALYTICS = "Analytics Enabled";

        private static bool enableAnalytics;

        private readonly GUILayoutOption toggleWidth = GUILayout.Width(20);
        private GUIStyle buttonStyle;

        private bool variablesLoaded;

        public UnityEvent OnButtonClick = new UnityEvent();

        private void LoadCachedVariables()
        {
            enableAnalytics = ProjectPrefs.GetBool(SetupWizard.NeverAskAgainPref);
            variablesLoaded = true;
        }


        public void Draw(Rect position = new Rect())
        {
            if (!variablesLoaded) LoadCachedVariables();

            HeadingAndDescriptionField.SetDescription(HEADING, DESCRIPTION, () =>
            {
                GUILayout.Space(20);
                if (GUILayout.Button(ANALYTICS_PRIVACY_TEXT, new GUIStyle(GUI.skin.label)
                    {
                        richText = true,
                        fixedWidth = 435,
                        margin = new RectOffset(15,0,0,0),
                        normal =
                        {
                            textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f)
                        }
                    }))
                {
                    Application.OpenURL(ANALYTICS_PRIVACY_URL);
                }
                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            });
            
            GUILayout.FlexibleSpace();

            Layout.Horizontal(() =>
            {
                GUILayout.Space(15);
                enableAnalytics = EditorGUILayout.Toggle(enableAnalytics, toggleWidth);
                GUILayout.Label(ENABLE_ANALYTICS);
                GUILayout.FlexibleSpace();
                ProjectPrefs.SetBool(SetupWizard.NeverAskAgainPref, enableAnalytics);
            });

            GUILayout.Space(10);
        }
    }
}
