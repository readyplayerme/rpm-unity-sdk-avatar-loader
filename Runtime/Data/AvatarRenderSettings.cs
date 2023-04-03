using System.Collections.Generic;
using System.Text;

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

        public string GetParametersAsString()
        {
            var parameters = "?";
            parameters += Scene.GetSceneNameAsString();
            if (BlendShapes.Count > 0)
            {
                parameters += BuildBlendshapeParameters();
            }
            return parameters;
        }

        public string BuildBlendshapeParameters()
        {
            var parameters = new StringBuilder();
            foreach (var blendShape in BlendShapes)
            {
                parameters.Append($"&blendShapes=[{BlendShapeMesh}][{blendShape.Key}]={blendShape.Value}");
            }
            return parameters.ToString();
        }
    }
}
