using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    [CreateAssetMenu(fileName = "Avatar Loader Settings", menuName = "Scriptable Objects/Ready Player Me/Avatar Loader Settings", order = 1)]
    public class AvatarLoaderSettings : ScriptableObject
    {
        public bool AvatarCachingEnabled;
        public AvatarConfig AvatarConfig;
        public const string SETTINGS_PATH = "Settings/AvatarLoaderSettings";

        public static AvatarLoaderSettings LoadSettings()
        {
            return Resources.Load<AvatarLoaderSettings>(SETTINGS_PATH);
        }
    }
}
