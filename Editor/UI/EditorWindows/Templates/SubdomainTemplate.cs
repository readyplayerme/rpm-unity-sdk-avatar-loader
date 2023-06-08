using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class SubdomainTemplate : VisualElement
    {
        private const string XML_PATH = "SubdomainTemplate";
        private const string DOMAIN_VALIDATION_ERROR = "Please enter a valid partner subdomain (e.g. demo). Click here to read more about this issue.";
        private const string PARTNERS_DOCS_LINK = "https://docs.readyplayer.me/ready-player-me/for-partners/partner-subdomains";
        private const string QUICKSTART_DOCS_LINK = "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/quickstart#before-you-begin";
        private const string WEB_VIEW_PARTNER_SAVE_KEY = "WebViewPartnerSubdomainName";

        public new class UxmlFactory : UxmlFactory<SubdomainTemplate, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private string partnerSubdomain;

        public SubdomainTemplate()
        {
            var visualTree = Resources.Load<VisualTreeAsset>(XML_PATH);
            visualTree.CloneTree(this);

            partnerSubdomain = CoreSettingsHandler.CoreSettings.Subdomain;
            var subdomainField = this.Q<TextField>("SubdomainField");
            subdomainField.value = partnerSubdomain;

            var errorIcon = this.Q<VisualElement>("ErrorIcon");
            errorIcon.tooltip = DOMAIN_VALIDATION_ERROR;

            this.Q<Button>("SubdomainHelpButton").clicked += OnSubdomainHelpClicked;

            errorIcon.RegisterCallback<MouseUpEvent>(OnErrorIconClicked);
            
            subdomainField.RegisterValueChangedCallback(OnSubdomainChanged(errorIcon));
            subdomainField.RegisterCallback<FocusOutEvent>(OnSubdomainFocusOut);
        }

        private static void OnSubdomainHelpClicked()
        {
            Application.OpenURL(QUICKSTART_DOCS_LINK);
        }

        private void OnErrorIconClicked(MouseUpEvent _)
        {
            Application.OpenURL(PARTNERS_DOCS_LINK);
        }

        private void OnSubdomainFocusOut(FocusOutEvent _)
        {
            if (ValidateSubdomain())
            {
                SaveSubdomain();
            }
        }

        private EventCallback<ChangeEvent<string>> OnSubdomainChanged(VisualElement errorIcon)
        {
            return changeEvent =>
            {
                partnerSubdomain = changeEvent.newValue;
                errorIcon.visible = !ValidateSubdomain();
            };
        }

        private bool ValidateSubdomain()
        {
            return !partnerSubdomain.All(char.IsWhiteSpace) && !partnerSubdomain.Contains('/') && !EditorUtilities.IsUrlShortcodeValid(partnerSubdomain);
        }

        public void SaveSubdomain()
        {
            EditorPrefs.SetString(WEB_VIEW_PARTNER_SAVE_KEY, partnerSubdomain);
            var subDomain = CoreSettingsHandler.CoreSettings.Subdomain;
            if (subDomain != partnerSubdomain)
            {
                AnalyticsEditorLogger.EventLogger.LogUpdatePartnerURL(subDomain, partnerSubdomain);
            }

            CoreSettingsHandler.SaveSubDomain(partnerSubdomain);
        }
    }
}
