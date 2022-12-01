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

        private static string GetTargetShaderPath()
        {
            switch (GetCurrentRenderPipeline())
            {
                case RenderPipeline.URP:
                    return URP_SHADERS;
                case RenderPipeline.HDRP:
                    return HDRP_SHADERS;
                case RenderPipeline.Standard:
                default:
                    return STANDARD_SHADERS;
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