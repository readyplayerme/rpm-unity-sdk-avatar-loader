using System.IO;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public static class TestUtils
    {
        public const string GLB_SUFFIX = ".glb";
        public const string JSON_SUFFIX = ".json";
        
        public const string MODELS_URL_PREFIX = "https://models.readyplayer.me/";
        public const string API_URL_PREFIX = "https://api.readyplayer.me/v1/avatars/";
        public const string TEST_WRONG_GUID = "wrong-guid";

        public const string MULTI_MESH_MALE_AVATAR_GLB_PROJECT_PATH =
            "Assets/Tests/Common/Models/fullbody-male-multi-mesh-file.glb";

        public const string MULTI_MESH_MALE_PROCESSED_AVATAR_PATH =
            "Assets/Tests/Common/Models/fullbody-male-multi-mesh-avatar.prefab";

        public const string SINGLE_MESH_FEMALE_PROCESSED_AVATAR_PATH =
            "Assets/Tests/Common/Models/fullbody-female-single-mesh-avatar.prefab";

        public const string MOCK_PREFAB_RELATIVE_SAVE_PATH = "Assets/Tests/Common/test.prefab";
        
        public static readonly string TestAvatarDirectory = $"{Application.persistentDataPath}/Ready Player Me/Test/Avatars";

        public static readonly string TestJsonFilePath =
            $"{DirectoryUtility.GetAvatarSaveDirectory(TestAvatarData.DefaultAvatarUri.Guid)}/test.json";


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
        
        public static void DeleteAvatarDirectoryIfExists(string avatarGuid, bool recursive = false)
        {
            var path = $"{TestAvatarDirectory}/{avatarGuid}";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive);
            }
        }
    }
}
