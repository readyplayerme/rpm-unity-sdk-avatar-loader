using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    [InitializeOnLoad]
    public class SetupWizardWindow : EditorWindow
    {
        private const string WINDOW_NAME = "Setup Wizard";
        private const int BUTTON_FONT_SIZE = 12;

        public const string FIRST_TIME_SETUP = "first-time-setup";

        private Header header;
        private GUIStyle descriptionStyle;
        private GUIStyle headingStyle;
        private AnalyticsPanel analyticsPanel;
        private SubdomainPanel subdomainPanel;
        private AvatarConfigPanel avatarConfigPanel;
        private QuickStartPanel quickStartPanel;
        private bool displayQuickStart;

        private IEditorWindowComponent[] panels;

        private int currentPanelIndex;

        /// <summary>
        ///     Constructor method that subscribes to the StartUp event.
        /// </summary>
        static SetupWizardWindow()
        {
            EntryPoint.Startup += OnStartup;
        }

        /// <summary>
        ///     This method is called when Unity Editor is closed or this package is removed.
        /// </summary>
        private void OnDestroy()
        {
            EntryPoint.Startup -= OnStartup;
        }

        /// <summary>
        ///     This method is called when a Unity project is opened or after this Unity package has finished importing and is
        ///     responsible for displaying the window. It also calls analytics events if enabled.
        /// </summary>
        private static void OnStartup()
        {
            if (CanShowWindow())
            {
                ShowWindow();
            }

            if (AnalyticsEditorLogger.IsEnabled)
            {
                AnalyticsEditorLogger.EventLogger.LogOpenProject();
                AnalyticsEditorLogger.EventLogger.IdentifyUser();
                EditorApplication.quitting += OnQuit;
            }
        }

        private static bool CanShowWindow()
        {
            return !ProjectPrefs.GetBool(FIRST_TIME_SETUP);
        }

        /// <summary>
        ///     This method is called when the Unity Editor is closed and logs the close event.
        /// </summary>
        private static void OnQuit()
        {
            AnalyticsEditorLogger.EventLogger.LogCloseProject();
        }

        [MenuItem("Ready Player Me/Re-run Setup")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SetupWizardWindow), false, WINDOW_NAME);
        }

        /// <summary>
        ///     This method the panels and banner if not set already and sets the button event listeners
        /// </summary>
        private void LoadPanels()
        {
            panels ??= new IEditorWindowComponent[]
            {
                new SubdomainPanel(),
                new AvatarConfigPanel(),
                new AnalyticsPanel(),
            };

            header ??= new Header();
            if (quickStartPanel == null)
            {
                quickStartPanel = new QuickStartPanel();
                quickStartPanel.OnQuickStartClick.AddListener(Close);
                quickStartPanel.OnCloseClick.AddListener(Close);
            }
        }

        private void OnGUI()
        {
            LoadPanels();
            DrawContent();
        }

        private GUIStyle GetButtonStyle()
        {
            return new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                fontSize = BUTTON_FONT_SIZE,
                padding = new RectOffset(10, 10, 5, 5),
                fixedHeight = 30,
                stretchWidth = true,
            };
        }

        private void DrawContent()
        {
            minSize = new Vector2(460, 400);

            Layout.Horizontal(() =>
            {
                GUILayout.FlexibleSpace();

                Layout.Vertical(() =>
                {
                    header.Draw(position);
                    panels[currentPanelIndex].Draw(position);
                    GUILayout.FlexibleSpace();
                    DrawFooter(panels[currentPanelIndex]);
                }, false, GUILayout.Height(400));
            });
        }

        private void DrawFooter(IEditorWindowComponent panel)
        {
            Layout.Horizontal(() =>
            {
                if (!(panel is SubdomainPanel))
                {
                    GUILayout.Space(15);
                    if (GUILayout.Button("Back", GetButtonStyle()))
                    {
                        OnBackButton();
                    }
                }

                GUILayout.FlexibleSpace();

                if (panel is AnalyticsPanel)
                {
                    if (GUILayout.Button("Finish Setup", GetButtonStyle()))
                    {
                        OnFinishSetup();
                    }

                    if (GUILayout.Button("Open QuickStart Scene", GetButtonStyle()))
                    {
                        OnOpenQuickStartScene();
                    }
                }
                else
                {
                    if (panel is SubdomainPanel { IsSubdomainFieldEmpty: true })
                    {
                        GUI.enabled = false;
                    }
                    
                    if(panel is AvatarConfigPanel { IsAvatarConfigFieldEmpty: true })
                    {
                        GUI.enabled = false;
                    }
                    
                    if (GUILayout.Button("Next", GetButtonStyle()))
                    {
                        OnNextButton();
                    }
                    GUI.enabled = true;
                }

                GUILayout.Space(15);

            }, true, GUILayout.Width(460));
        }

        private void OnBackButton()
        {
            currentPanelIndex--;
        }

        private void OnNextButton()
        {
            currentPanelIndex++;
        }

        private void OnFinishSetup()
        {
            Close();
        }

        private void OnOpenQuickStartScene()
        {

        }
    }
}
