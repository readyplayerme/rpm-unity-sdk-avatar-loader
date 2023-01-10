using System;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This structure holds information about the avatar that is retrieved from the Url.
    /// </summary>
    public struct AvatarMetadata
    {
        public BodyType BodyType;
        public OutfitGender OutfitGender;
        public DateTime LastModified;
        public bool IsUpdated;
    }
}
