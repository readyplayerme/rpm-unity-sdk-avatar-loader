using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class EditorUtilitiesTests
    {
        [TearDown]
        public void Cleanup()
        {
            AssetDatabase.DeleteAsset(TestUtils.MOCK_PREFAB_RELATIVE_SAVE_PATH);
        }

        [UnityTest, Order(0)]
        public IEnumerator Avatar_Loaded_And_Saved_In_Prefab()
        {
            var testObject = new GameObject("Test");
            EditorUtilities.CreatePrefab(testObject, TestUtils.MOCK_PREFAB_RELATIVE_SAVE_PATH);

            yield return null;

            var prefabExists = File.Exists(TestUtils.MockPrefabSavePath);

            Assert.IsTrue(prefabExists);
            Assert.AreEqual(true, PrefabUtility.IsPartOfAnyPrefab(testObject));
        }

        [UnityTest, Order(1)]
        public IEnumerator Avatar_Prefab_Has_Mesh_And_Material()
        {
            var testGlb = AssetDatabase.LoadAssetAtPath<GameObject>(TestUtils.MULTI_MESH_MALE_AVATAR_GLB_PROJECT_PATH);
            var testObject = Object.Instantiate(testGlb);
            EditorUtilities.CreatePrefab(testObject, TestUtils.MOCK_PREFAB_RELATIVE_SAVE_PATH);

            yield return null;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TestUtils.MOCK_PREFAB_RELATIVE_SAVE_PATH);

            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
            var isAnyMaterialNull = renderers.Any(x => x.sharedMaterials.Length == 0);
            Assert.IsFalse(isAnyMaterialNull);

            MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>();
            var isAnyMeshNull = meshFilters.Any(x => x.mesh == null);
            Assert.IsFalse(isAnyMeshNull);
        }
    }
}
