using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core
{
    [CreateAssetMenu(fileName = "Avatar Loader Settings", menuName = "Scriptable Objects/Ready Player Me/Avatar Loader Settings", order = 1)]
    public class AvatarLoaderSettings : ScriptableObject
    {
        public bool AvatarCachingEnabled;
        public AvatarConfig AvatarConfig;
        public const string SETTINGS_PATH = "Settings/AvatarLoaderSettings";

        public static AvatarLoaderSettings LoadSettings()
        {
#if DISABLE_AUTO_INSTALLER && UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<AvatarLoaderSettings>($"Assets/Ready Player Me/Core/Settings/AvatarLoaderSettings.asset");
#else
            return Resources.Load<AvatarLoaderSettings>(SETTINGS_PATH);
#endif
        }
    }
}
