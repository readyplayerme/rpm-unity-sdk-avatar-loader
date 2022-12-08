using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Core
{
    [CustomEditor(typeof(AvatarConfig))]
    public class AvatarConfigEditor : UnityEditor.Editor
    {
        private const string USE_DRACO_COMPRESSION = "UseDracoCompression";

        private AvatarConfig avatarConfigTarget;
        private SerializedProperty userDracoCompressionField;

        public override void OnInspectorGUI()
        {
            avatarConfigTarget = (AvatarConfig) target;
            var previousValue = userDracoCompressionField.boolValue;

            DrawDefaultInspector();
            DrawMorphTargets();

            if (previousValue != userDracoCompressionField.boolValue && userDracoCompressionField.boolValue)
            {
                if (ModuleInstaller.IsModuleInstalled(ModuleList.DracoCompression.name)) return;
                if (EditorUtility.DisplayDialog("Read Player Me", "Do you want to install Draco Compression Unity Package: com.atteneder.draco ?",
                        "Ok", "Cancel"))
                {
                    ModuleInstaller.AddModuleRequest(ModuleList.DracoCompression.Identifier);
                }
            }
        }

        private void OnEnable()
        {
            userDracoCompressionField = serializedObject.FindProperty(USE_DRACO_COMPRESSION);
        }

        private void DrawMorphTargets()
        {
            GUILayout.Space(5);
            GUILayout.Label("Morph Targets", EditorStyles.boldLabel);
            GUILayout.Space(3);
            for (var i = 0; i < avatarConfigTarget.MorphTargets.Count; i++)
            {
                DrawMorphTarget(i);
            }
            DrawAddMorphTargetButton();
        }

        private void DrawMorphTarget(int targetIndex)
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                int index = AvatarMorphTarget.MorphTargetAvatarAPI.IndexOf(avatarConfigTarget.MorphTargets[targetIndex]);
                int selected = EditorGUILayout.Popup(index, AvatarMorphTarget.MorphTargetAvatarAPI.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(avatarConfigTarget, "Modify Morph Target");
                    avatarConfigTarget.MorphTargets[targetIndex] = AvatarMorphTarget.MorphTargetAvatarAPI[selected];
                    EditorUtility.SetDirty(avatarConfigTarget);
                }

                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    Undo.RecordObject(avatarConfigTarget, "Delete Morph Target");
                    avatarConfigTarget.MorphTargets.RemoveAt(targetIndex);
                    EditorUtility.SetDirty(avatarConfigTarget);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawAddMorphTargetButton()
        {
            GUILayout.Space(3);
            if (GUILayout.Button("Add", GUILayout.Height(30)))
            {
                Undo.RecordObject(avatarConfigTarget, "Add Morph Target");
                avatarConfigTarget.MorphTargets.Add(AvatarMorphTarget.MorphTargetAvatarAPI[0]);
                EditorUtility.SetDirty(avatarConfigTarget);
            }
        }
    }
}
