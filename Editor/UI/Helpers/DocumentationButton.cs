using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public static class DocumentationButton
    {
        public static void Draw(string url)
        {
            if (GUILayout.Button("?", new GUIStyle(GUI.skin.button)
                {
                    fontSize = 14,
                    fixedHeight = 18,
                    fixedWidth = 18,
                    margin = new RectOffset(5, 0, 0, 8),
                    normal =
                    {
                        textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f)
                    }
                }))
            {
                Application.OpenURL(url);
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
        }
    }
}
