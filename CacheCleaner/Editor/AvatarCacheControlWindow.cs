using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class AvatarCacheControlWindow : EditorWindow
    {
        private AvatarUrlCollection avatarUrlCollection;

        private string avatarCount;
        private string avatarCache;
        private string divider = "|";
        private bool isLoading = false;

        private int currentIndex = 0;

        private bool enableAutoCacheControl;

        private AvatarCacheConfig config;

        private readonly AvatarManifest avatarManifest = new AvatarManifest();

        [MenuItem("Tools/Avatar Cache Control")] // add menu item to access window
        public static void ShowWindow()
        {
            GetWindow<AvatarCacheControlWindow>("Avatar Cache Control");

        }

        private void OnEnable()
        {
            InitializeConfigWithDefaults();
            LoadEditorPrefs();
        }

        private void OnDisable()
        {
            SaveEditorPrefs(config);
        }

        private void Awake()
        {
            avatarManifest.Load();
            UpdateCacheInfo();
        }

        private void OnFocus()
        {
            UpdateCacheInfo();
        }

        private void OnLostFocus()
        {
            UpdateCacheInfo();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical("Box");
            // add title at the top of the window
            EditorGUILayout.LabelField("Avatar Url Collection", EditorStyles.boldLabel);

            GUILayout.Space(5);
            // add a field to assign the ScriptableObject
            avatarUrlCollection = (AvatarUrlCollection) EditorGUILayout.ObjectField(
                "Avatar Url Collection",
                avatarUrlCollection,
                typeof(AvatarUrlCollection),
                false);


            GUILayout.Space(20);

            // add button to load avatars
            EditorGUI.BeginDisabledGroup(isLoading);
            if (GUILayout.Button("Load All Avatars", GUILayout.Height(30)))
            {
                if (avatarUrlCollection != null)
                {
                    LoadAllAvatars();
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Cache Config", EditorStyles.boldLabel);
            GUILayout.Space(5);
            enableAutoCacheControl = EditorGUILayout.Toggle("Auto cache cleanup", enableAutoCacheControl);
            config.limitStrategy = (LimitStrategy) EditorGUILayout.EnumPopup("Cache Limit Type", config.limitStrategy);
            EditorGUI.BeginDisabledGroup(config.limitStrategy != LimitStrategy.AvatarCount);
            config.avatarCountLimit = EditorGUILayout.IntField("Avatar Count Limit", config.avatarCountLimit);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(config.limitStrategy != LimitStrategy.CacheSize);
            config.avatarCacheSizeLimit = EditorGUILayout.FloatField("Cache Size Limit (MB)", config.avatarCacheSizeLimit);
            EditorGUI.EndDisabledGroup();

            // add button to clean cache
            if (GUILayout.Button("Enforce Cache Limit", GUILayout.Height(30)))
            {
                CacheManager.EnforceCacheLimit(config, avatarManifest);
                AssetDatabase.Refresh();
                UpdateCacheInfo();
            }

            GUILayout.Space(20);
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Cache Status", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(avatarCount);
            EditorGUILayout.LabelField(avatarCache);
            GUILayout.EndVertical();
            GUILayout.Space(10);
            // add button to clean cache
            if (GUILayout.Button("Clean Cache", GUILayout.Height(30)))
            {
                AvatarCache.Clear();
                AssetDatabase.Refresh();
                UpdateCacheInfo();
                avatarManifest.Clear();
            }

            DisplayAvatarManifest();
        }

        private void DisplayAvatarManifest()
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Load / Generate Manifest", GUILayout.Height(30)))
            {
                avatarManifest.Load();
            }

            if (GUILayout.Button("Save Manifest", GUILayout.Height(30)))
            {
                avatarManifest.Save();
                AssetDatabase.Refresh();
            }
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Avatar Manifest", EditorStyles.boldLabel);

            if (avatarManifest.AvatarRecords == null || avatarManifest.AvatarRecords.Count == 0)
            {
                EditorGUILayout.LabelField("No avatar records available.");
            }
            else
            {
                foreach (KeyValuePair<string, DateTime> record in avatarManifest.AvatarRecords)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Avatar ID:", GUILayout.Width(80));
                    EditorGUILayout.LabelField(record.Key, GUILayout.Width(200));
                    EditorGUILayout.LabelField("Date:", GUILayout.Width(50));
                    EditorGUILayout.LabelField(record.Value.ToString(), GUILayout.Width(200));
                    EditorGUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        public void LoadAllAvatars()
        {
            if (isLoading) return;
            currentIndex = 0;
            isLoading = true;
            LoadAvatarAtIndex(currentIndex);
        }

        private void LoadAvatarAtIndex(int index)
        {
            if (index >= avatarUrlCollection.urls.Length) return;
            var loader = new AvatarObjectLoader();
            loader.OnCompleted += LoadCompletedHandler;
            loader.SaveInProjectFolder = true;
            loader.LoadAvatar(avatarUrlCollection.urls[index]);
        }

        private void LoadCompletedHandler(object sender, CompletionEventArgs args)
        {
            UpdateCacheInfo();
            AddToManifestAndSave(args.Avatar.name);
            currentIndex++;
            if (currentIndex < avatarUrlCollection.urls.Length)
            {
                LoadAvatarAtIndex(currentIndex);
            }
            else
            {
                isLoading = false;
            }
            if (enableAutoCacheControl && IsCacheFull())
            {
                ReduceNumberOfAvatars();
            }
        }

        private void LoadEditorPrefs()
        {
            var loadedConfig = new AvatarCacheConfig
            {
                limitStrategy = (LimitStrategy) EditorPrefs.GetInt("CacheLimitType", (int) LimitStrategy.AvatarCount),
                avatarCountLimit = EditorPrefs.GetInt("AvatarCountLimit", 10),
                avatarCacheSizeLimit = EditorPrefs.GetFloat("AvatarCacheSizeLimit", 2.5f)
            };
            config = loadedConfig;

            var assetPath = EditorPrefs.GetString("AvatarUrlCollectionAssetPath", "");

            if (!string.IsNullOrEmpty(assetPath))
            {
                avatarUrlCollection = AssetDatabase.LoadAssetAtPath<AvatarUrlCollection>(assetPath);
            }
        }

        private void SaveEditorPrefs(AvatarCacheConfig configToSave)
        {
            EditorPrefs.SetInt("CacheLimitType", (int) configToSave.limitStrategy);
            EditorPrefs.SetInt("AvatarCountLimit", configToSave.avatarCountLimit);
            EditorPrefs.SetFloat("AvatarCacheSizeLimit", configToSave.avatarCacheSizeLimit);

            if (avatarUrlCollection != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(avatarUrlCollection);
                EditorPrefs.SetString("AvatarUrlCollectionAssetPath", assetPath);
            }
        }

        private void InitializeConfigWithDefaults()
        {
            var defaultConfig = new AvatarCacheConfig
            {
                limitStrategy = LimitStrategy.AvatarCount, // Replace with your default CacheLimitType value
                avatarCountLimit = 10,
                avatarCacheSizeLimit = 2.5f
            };
        }

        private bool IsCacheFull()
        {
            switch (config.limitStrategy)
            {
                default:
                case LimitStrategy.AvatarCount:
                    return AvatarCache.GetAvatarCount() > config.avatarCountLimit;
                case LimitStrategy.CacheSize:
                    return AvatarCache.GetCacheSize() > config.avatarCacheSizeLimit;
            }
        }

        private void ReduceNumberOfAvatars()
        {
            var idsOrderedByOldest = avatarManifest.GetIdsByOldestDate();
            for (var i = 0; i < idsOrderedByOldest.Length; i++)
            {
                Debug.Log($"ID = {idsOrderedByOldest[i]} date = {avatarManifest.GetAvatarLastLoadDate(idsOrderedByOldest[i]).ToString()}");
            }

            avatarManifest.RemoveAvatar(idsOrderedByOldest[0]);
            AvatarCache.DeleteAvatarFolder(idsOrderedByOldest[0]);
            avatarManifest.Save();
            UpdateCacheInfo();
            AssetDatabase.Refresh();
        }

        private void UpdateCacheInfo()
        {
            avatarCount = $"Avatar Count: {AvatarCache.GetAvatarCount()}";
            avatarCache = $"Avatar Cache: {CacheManager.GetCacheSizeMb():F2} mb";
        }

        private void AddToManifestAndSave(string guid)
        {
            avatarManifest.AddAvatar(guid);
            avatarManifest.Save();
        }
    }
}
