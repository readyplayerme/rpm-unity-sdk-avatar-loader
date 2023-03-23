using System;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This structure holds information about the avatar that is retrieved from the Url.
    /// </summary>
    [Serializable]
    public struct AvatarMetadata
    {
        public BodyType BodyType;
        public OutfitGender OutfitGender;
        public DateTime UpdatedAt;
        public string SkinTone;
    }
}
