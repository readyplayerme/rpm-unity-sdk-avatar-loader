using ReadyPlayerMe.AvatarLoader.Editor;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Analytics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using ObjectField = UnityEditor.UIElements.ObjectField;

public class SetupGuide : EditorWindow
{
    private const string SETUP_GUIDE = "SetupGuide";
    private const string STUDIO_URL = "https://studio.readyplayer.me";
    private const string ANALYTICS_PRIVACY_URL = "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/help-us-improve-the-unity-sdk";
    private const string SUBDOMAIN_PANEL = "SubdomainPanel";
    private const string STUDIO_URL_LABEL = "StudioUrl";
    private const string SUBDOMAIN_FIELD = "SubdomainField";
    private const string USE_DEMO_SUBDOMAIN_TOGGLE = "UseDemoSubdomainToggle";
    private const string DEMO = "demo";
    private const string AVATAR_CONFIG_PANEL = "AvatarConfigPanel";
    private const string AVATAR_CONFIG_FIELD = "AvatarConfigField";
    private const string ANALYTICS_PANEL = "AnalyticsPanel";
    private const string ANALYTICS_ENABLED_TOGGLE = "AnalyticsEnabledToggle";
    private const string NEXT_BUTTON = "NextButton";
    private const string BACK_BUTTON = "BackButton";
    private const string FINISH_SETUP_BUTTON = "FinishSetupButton";
    private const string OPEN_QUICK_START_BUTTON = "OpenQuickStartButton";

    [SerializeField] private VisualTreeAsset visualTreeAsset;

    private VisualElement[] panel;
    private VisualElement currentPanel;
    private int currentPanelIndex;

    private ObjectField avatarConfigField;

    private Button backButton;
    private Button nextButton;
    private Button finishSetupButton;
    private Button openQuickStartButton;

    [MenuItem("Ready Player Me/Re-run Setup")]
    public static void ShowWindow()
    {
        var window = GetWindow<SetupGuide>();
        window.titleContent = new GUIContent(SETUP_GUIDE);
        window.minSize = new Vector2(500, 400);
    }

    public void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);
        panel = new[]
        {
            InitializeSubdomainPanel(),
            InitializedAvatarConfigPanel(),
            InitializeAnalyticsPanel()
        };

        InitializeFooter();
        StartStateMachine();
    }

    private VisualElement InitializeSubdomainPanel()
    {
        var subdomainPanel = rootVisualElement.Q<VisualElement>(SUBDOMAIN_PANEL);
        subdomainPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        subdomainPanel.Q<Label>(STUDIO_URL_LABEL).RegisterCallback<MouseUpEvent>(x =>
        {
            Application.OpenURL(STUDIO_URL);
        });

        var subdomainTemplate = subdomainPanel.Q<SubdomainTemplate>();
        var subdomainField = subdomainTemplate.Q<TextField>(SUBDOMAIN_FIELD);

        var useDemoSubdomainToggle = subdomainPanel.Q<Toggle>(USE_DEMO_SUBDOMAIN_TOGGLE);
        if (CoreSettingsHandler.CoreSettings.Subdomain == DEMO)
        {
            useDemoSubdomainToggle.value = true;
        }

        subdomainPanel.Q<Toggle>(USE_DEMO_SUBDOMAIN_TOGGLE).RegisterValueChangedCallback(x =>
        {
            if (x.newValue)
            {
                subdomainTemplate.SetSubdomain(DEMO);
            }
        });

        subdomainField.RegisterValueChangedCallback(x =>
        {
            useDemoSubdomainToggle.SetValueWithoutNotify(x.newValue == DEMO);
        });

        return subdomainPanel;
    }

    private VisualElement InitializedAvatarConfigPanel()
    {
        var avatarConfigPanel = rootVisualElement.Q<VisualElement>(AVATAR_CONFIG_PANEL);
        avatarConfigPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        avatarConfigField = avatarConfigPanel.Q<ObjectField>(AVATAR_CONFIG_FIELD);
        avatarConfigField.RegisterValueChangedCallback(x =>
        {
            nextButton.SetEnabled(x.newValue != null);
        });

        return avatarConfigPanel;
    }

    private VisualElement InitializeAnalyticsPanel()
    {
        var analyticsPanel = rootVisualElement.Q<VisualElement>(ANALYTICS_PANEL);
        analyticsPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        analyticsPanel.Q<Label>("PrivacyUrl").RegisterCallback<MouseUpEvent>(x =>
        {
            Application.OpenURL(ANALYTICS_PRIVACY_URL);
        });

        var analyticsEnabledToggle = analyticsPanel.Q<Toggle>(ANALYTICS_ENABLED_TOGGLE);
        analyticsEnabledToggle.SetValueWithoutNotify(AnalyticsEditorLogger.IsEnabled);
        analyticsEnabledToggle.RegisterValueChangedCallback(x =>
        {
            if (x.newValue)
            {
                AnalyticsEditorLogger.Enable();
            }
            else
            {
                AnalyticsEditorLogger.Disable();
            }
            ProjectPrefs.SetBool(ProjectPrefs.FIRST_TIME_SETUP_DONE, x.newValue);

        });

        return analyticsPanel;
    }

    private void InitializeFooter()
    {
        nextButton = rootVisualElement.Q<Button>(NEXT_BUTTON);
        nextButton.clicked += NextPanel;

        backButton = rootVisualElement.Q<Button>(BACK_BUTTON);
        backButton.clicked += PreviousPanel;

        finishSetupButton = rootVisualElement.Q<Button>(FINISH_SETUP_BUTTON);
        finishSetupButton.clicked += Close;

        openQuickStartButton = rootVisualElement.Q<Button>(OPEN_QUICK_START_BUTTON);
        openQuickStartButton.clicked += OnOpenQuickStartButton;
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
                nextButton.SetEnabled(avatarConfigField.value != null);
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
        SetDisplay(panel[currentPanelIndex], true);
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
