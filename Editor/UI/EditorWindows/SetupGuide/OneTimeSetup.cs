using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    [InitializeOnLoad]
    public static class OneTimeSetup
    {
        static OneTimeSetup()
        {
            EntryPoint.Startup += OnStartup;
        }

        private static void OnStartup()
        {
            EntryPoint.Startup -= OnStartup;

            if (CanShowWindow())
            {
                AnalyticsEditorLogger.Enable();
                SetupGuide.ShowWindow();
            }

            if (AnalyticsEditorLogger.IsEnabled)
            {
                AnalyticsEditorLogger.EventLogger.LogOpenProject();
                AnalyticsEditorLogger.EventLogger.IdentifyUser();
                EditorApplication.quitting += OnQuit;

            }
        }

        private static void OnQuit()
        {
            AnalyticsEditorLogger.EventLogger.LogCloseProject();
        }

        private static bool CanShowWindow()
        {
            return !ProjectPrefs.GetBool(ProjectPrefs.FIRST_TIME_SETUP_DONE);
        }
    }
}
