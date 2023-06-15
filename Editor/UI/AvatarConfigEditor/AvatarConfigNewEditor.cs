using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    [CustomEditor(typeof(AvatarConfig))]
    public class AvatarConfigNewEditor : UnityEditor.Editor
    {
        private const string DIALOG_TITLE = "Read Player Me";
        private const string DIALOG_MESSAGE = "Do you want to install Draco Compression Unity Package: com.atteneder.draco ?";
        private const string DIALOG_OK = "Ok";
        private const string DIALOG_CANCEL = "Cancel";
        private const string USE_DRACO_COMPRESSION = "UseDracoCompression";
        private const string ADD_MORPH_TARGET = "Add Morph Target";
        private const string DELETE_MORPH_TARGET = "Delete Morph Target";
        private const string REMOVE_BUTTON_TEXT = "Remove";

        [SerializeField] private VisualTreeAsset visualTreeAsset;

        private AvatarConfig avatarConfigTarget;
        private List<Label> morphTargetLabels;

        private Dictionary<VisualElement, string> morphTargetsParentVisualElement;

        private VisualElement selectedMorphTargets;

        private SerializedProperty userDracoCompressionField;
        private bool previousDracoCompressionValue;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            visualTreeAsset.CloneTree(root);

            userDracoCompressionField = serializedObject.FindProperty(USE_DRACO_COMPRESSION);
            userDracoCompressionField.boolValue = ModuleInstaller.IsModuleInstalled(ModuleList.DracoCompression.name);
            serializedObject.ApplyModifiedProperties();

            previousDracoCompressionValue = userDracoCompressionField.boolValue;

            var defaultInspector = root.Q<IMGUIContainer>("DefaultInspector");
            defaultInspector.onGUIHandler = () =>
            {
                DrawDefaultInspector();
                InitializeDefaultInspector();
            };

            avatarConfigTarget = (AvatarConfig) target;
            morphTargetLabels = AvatarMorphTarget.MorphTargetAvatarAPI.Select(x => new Label(x)).ToList();
            morphTargetsParentVisualElement = new Dictionary<VisualElement, string>();

            selectedMorphTargets = root.Q<VisualElement>("SelectedMorphTargets");

            for (var i = 0; i < avatarConfigTarget.MorphTargets.Count; i++)
            {
                var defaultIndex = AvatarMorphTarget.MorphTargetAvatarAPI.IndexOf(avatarConfigTarget.MorphTargets[i]);
                CreateNewElement(defaultIndex);
            }

            var addButton = root.Q<Button>("AddButton");
            addButton.clicked += OnAddButtonClicked;

            return root;
        }

        private void InitializeDefaultInspector()
        {
            if (!previousDracoCompressionValue && userDracoCompressionField.boolValue)
            {
                if (ModuleInstaller.IsModuleInstalled(ModuleList.DracoCompression.name))
                {
                    return;
                }

                if (EditorUtility.DisplayDialog(DIALOG_TITLE, DIALOG_MESSAGE, DIALOG_OK, DIALOG_CANCEL))
                {
                    ModuleInstaller.AddModuleRequest(ModuleList.DracoCompression.Identifier);
                }
                else
                {
                    userDracoCompressionField.boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void OnAddButtonClicked()
        {
            Undo.RecordObject(avatarConfigTarget, ADD_MORPH_TARGET);
            avatarConfigTarget.MorphTargets.Add(AvatarMorphTarget.MorphTargetAvatarAPI[0]);
            EditorUtility.SetDirty(avatarConfigTarget);
            CreateNewElement(0);
        }

        private void CreateNewElement(int popFieldDefaultIndex)
        {
            var parent = new VisualElement();
            parent.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            parent.style.justifyContent = new StyleEnum<Justify>(Justify.SpaceBetween);

            morphTargetsParentVisualElement.Add(parent, AvatarMorphTarget.MorphTargetAvatarAPI[popFieldDefaultIndex]);
            parent.Add(CreatePopupField(popFieldDefaultIndex, parent));
            parent.Add(CreateRemoveButton(parent));
            selectedMorphTargets.Add(parent);
        }

        private PopupField<Label> CreatePopupField(int defaultIndex, VisualElement parent)
        {
            return new PopupField<Label>(string.Empty,
                morphTargetLabels,
                defaultIndex,
                x =>
                {
                    avatarConfigTarget.MorphTargets[GetIndex(morphTargetsParentVisualElement[parent])] = x.text;
                    morphTargetsParentVisualElement[parent] = x.text;
                    return x.text;
                },
                x => x.text);
        }

        private VisualElement CreateRemoveButton(VisualElement parent)
        {
            var removeButton = new Button(() =>
            {
                Undo.RecordObject(avatarConfigTarget, DELETE_MORPH_TARGET);
                avatarConfigTarget.MorphTargets.RemoveAt(GetIndex(morphTargetsParentVisualElement[parent]));
                selectedMorphTargets.Remove(parent);
                EditorUtility.SetDirty(avatarConfigTarget);
            });
            removeButton.text = REMOVE_BUTTON_TEXT;
            return removeButton;
        }

        private int GetIndex(string morphTarget)
        {
            return avatarConfigTarget.MorphTargets.FindIndex(x => x == morphTarget);
        }
    }
}
