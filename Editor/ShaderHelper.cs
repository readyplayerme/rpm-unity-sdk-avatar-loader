using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public enum RenderPipeline { Standard, URP, HDRP };
    public static class ShaderHelper
    {
        private const string INCLUDE_SHADER_PROPERTY = "m_PreloadedShaders";
        private const string GRAPHICS_SETTING_PATH = "ProjectSettings/GraphicsSettings.asset";

#if DISABLE_AUTO_INSTALLER
        private const string SHADER_VARIANT_FOLDER = "Assets/Ready Player Me/Avatar Loader/Shaders";
#else
    private const string SHADER_VARIANT_FOLDER = "Packages/com.readyplayerme.avatarloader/Shaders";
#endif

        private const string SHADER_VARIANTS_STANDARD = "glTFastShaderVariants";
        private const string SHADER_VARIANTS_URP = "glTFastShaderVariantsURP";
        private const string SHADER_VARIANTS_HDRP = "glTFastShaderVariantsHDRP";

        private const string HDRP_TYPE_NAME = "HDRenderPipelineAsset";
        
        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted)
                Thread.Sleep(100);
            EditorApplication.update += AddPreloadShaders;
        }
        
        public static void AddPreloadShaders()
        {
            EditorApplication.update -= AddPreloadShaders;

            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);
            var shaderPreloadArray = serializedGraphicsObject.FindProperty(INCLUDE_SHADER_PROPERTY);

            string shaderPath = GetTargetShaderPath();
            
            AssetDatabase.Refresh();
            var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderPath);

            if (shaderVariants == null)
            {
                Debug.Log($"Shader variants Path is null: {shaderPath}");
                return;
            }

            var newArrayIndex = shaderPreloadArray.arraySize;

            RemovedUnusedShaderVariantCollections(shaderPreloadArray);

            if (CheckShaderVariantsMissing(shaderPreloadArray))
            {
                Debug.Log($"Added {shaderVariants.name} to Preloaded Shaders in Graphics Settings ");

                shaderPreloadArray.InsertArrayElementAtIndex(newArrayIndex);
                SerializedProperty shaderInArray = shaderPreloadArray.GetArrayElementAtIndex(newArrayIndex);
                shaderInArray.objectReferenceValue = shaderVariants;
                serializedGraphicsObject.ApplyModifiedProperties();
            }
            AssetDatabase.SaveAssets();
        }
        
        public static bool IsVariantsMissing()
        {
            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);
            return CheckShaderVariantsMissing(serializedGraphicsObject.FindProperty(INCLUDE_SHADER_PROPERTY));
        }

        private static bool CheckShaderVariantsMissing(SerializedProperty shaderPreloadArray)
        {
            var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(GetTargetShaderPath());
            Debug.Log($"shader path = {GetTargetShaderPath()}");
            var shadersMissing = true;
            var serializedVariants = new SerializedObject(shaderVariants);

            foreach (SerializedProperty shaderInclude in shaderPreloadArray)
            {
                if (shaderInclude.objectReferenceValue.name == serializedVariants.targetObject.name)
                {
                    Debug.Log("glTFast shader variants found in Graphics Settings->Preloaded Shaders");
                    shadersMissing = false;
                    break;
                }
            }
            return shadersMissing;
        }
        
        private static void RemovedUnusedShaderVariantCollections(SerializedProperty shaderPreloadArray)
        {
            var collectionsToRemove = GetShaderCollectionsToRemove();
            
            var index = 0;
            var indexToRemoveList = new List<int>();
            foreach (SerializedProperty shaderInclude in shaderPreloadArray)
            {
                if (collectionsToRemove.Contains(shaderInclude.objectReferenceValue.name))
                {
                    indexToRemoveList.Add(index);
                }
                index++;
            }

            foreach (var indexToRemove in indexToRemoveList)
            {
                shaderPreloadArray.DeleteArrayElementAtIndex(indexToRemove);
            }
        }

        private static string[] GetShaderCollectionsToRemove()
        {
            var removeList = new List<string>();
            var currentRenderPipeline = GetCurrentRenderPipeline();
            string[] variantCollections = { SHADER_VARIANTS_STANDARD, SHADER_VARIANTS_URP, SHADER_VARIANTS_HDRP };
            for (int i = 0; i < variantCollections.Length; i++)
            {
                if (i != (int) currentRenderPipeline)
                {
                    removeList.Add(variantCollections[i]);
                }
            }
            return removeList.ToArray();
        }

        private static string GetTargetShaderPath()
        {
            const string shaderVariants = ".shadervariants";
            switch (GetCurrentRenderPipeline())
            {
                case RenderPipeline.URP:
                    
                    return $"{SHADER_VARIANT_FOLDER}/{SHADER_VARIANTS_URP}{shaderVariants}";
                case RenderPipeline.HDRP:
                    return $"{SHADER_VARIANT_FOLDER}/{SHADER_VARIANTS_HDRP}{shaderVariants}";
                case RenderPipeline.Standard:
                default:
                    return $"{SHADER_VARIANT_FOLDER}/{SHADER_VARIANTS_STANDARD}{shaderVariants}";
            }
        }

        private static RenderPipeline GetCurrentRenderPipeline()
        {
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                var renderPipelineType = GraphicsSettings.renderPipelineAsset.GetType().ToString();
                if (renderPipelineType.Contains(HDRP_TYPE_NAME))
                {
                    return RenderPipeline.HDRP;
                }
                return RenderPipeline.URP;
            }
            return RenderPipeline.Standard;
        }
    }
}