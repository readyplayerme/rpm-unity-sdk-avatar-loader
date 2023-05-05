using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    [InitializeOnLoad]
    public class SetupWizard : EditorWindow
    {
        private enum Panel
        {
            SubdomainPanel,
            AvatarConfigPanel,
            AnalyticsPanel
        }

        private const string WINDOW_NAME = "Welcome";
        private const int BUTTON_FONT_SIZE = 12;

        private Header header;
        private GUIStyle descriptionStyle;
        private GUIStyle headingStyle;
        private AnalyticsPanel analyticsPanel;
        private SubdomainPanel subdomainPanel;
        private AvatarConfigPanel avatarConfigPanel;
        private QuickStartPanel quickStartPanel;
        private bool displayQuickStart;

        public static readonly string NeverAskAgainPref = "rpm-sdk-metrics-never-ask-again";

        /// <summary>
        ///     Constructor method that subscribes to the StartUp event.
        /// </summary>
        static SetupWizard()
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
            return !ProjectPrefs.GetBool(NeverAskAgainPref);
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
            GetWindow(typeof(SetupWizard), false, WINDOW_NAME);
        }

        /// <summary>
        ///     This method the panels and banner if not set already and sets the button event listeners
        /// </summary>
        private void LoadPanels()
        {
            if (panels == null)
            {
                panels = new IEditorWindowComponent[]
                {
                    new SubdomainPanel(),
                    new AvatarConfigPanel(),
                    new AnalyticsPanel(),
                };
                // analyticsPanel = new AnalyticsPanel();
                // subdomainPanel = new SubdomainPanel();
                // avatarConfigPanel = new AvatarConfigPanel();
            }
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

        private IEditorWindowComponent[] panels;

        private int currentPanelIndex;

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

        }

        private void OnOpenQuickStartScene()
        {

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
                // GUILayout.Space(3);


                // GUILayout.Space(460 - 3 * GetButtonStyle().fixedWidth - 30);
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
                    // if (disableNextButton)
                    {
                        // GUI.enabled = false;
                    }
                    if (GUILayout.Button("Next", GetButtonStyle()))
                    {
                        OnNextButton();
                    }
                    // GUI.enabled = true;
                }

                GUILayout.Space(15);

            }, true, GUILayout.Width(460));
        }
    }
}
