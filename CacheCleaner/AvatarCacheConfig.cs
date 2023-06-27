using System;

namespace ReadyPlayerMe.AvatarLoader
{
    [Serializable]
    public enum LimitStrategy
    {
        AvatarCount,
        CacheSize
    }

    [Serializable]
    public struct AvatarCacheConfig
    {
        public LimitStrategy limitStrategy;
        public int avatarCountLimit;
        public float avatarCacheSizeLimit;
    }
}
