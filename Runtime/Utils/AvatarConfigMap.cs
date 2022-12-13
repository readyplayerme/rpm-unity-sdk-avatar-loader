using System.Collections.Generic;

namespace ReadyPlayerMe.Core
{
    public static class AvatarConfigMap
    {
        public static readonly Dictionary<Pose, string> Pose = new Dictionary<Pose, string>
        {
            { Core.Pose.APose, "A" },
            { Core.Pose.TPose, "T" }
        };

        public static readonly Dictionary<TextureAtlas, string> TextureAtlas = new Dictionary<TextureAtlas, string>
        {
            { Core.TextureAtlas.None, "none" },
            { Core.TextureAtlas.High, "1024" },
            { Core.TextureAtlas.Medium, "512" },
            { Core.TextureAtlas.Low, "256" }
        };
    }
}
