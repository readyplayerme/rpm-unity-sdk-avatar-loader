using System.Collections.Generic;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This structure holds all the data required a request to the Avatar Render API.
    /// </summary>
    public struct AvatarRenderSettings
    {
        public string Model;
        public AvatarRenderScene Scene;
        public string Armature;
        public string BlendShapeMesh;
        public Dictionary<string, float> BlendShapes;
    }
}
