using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public static class LocalTestAssets
    {
        public const string MULTI_MESH_MALE_AVATAR_GLB_PROJECT_PATH =
            "Assets/Tests/Common/Models/fullbody-male-multi-mesh-file.glb";

        public const string MULTI_MESH_MALE_PROCESSED_AVATAR_PATH =
            "Assets/Tests/Common/Models/fullbody-male-multi-mesh-avatar.prefab";

        public const string SINGLE_MESH_FEMALE_PROCESSED_AVATAR_PATH =
            "Assets/Tests/Common/Models/fullbody-female-single-mesh-avatar.prefab";

        public const string MOCK_PREFAB_RELATIVE_SAVE_PATH = "Assets/Tests/Common/test.prefab";

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
    }
}