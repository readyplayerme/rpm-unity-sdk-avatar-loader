using System.Collections.Generic;
using ReadyPlayerMe.AvatarLoader.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SetupGuide : EditorWindow
{
    private const string SETUP_GUIDE = "SetupGuide";
    [SerializeField] private VisualTreeAsset visualTreeAsset;

    private VisualElement[] panel;

    private VisualElement currentPanel;
    private int currentPanelIndex = 0;

    private Button backButton;
    private Button nextButton;
    private Button finishSetupButton;
    private Button openQuickStartButton;


    [MenuItem("Ready Player Me/Setup Guide")]
    public static void ShowExample()
    {
        var window = GetWindow<SetupGuide>();
        window.titleContent = new GUIContent(SETUP_GUIDE);
        window.minSize = new Vector2(500, 400);
    }

    public void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);

        var subdomainPanel = rootVisualElement.Q<VisualElement>("SubdomainPanel");
        subdomainPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        var avatarConfigPanel = rootVisualElement.Q<VisualElement>("AvatarConfigPanel");
        avatarConfigPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        var analyticsPanel = rootVisualElement.Q<VisualElement>("AnalyticsPanel");
        analyticsPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        panel = new[] { subdomainPanel, avatarConfigPanel, analyticsPanel };

        nextButton = rootVisualElement.Q<Button>("NextButton");
        nextButton.clicked += NextPanel;

        backButton = rootVisualElement.Q<Button>("BackButton");
        backButton.clicked += PreviousPanel;

        finishSetupButton = rootVisualElement.Q<Button>("FinishSetupButton");
        finishSetupButton.clicked += Close;

        openQuickStartButton = rootVisualElement.Q<Button>("OpenQuickStartButton");
        openQuickStartButton.clicked += OnOpenQuickStartButton;
        StartStateMachine();
    }


    private void PreviousPanel()
    {
        SetDisplay(panel[currentPanelIndex], false);
        currentPanelIndex--;
        SetDisplay(panel[currentPanelIndex], true);
        SwitchButtons();
    }

    private void NextPanel()
    {
        SetDisplay(panel[currentPanelIndex], false);
        currentPanelIndex++;
        SetDisplay(panel[currentPanelIndex], true);
        SwitchButtons();
    }

    private void SwitchButtons()
    {
        switch (currentPanelIndex)
        {
            case 0:
                SetVisibility(backButton, false);
                SetDisplay(nextButton, true);
                SetDisplay(finishSetupButton, false);
                SetDisplay(openQuickStartButton, false);
                break;
            case 1:
                SetVisibility(backButton, true);
                SetDisplay(nextButton, true);
                SetDisplay(finishSetupButton, false);
                SetDisplay(openQuickStartButton, false);
                break;
            case 2:
                SetVisibility(backButton, true);
                SetDisplay(nextButton, false);
                SetDisplay(finishSetupButton, true);
                SetDisplay(openQuickStartButton, true);
                break;
        }
    }

    private void OnOpenQuickStartButton()
    {
        if (!new QuickStartHelper().Open())
        {
            EditorUtility.DisplayDialog(SETUP_GUIDE, "No quick start sample found.", "OK");
        }
    }


    private void StartStateMachine()
    {
        panel[currentPanelIndex].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
    }


    private void SetVisibility(VisualElement visualElement, bool enable)
    {
        visualElement.style.visibility = new StyleEnum<Visibility>(enable ? Visibility.Visible : Visibility.Hidden);
    }

    private void SetDisplay(VisualElement visualElement, bool enable)
    {
        visualElement.style.display = new StyleEnum<DisplayStyle>(enable ? DisplayStyle.Flex : DisplayStyle.None);
    }
}
