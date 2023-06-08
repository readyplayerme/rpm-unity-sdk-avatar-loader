using System.Globalization;
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
        public string[] BlendShapeMeshes;
        public Dictionary<string, float> BlendShapes;

        public string GetParametersAsString()
        {
            BlendShapes ??= new Dictionary<string, float>();
            var queryBuilder = new QueryBuilder();
            queryBuilder.AddKeyValue(AvatarAPIParameters.RENDER_SCENE, Scene.GetSceneNameAsString());
            foreach (KeyValuePair<string, float> blendShape in BlendShapes)
            {
                foreach (var blendShapeMesh in BlendShapeMeshes)
                {
                    string key = $"{AvatarAPIParameters.RENDER_BLEND_SHAPES}[{blendShapeMesh}][{blendShape.Key}]";
                    string value = blendShape.Value.ToString(CultureInfo.InvariantCulture);
                    queryBuilder.AddKeyValue(key, value);
                }
            }
            
            return queryBuilder.Query;
        }
    }
}
