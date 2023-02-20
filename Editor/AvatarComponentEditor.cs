using UnityEditor;

namespace ReadyPlayerMe.AvatarLoader
{
    [CustomEditor(typeof(AvatarComponent))]
    public class AvatarComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty avatarIdProperty;
        private SerializedProperty avatarMetadataProperty;

        private void OnEnable()
        {
            var avatarComponent = (AvatarComponent) target;

            avatarIdProperty = serializedObject.FindProperty(nameof(avatarComponent.AvatarId));
            avatarMetadataProperty = serializedObject.FindProperty(nameof(avatarComponent.AvatarMetadata));
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(avatarIdProperty);
            EditorGUILayout.PropertyField(avatarMetadataProperty);
            EditorGUI.EndDisabledGroup();
        }
    }
}
