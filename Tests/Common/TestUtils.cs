using System.IO;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public static class TestUtils
    {
        public const string CLOUDFRONT_TEST_AVATAR_GUID = "7f7f0ab3-c639-4e0c-82b1-2134c03d2af4";
        public const string TEST_AVATAR_GUID = "64184ac404207164c85216d6";
        public const string TEST_WRONG_GUID = "wrong-guid";

        public const string MULTI_MESH_MALE_AVATAR_GLB_PROJECT_PATH =
            "Assets/Tests/Common/Models/fullbody-male-multi-mesh-file.glb";

        public const string MULTI_MESH_MALE_PROCESSED_AVATAR_PATH =
            "Assets/Tests/Common/Models/fullbody-male-multi-mesh-avatar.prefab";

        public const string SINGLE_MESH_FEMALE_PROCESSED_AVATAR_PATH =
            "Assets/Tests/Common/Models/fullbody-female-single-mesh-avatar.prefab";

        public const string MOCK_PREFAB_RELATIVE_SAVE_PATH = "Assets/Tests/Common/test.prefab";

        public const string AVATAR_URL_FULL_BODY_MASCULINE = "https://models.readyplayer.me/64184ac404207164c85216d6.glb";
        public const string AVATAR_URL_FULL_BODY_FEMININE = "https://models.readyplayer.me/641975b2398f7e86e696913e.glb";
        public const string AVATAR_URL_HALF_BODY_MASCULINE = "https://models.readyplayer.me/64184ac404207164c85216d6.glb";
        public const string AVATAR_URL_HALF_BODY_FEMININE = "https://models.readyplayer.me/641975b2398f7e86e696913e.glb";

        public static readonly string TestAvatarDirectory = $"{Application.persistentDataPath}/Avatars";

        public static readonly string TestJsonFilePath =
            $"{DirectoryUtility.GetAvatarSaveDirectory(CLOUDFRONT_TEST_AVATAR_GUID)}/test.json";
        public static readonly AvatarUri CloudfrontUri = new AvatarUri
        {
            Guid = CLOUDFRONT_TEST_AVATAR_GUID,
            ModelUrl = $"https://d1a370nemizbjq.cloudfront.net/{CLOUDFRONT_TEST_AVATAR_GUID}.glb",
            LocalModelPath = $"{TestAvatarDirectory}/{CLOUDFRONT_TEST_AVATAR_GUID}/{CLOUDFRONT_TEST_AVATAR_GUID}.glb",
            MetadataUrl = $"https://d1a370nemizbjq.cloudfront.net/{CLOUDFRONT_TEST_AVATAR_GUID}.json",
            LocalMetadataPath = $"{TestAvatarDirectory}/{CLOUDFRONT_TEST_AVATAR_GUID}/{CLOUDFRONT_TEST_AVATAR_GUID}.json"
        };
        
        public static readonly AvatarUri Uri = new AvatarUri
        {
            Guid = CLOUDFRONT_TEST_AVATAR_GUID,
            ModelUrl = $"https://models.readyplayer.me/{TEST_AVATAR_GUID}.glb",
            LocalModelPath = $"{TestAvatarDirectory}/{TEST_AVATAR_GUID}/{TEST_AVATAR_GUID}.glb",
            MetadataUrl = $"https://models.readyplayer.me/{TEST_AVATAR_GUID}.json",
            LocalMetadataPath = $"{TestAvatarDirectory}/{TEST_AVATAR_GUID}/{TEST_AVATAR_GUID}.json"
        };
        
        public static readonly AvatarUri WrongUri = new AvatarUri
        {
            Guid = TEST_WRONG_GUID,
            ModelUrl = $"{TEST_WRONG_GUID}",
            LocalModelPath = $"{TestAvatarDirectory}/{TEST_WRONG_GUID}/{TEST_WRONG_GUID}.glb",
            MetadataUrl = $"https://{TEST_WRONG_GUID}.com/{TEST_WRONG_GUID}.json",
            LocalMetadataPath = $"{TestAvatarDirectory}/{TEST_WRONG_GUID}/{TEST_WRONG_GUID}.json"
        };
        
        public static readonly string MockAvatarGlbWrongPath =
            $"{DirectoryUtility.GetAvatarSaveDirectory(TEST_WRONG_GUID)}/Tests/Common/wrong.glb";

        public static readonly string MultiMeshMaleAvatarGlbPath =
            $"{Application.dataPath}/Tests/Common/Models/fullbody-male-multi-mesh-file.glb";

        public static readonly string SingleMeshFemaleAvatarGlbPath =
            $"{Application.dataPath}/Tests/Common/Models/fullbody-female-single-mesh-file.glb";

        public static readonly string SingleMeshHalfBodyAvatarGlbPath =
            $"{Application.dataPath}/Tests/Common/Models/halfbody-single-mesh-file.glb";

        public static readonly string MultiMeshHalfBodyAvatarGlbPath =
            $"{Application.dataPath}/Tests/Common/Models/halfbody-multi-mesh-file.glb";

        public static readonly string TestAudioClipPath = "Assets/Tests/Common/Voice Handler Test Audio.mp3";
        public static readonly string MockPrefabSavePath =
            $"{Application.dataPath}/Tests/Common/test.prefab";

        public static void DeleteDirectoryIfExists(string path, bool recursive = false)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive);
            }
        }
    }
}
