﻿using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Editor;
using ReadyPlayerMe.Core.Analytics;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class SettingsEditorWindow : EditorWindowBase
    {
        private const string WEB_VIEW_PARTNER_SAVE_KEY = "WebViewPartnerSubdomainName";
        private const string SETTINGS_HEADING = "Partner Settings";
        private const string HELP_TEXT =
            "If you are a Ready Player Me partner, please enter your subdomain here to apply your configuration to the WebView.";
        private const string OTHER_SECTION_HEADING = "Other";
        private const string ANALYTICS_LOGGING_DESCRIPTION =
            "We are constantly adding new features and improvements to our SDK. Enable analytics and help us in building even better free tools for more developers. This data is used for internal purposes only and is not shared with third parties.";
        private const string ANALYTICS_PRIVACY_TOOLTIP = "Click to read our Privacy Policy.";
        private const string LOGGING_ENABLED_TOOLTIP = "Enable for detailed console logging of RPM Unity SDK at Runtime and in Editor.";
        private const string AVATAR_CONFIG_TOOLTIP = "Assign an avatar configuration to include Avatar API request parameters.";
        private const string DEFER_AGENT_TOOLTIP = "Assign a defer agent which decides how the glTF will be loaded.";
        private const string ANALYTICS_PRIVACY_URL =
            "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/help-us-improve-the-unity-sdk";
        private const string CACHING_TOOLTIP =
            "Enable caching to improve avatar loading performance at runtime.";
        private const string EDITOR_WINDOW_NAME = "rpm settings";

#if UNITY_EDITOR_LINUX
        private const string SHOW_CACHING_FOLDER_BUTTON_TEXT = "Show in file manager";
#elif UNITY_EDITOR_OSX
        private const string SHOW_CACHING_FOLDER_BUTTON_TEXT = "Reveal in finder";
#else
        private const string SHOW_CACHING_FOLDER_BUTTON_TEXT = "Show in explorer";
#endif

        private const string DOMAIN_VALIDATION_ERROR = "Please enter a valid partner subdomain (e.g. demo). Click here to read more about this issue.";

        private string partnerSubdomain = string.Empty;
        private bool initialized;
        private bool analyticsEnabled;
        private bool avatarCachingEnabled;
        private bool sdkLoggingEnabled;

        private bool isCacheEmpty;
        private AvatarLoaderSettings avatarLoaderSettings;

        private readonly GUILayoutOption inputFieldWidth = GUILayout.Width(128);
        private readonly GUILayoutOption objectFieldWidth = GUILayout.Width(318);

        private GUIStyle textFieldStyle;
        private GUIStyle textLabelStyle;
        private GUIStyle saveButtonStyle;
        private GUIStyle partnerButtonStyle;
        private GUIStyle avatarCachingButtonStyle;
        private GUIStyle privacyPolicyStyle;
        private GUIStyle errorButtonStyle;

        private AvatarConfig avatarConfig;
        private GLTFDeferAgent gltfDeferAgent;

        private bool subdomainFocused;
        private string subdomainAfterFocus = string.Empty;
        private const string SUBDOMAIN_FIELD_CONTROL_NAME = "subdomain";
        private const string SUBDOMAIN_DOCS_LINK = "https://docs.readyplayer.me/ready-player-me/for-partners/partner-subdomains";

        [MenuItem("Ready Player Me/Settings", priority = 1)]
        public static void ShowWindowMenu()
        {
            var window = (SettingsEditorWindow) GetWindow(typeof(SettingsEditorWindow));
            window.titleContent = new GUIContent("Settings");
            window.ShowUtility();

            AnalyticsEditorLogger.EventLogger.LogOpenDialog(EDITOR_WINDOW_NAME);
        }

        private void Initialize()
        {
            SetEditorWindowName(EDITOR_WINDOW_NAME);

            partnerSubdomain = CoreSettings.PartnerSubdomainSettings.Subdomain ?? "demo";
            SaveSubdomain();

            analyticsEnabled = AnalyticsEditorLogger.IsEnabled;
            avatarLoaderSettings = AvatarLoaderSettings.LoadSettings();

            avatarCachingEnabled = avatarLoaderSettings != null && avatarLoaderSettings.AvatarCachingEnabled;
            isCacheEmpty = AvatarCache.IsCacheEmpty();
            if (avatarLoaderSettings != null)
            {
                avatarConfig = avatarLoaderSettings.AvatarConfig;
                gltfDeferAgent = avatarLoaderSettings.GLTFDeferAgent;
            }

            initialized = true;
            sdkLoggingEnabled = SDKLogger.GetEnabledPref();
        }

        private void OnFocus()
        {
            isCacheEmpty = AvatarCache.IsCacheEmpty();
        }

        private void OnGUI()
        {
            if (!initialized) Initialize();
            LoadStyles();
            DrawContent(DrawContent);
        }

        private void LoadStyles()
        {
            saveButtonStyle ??= new GUIStyle(GUI.skin.button);
            saveButtonStyle.fontSize = 14;
            saveButtonStyle.fontStyle = FontStyle.Bold;
            saveButtonStyle.fixedWidth = 449;
            saveButtonStyle.fixedHeight = ButtonHeight;
            saveButtonStyle.padding = new RectOffset(5, 5, 5, 5);
            
            textFieldStyle ??= new GUIStyle(GUI.skin.textField);
            textFieldStyle.fontSize = 12;

            textLabelStyle ??= new GUIStyle(GUI.skin.label);
            textLabelStyle.fontStyle = FontStyle.Bold;
            textLabelStyle.fontSize = 12;
            
            partnerButtonStyle ??= new GUIStyle(GUI.skin.button);
            partnerButtonStyle.fontSize = 12;
            partnerButtonStyle.padding = new RectOffset(5, 5, 5, 5);
            
            avatarCachingButtonStyle ??= new GUIStyle(GUI.skin.button);
            avatarCachingButtonStyle.fontStyle = FontStyle.Bold;
            avatarCachingButtonStyle.fontSize = 12;
            avatarCachingButtonStyle.padding = new RectOffset(5, 5, 5, 5);
            avatarCachingButtonStyle.fixedHeight = ButtonHeight;
            avatarCachingButtonStyle.fixedWidth = 225;
            
            privacyPolicyStyle ??= new GUIStyle(GUI.skin.label);
            privacyPolicyStyle.fontStyle = FontStyle.Bold;
            privacyPolicyStyle.fontSize = 12;
            privacyPolicyStyle.fixedWidth = 100;
            
            errorButtonStyle ??= new GUIStyle();
            errorButtonStyle.fixedWidth = 20;
            errorButtonStyle.fixedHeight = 20;
            errorButtonStyle.margin = new RectOffset(2, 0, 2, 2);
        }

        private void DrawContent()
        {
            Vertical(() =>
            {
                DrawPartnerSettings();
                DrawAvatarSettings();
                DrawAvatarCaching();
                DrawOtherSection();
            });
        }

        private void DrawPartnerSettings()
        {
            Vertical(() =>
            {
                GUILayout.Label(new GUIContent(SETTINGS_HEADING, HELP_TEXT), HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);

                    EditorGUILayout.LabelField("Your subdomain:          https:// ", textLabelStyle, GUILayout.Width(176));
                    var oldValue = partnerSubdomain;
                    GUI.SetNextControlName(SUBDOMAIN_FIELD_CONTROL_NAME);
                    partnerSubdomain = EditorGUILayout.TextField(oldValue, textFieldStyle, GUILayout.Width(128), GUILayout.Height(20));

                    EditorGUILayout.LabelField(".readyplayer.me", textLabelStyle, GUILayout.Width(116), GUILayout.Height(20));
                    GUIContent button = new GUIContent(errorIcon, DOMAIN_VALIDATION_ERROR);

                    var isSubdomainValid = ValidateSubdomain();

                    if (!isSubdomainValid)
                    {
                        if (GUILayout.Button(button, errorButtonStyle))
                        {
                            Application.OpenURL(SUBDOMAIN_DOCS_LINK);
                        }

                        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
                    }

                    if (IsSubdomainFocusLost())
                    {
                        SaveSubdomain();
                    }
                });
            }, true);
        }

        private void DrawAvatarSettings()
        {
            Vertical(() =>
            {
                GUILayout.Label(new GUIContent("Avatar Settings"), HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);
                    EditorGUILayout.LabelField(new GUIContent("Avatar Config", AVATAR_CONFIG_TOOLTIP), inputFieldWidth);
                    avatarConfig = EditorGUILayout.ObjectField(avatarConfig, typeof(AvatarConfig), false, objectFieldWidth) as AvatarConfig;
                    if (avatarLoaderSettings != null && avatarLoaderSettings.AvatarConfig != avatarConfig)
                    {
                        avatarLoaderSettings.AvatarConfig = avatarConfig;
                        SaveAvatarLoaderSettings();
                    }
                });

                Horizontal(() =>
                {
                    GUILayout.Space(2);
                    EditorGUILayout.LabelField(new GUIContent("GLTF defer agent", DEFER_AGENT_TOOLTIP), inputFieldWidth);
                    gltfDeferAgent = EditorGUILayout.ObjectField(gltfDeferAgent, typeof(GLTFDeferAgent), false, objectFieldWidth) as GLTFDeferAgent;
                    if (avatarLoaderSettings != null && avatarLoaderSettings.GLTFDeferAgent != gltfDeferAgent)
                    {
                        avatarLoaderSettings.GLTFDeferAgent = gltfDeferAgent;
                        SaveAvatarLoaderSettings();
                    }
                });
            }, true);
        }

        private void DrawAvatarCaching()
        {
            Vertical(() =>
            {
                GUILayout.Label("Avatar Caching", HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);
                    var cachingEnabled = avatarCachingEnabled;
                    avatarCachingEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Avatar caching enabled", CACHING_TOOLTIP), avatarCachingEnabled);

                    if (cachingEnabled != avatarCachingEnabled && avatarLoaderSettings != null)
                    {
                        avatarLoaderSettings.AvatarCachingEnabled = avatarCachingEnabled;
                        SaveAvatarLoaderSettings();
                    }
                });

                GUILayout.Space(4);

                Horizontal(() =>
                {
                    GUI.enabled = !isCacheEmpty;
                    if (GUILayout.Button("Clear local avatar cache", avatarCachingButtonStyle))
                    {
                        TryClearCache();
                        isCacheEmpty = AvatarCache.IsCacheEmpty();
                    }
                    GUI.enabled = true;

                    if (GUILayout.Button(SHOW_CACHING_FOLDER_BUTTON_TEXT, avatarCachingButtonStyle))
                    {
                        var path = DirectoryUtility.GetAvatarsDirectoryPath();
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        EditorUtility.RevealInFinder(path);
                    }
                });
            }, true);
        }

        private void DrawOtherSection()
        {
            Vertical(() =>
            {
                GUILayout.Label(OTHER_SECTION_HEADING, HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);
                    analyticsEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Analytics enabled", ANALYTICS_LOGGING_DESCRIPTION), analyticsEnabled, GUILayout.Width(125));

                    if (GUILayout.Button(new GUIContent("(Privacy Policy)", ANALYTICS_PRIVACY_TOOLTIP), privacyPolicyStyle))
                    {
                        Application.OpenURL(ANALYTICS_PRIVACY_URL);
                    }

                    if (AnalyticsEditorLogger.IsEnabled != analyticsEnabled)
                    {
                        if (analyticsEnabled)
                        {
                            AnalyticsEditorLogger.Enable();
                        }
                        else
                        {
                            AnalyticsEditorLogger.Disable();
                        }
                    }
                });
                Horizontal(() =>
                {
                    GUILayout.Space(2);
                    sdkLoggingEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Logging enabled", LOGGING_ENABLED_TOOLTIP), sdkLoggingEnabled, GUILayout.Width(125));
                    if (sdkLoggingEnabled != SDKLogger.GetEnabledPref())
                    {
                        SDKLogger.SetEnabledPref(sdkLoggingEnabled);
                    }
                });
            }, true);
        }

        private void SaveSubdomain()
        {
            EditorPrefs.SetString(WEB_VIEW_PARTNER_SAVE_KEY, partnerSubdomain);
            var subDomain = CoreSettings.PartnerSubdomainSettings.Subdomain;
            if (subDomain != partnerSubdomain)
            {
                AnalyticsEditorLogger.EventLogger.LogUpdatePartnerURL(subDomain, partnerSubdomain);
            }

            CoreSettings.SaveSubDomain(partnerSubdomain);
        }

        private bool IsSubdomainFocusLost()
        {
            // focus changed from subdomain to another item
            if (GUI.GetNameOfFocusedControl() == string.Empty && subdomainFocused)
            {
                subdomainFocused = false;

                if (subdomainAfterFocus != partnerSubdomain)
                {
                    return true;
                }
            }
            if (GUI.GetNameOfFocusedControl() == SUBDOMAIN_FIELD_CONTROL_NAME && !subdomainFocused)
            {
                subdomainFocused = true;
                subdomainAfterFocus = partnerSubdomain;
            }

            return false;
        }

        private bool ValidateSubdomain()
        {
            if (partnerSubdomain == null)
            {
                partnerSubdomain = "demo";
            }
            return !partnerSubdomain.All(char.IsWhiteSpace) && !partnerSubdomain.Contains('/') && !EditorUtilities.IsUrlShortcodeValid(partnerSubdomain);
        }

        private static void TryClearCache()
        {
            if (AvatarCache.IsCacheEmpty())
            {
                EditorUtility.DisplayDialog("Clear Cache", "Cache is already empty", "OK");
                return;
            }
            var size = (AvatarCache.GetCacheSize() / (1024f * 1024)).ToString("F2");
            var avatarCount = AvatarCache.GetAvatarCount();
            if (EditorUtility.DisplayDialog("Clear Cache", $"Do you want to clear all the Avatars cache from persistent data path, {size} MB and {avatarCount} avatars?", "Ok", "Cancel"))
            {
                AvatarCache.Clear();
            }
        }

        private void SaveAvatarLoaderSettings()
        {
            EditorUtility.SetDirty(avatarLoaderSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
