using ReadyPlayerMe.AvatarLoader;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    [CustomEditor(typeof(EyeAnimationHandler))]
    public class EyeAnimationHandlerEditor : Editor
    {
        private readonly GUIContent blinkSpeedLabel =
            new GUIContent("Blink Duration", "Effects the duration of the avatar blink animation in seconds.");

        private readonly GUIContent blinkIntervalLabel =
            new GUIContent("Blink Interval", "Effects the amount of time in between each blink in seconds..");

        private SerializedProperty blinkDuration;
        private SerializedProperty blinkInterval;

        public override void OnInspectorGUI()
        {
            DrawPropertyField(blinkDuration, blinkSpeedLabel);
            DrawPropertyField(blinkInterval, blinkIntervalLabel);
        }

        private void OnEnable()
        {
            blinkDuration = serializedObject.FindProperty("blinkDuration");
            blinkInterval = serializedObject.FindProperty("blinkInterval");
        }

        private void DrawPropertyField(SerializedProperty property, GUIContent content)
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, content);
            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                (target as EyeAnimationHandler)?.Initialize();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
