using UnityEditor;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public static class EditorMenu
    {
        [MenuItem("Ready Player Me/Avatar Loader", priority = 0)]
        public static void OpenAvatarLoaderWindow()
        {
            AvatarLoaderEditorWindow.ShowWindowMenu();
        }
    }
}
