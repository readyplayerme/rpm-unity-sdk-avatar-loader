using System.Collections.Generic;

namespace ReadyPlayerMe.Core
{
    public struct AvatarRenderSettings
    {
        public string Model;
        public AvatarRenderScene Scene;
        public string Armature;
        public string BlendShapeMesh;
        public Dictionary<string, float> BlendShapes;
    }
}
