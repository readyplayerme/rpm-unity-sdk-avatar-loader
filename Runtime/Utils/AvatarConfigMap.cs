using System.Collections.Generic;

namespace ReadyPlayerMe.AvatarLoader
{
    public static class AvatarConfigMap
    {
        public static readonly Dictionary<Pose, string> Pose = new Dictionary<Pose, string>
        {
            { AvatarLoader.Pose.APose, "A" },
            { AvatarLoader.Pose.TPose, "T" }
        };

        public static readonly Dictionary<TextureAtlas, string> TextureAtlas = new Dictionary<TextureAtlas, string>
        {
            { AvatarLoader.TextureAtlas.None, "none" },
            { AvatarLoader.TextureAtlas.High, "1024" },
            { AvatarLoader.TextureAtlas.Medium, "512" },
            { AvatarLoader.TextureAtlas.Low, "256" }
        };
    }
}
