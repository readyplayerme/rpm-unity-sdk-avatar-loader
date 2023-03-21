namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public static class TestAvatars
    {
        private const string FULLBODY_MASCULINE = "64184ac404207164c85216d6";
        private const string FULLBODY_FEMININE = "641975b2398f7e86e696913e";
        private const string HALFBODY_MASCULINE = "6419862204207164c854045b";
        private const string HALFBODY_FEMININE = "6419864b398f7e86e696aa77";
        
        private const string CLOUDFRONT_FULLBODY_MASCULINE = "fa83d7ac-3fe0-4589-a42e-7b74ea6142e5";
        private const string CLOUDFRONT_FULLBODY_FEMININE = "7f7f0ab3-c639-4e0c-82b1-2134c03d2af4";
        private const string CLOUDFRONT_HALFBODY_MASCULINE = "b4082a25-1529-4160-b256-b9595fa7f269";
        private const string CLOUDFRONT_HALFBODY_FEMININE = "419f78a1-f9d4-4695-9cc9-4537a6b2f671";

        private const string CLOUDFRONT_URL_PREFIX = "https://d1a370nemizbjq.cloudfront.net/";
        private const string MODELS_URL_PREFIX = "https://models.readyplayer.me/";
        private const string API_URL_PREFIX = "https://api.readyplayer.me/v1/avatars/";
        private const string GLB_SUFFIX = ".glb";
        private const string JSON_SUFFIX = ".json";

        public static string GetCloudfrontAvatarUrl(BodyType bodyType, OutfitGender outfitGender)
        {
            var avatarGuid = GetCloudfrontAvatarGuid(bodyType, outfitGender);
            return $"{CLOUDFRONT_URL_PREFIX}{avatarGuid}{GLB_SUFFIX}";
        }
        
        public static string GetCloudfrontAvatarJsonUrl(BodyType bodyType, OutfitGender outfitGender)
        {
            var avatarGuid = GetCloudfrontAvatarGuid(bodyType, outfitGender);
            return $"{CLOUDFRONT_URL_PREFIX}{avatarGuid}{JSON_SUFFIX}";
        }

        private static string GetCloudfrontAvatarGuid(BodyType bodyType, OutfitGender outfitGender)
        {
            if (bodyType == BodyType.HalfBody)
            {
                return outfitGender == OutfitGender.Masculine ? CLOUDFRONT_HALFBODY_MASCULINE : CLOUDFRONT_HALFBODY_FEMININE;
            }
            return outfitGender == OutfitGender.Masculine ? CLOUDFRONT_FULLBODY_MASCULINE : CLOUDFRONT_FULLBODY_FEMININE;
        }
        
        public static string GetApiAvatarUrl(BodyType bodyType, OutfitGender outfitGender)
        {
            var avatarGuid = GetAvatarGuid(bodyType, outfitGender);
            return $"{API_URL_PREFIX}{avatarGuid}{GLB_SUFFIX}";
        }
        
        public static string GetModelsAvatarUrl(BodyType bodyType, OutfitGender outfitGender)
        {
            var avatarGuid = GetAvatarGuid(bodyType, outfitGender);
            return $"{MODELS_URL_PREFIX}{avatarGuid}{GLB_SUFFIX}";
        }
        
        public static string GetApiAvatarJsonUrl(BodyType bodyType, OutfitGender outfitGender)
        {
            var avatarGuid = GetAvatarGuid(bodyType, outfitGender);
            return $"{API_URL_PREFIX}{avatarGuid}{JSON_SUFFIX}";
        }
        
        public static string GetModelsAvatarJsonUrl(BodyType bodyType, OutfitGender outfitGender)
        {
            var avatarGuid = GetAvatarGuid(bodyType, outfitGender);
            return $"{MODELS_URL_PREFIX}{avatarGuid}{JSON_SUFFIX}";
        }

        private static string GetAvatarGuid(BodyType bodyType, OutfitGender outfitGender)
        {
            if (bodyType == BodyType.HalfBody)
            {
                return outfitGender == OutfitGender.Masculine ? HALFBODY_MASCULINE : HALFBODY_FEMININE;
            }
            return outfitGender == OutfitGender.Masculine ? FULLBODY_MASCULINE : FULLBODY_FEMININE;
        }
    }
}