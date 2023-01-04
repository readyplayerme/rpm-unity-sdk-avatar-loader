using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// The <c>AvatarLoaderSettings</c> class is a <c>ScriptableObject</c> that can be used to easily configure the settings that should be used when loading a Ready Player Me avatar.
    [CreateAssetMenu(fileName = "AvatarLoaderSettings", menuName = "Scriptable Objects/Ready Player Me/Avatar Loader Settings", order = 1)]
    public class AvatarLoaderSettings : ScriptableObject
    {
        /// if enabled avatar assets will be stored locally and only downloaded again if the avatar has been updated
        [Tooltip("If enabled avatar assets will be stored locally and only downloaded again if the avatar has been updated.")]
        public bool AvatarCachingEnabled;
        
        /// 
        public AvatarConfig AvatarConfig;
        
        /// path to the AvatarLoaderSettings relative to the Resources folder
        public const string SETTINGS_PATH = "Settings/AvatarLoaderSettings";
        
        /// Loads avatar settings from resource at <c>AvatarLoaderSettings.SETTINGS_PATH</c>.
        public static AvatarLoaderSettings LoadSettings()
        {
            return Resources.Load<AvatarLoaderSettings>(SETTINGS_PATH);
        }
    }
}
