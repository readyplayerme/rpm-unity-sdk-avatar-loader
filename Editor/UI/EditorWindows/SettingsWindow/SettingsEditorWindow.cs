using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class SettingsEditorWindow : EditorWindow
{
    private const string SETTINGS = "Settings";
    private const string HEADER_LABEL = "HeaderLabel";

    [SerializeField] private VisualTreeAsset visualTreeAsset;

    
    [MenuItem("Ready Player Me/New Settings 2", priority = 1)]
    public static void ShowWindow()
    {
        SettingsEditorWindow window = GetWindow<SettingsEditorWindow>();
        window.titleContent = new GUIContent("SettingsEditorWindow");
    }

    public void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);
        
        var headerLabel = rootVisualElement.Q<Label>(HEADER_LABEL);
        headerLabel.text = SETTINGS;
    }
}
