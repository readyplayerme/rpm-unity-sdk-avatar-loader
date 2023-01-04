using System.IO;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public static class TestUtils
    {
        public const string TEST_AVATAR_GUID = "7f7f0ab3-c639-4e0c-82b1-2134c03d2af4";
        public const string TEST_WRONG_GUID = "wrong-guid";

        public const string MULTI_MESH_MALE_AVATAR_GLB_PROJECT_PATH =
            "Assets/Tests/Common/Models/fullbody-male-multi-mesh-file.glb";

        public const string MULTI_MESH_MALE_PROCESSED_AVATAR_PATH =
            "Assets/Tests/Common/Models/fullbody-male-multi-mesh-avatar.prefab";

        public const string SINGLE_MESH_FEMALE_PROCESSED_AVATAR_PATH =
            "Assets/Tests/Common/Models/fullbody-female-single-mesh-avatar.prefab";

        public const string MOCK_PREFAB_RELATIVE_SAVE_PATH = "Assets/Tests/Common/test.prefab";
        public static readonly string TestAvatarDirectory = $"{Application.persistentDataPath}/Avatars";

        public static readonly string TestJsonFilePath =
            $"{DirectoryUtility.GetAvatarSaveDirectory(TEST_AVATAR_GUID)}/test.json";
        public static readonly AvatarUri Uri = new AvatarUri
        {
            Guid = TEST_AVATAR_GUID,
            ModelUrl = $"https://d1a370nemizbjq.cloudfront.net/{TEST_AVATAR_GUID}.glb",
            LocalModelPath = $"{TestAvatarDirectory}/{TEST_AVATAR_GUID}/{TEST_AVATAR_GUID}.glb",
            MetadataUrl = $"https://d1a370nemizbjq.cloudfront.net/{TEST_AVATAR_GUID}.json",
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

        #region JSON Metadata Variables

        public const string JSON_FEMININE_FULL_BODY =
            "https://d1a370nemizbjq.cloudfront.net/7f7f0ab3-c639-4e0c-82b1-2134c03d2af4.json";

        public const string JSON_MASCULINE_FULL_BODY =
            "https://d1a370nemizbjq.cloudfront.net/fa83d7ac-3fe0-4589-a42e-7b74ea6142e5.json";

        public const string JSON_FEMININE_HALF_BODY =
            "https://d1a370nemizbjq.cloudfront.net/419f78a1-f9d4-4695-9cc9-4537a6b2f671.json";

        public const string JSON_MASCULINE_HALF_BODY =
            "https://d1a370nemizbjq.cloudfront.net/b4082a25-1529-4160-b256-b9595fa7f269.json";

        public const string WRONG_JSON_URL =
            "https://gist.githubusercontent.com/srcnalt/2ca44ce804ac28ce8722a93dca3635c9/raw";

        #endregion

        #region Avatar Uri Variables

        #endregion

        #region Avatar Render API Variables

        public const AvatarRenderScene RENDER_SCENE = AvatarRenderScene.PortraitTransparent;
        public const string RENDER_BLENDSHAPE_MESH = "Wolf3D_Head";
        public const string RENDER_WRONG_BLENDSHAPE_MESH = "wrong_blendshape_mesh";
        public const string RENDER_BLENDSHAPE = "mouthSmile";
        public const string RENDER_WRONG_BLENDSHAPE = "wrong_blendshape";

        #endregion

        #region Avatar Loader Avatar API Variables

        public const string AVATAR_API_AVATAR_URL = "https://api.readyplayer.me/v1/avatars/638df693d72bffc6fa17943c.glb";
        public const string AVATAR_CONFIG_PATH_LOW = "Avatar Config Low";
        public const string AVATAR_CONFIG_PATH_MED = "Avatar Config Medium";
        public const string AVATAR_CONFIG_PATH_HIGH = "Avatar Config High";
        public const int TEXTURE_SIZE_LOW = 256;
        public const int TEXTURE_SIZE_MED = 512;
        public const int TEXTURE_SIZE_HIGH = 1024;
        public const int AVATAR_CONFIG_BLEND_SHAPE_COUNT_MED = 15;

        #endregion
    }
}
