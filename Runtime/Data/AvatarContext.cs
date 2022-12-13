using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarLoader
{
    public class AvatarContext : Context
    {
        public bool AvatarCachingEnabled;
        public AvatarConfig AvatarConfig;
        public AvatarUri AvatarUri;
        public byte[] Bytes;
        public object Data;
        public AvatarMetadata Metadata;
        public string ParametersHash;
        public AvatarRenderSettings RenderSettings;
        public bool SaveInProjectFolder;
        public string Url;
    }
}
