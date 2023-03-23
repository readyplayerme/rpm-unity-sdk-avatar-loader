

namespace ReadyPlayerMe.AvatarLoader
{
    public static class RenderParameterProcessor
    {
        public static string BuildRenderUrl(string avatarUrl, AvatarRenderSettings avatarRenderSettings)
        {
            var imageUrl = avatarUrl.Replace(".glb", ".png");
            return $"{imageUrl}{GetParametersFromSettings(avatarRenderSettings)}";
        }

        public static string GetParametersFromSettings(AvatarRenderSettings avatarRenderSettings)
        {
            var parameters = "?";
            parameters += $"&scene={avatarRenderSettings.Scene.ToString()}";
            parameters += $"&blendShapes={avatarRenderSettings.Scene.ToString()}";
            return parameters;
        }
    }
}
