using System.Threading;
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
        private const string STANDARD_SHADERS = "Assets/Ready Player Me/Avatar Loader/Shaders/glTFastShaderVariants.shadervariants";
#else
    private const string STANDARD_SHADERS = "Packages/com.readyplayerme.avatarloader/Shaders/glTFastShaderVariants.shadervariants";
#endif

#if DISABLE_AUTO_INSTALLER
        private const string URP_SHADERS = "Assets/Ready Player Me/Avatar Loader/Shaders/glTFastShaderVariantsURP.shadervariants";
#else
    private const string URP_SHADERS = "Packages/com.readyplayerme.avatarloader/Shaders/glTFastShaderVariantsURP.shadervariants";
#endif
        
#if DISABLE_AUTO_INSTALLER
        private const string HDRP_SHADERS = "Assets/Ready Player Me/Avatar Loader/Shaders/glTFastShaderVariantsHDRP.shadervariants";
#else
    private const string HDRP_SHADERS = "Packages/com.readyplayerme.avatarloader/Shaders/glTFastShaderVariantsHDRP.shadervariants";
#endif
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
            Debug.Log("Run add preload shaders");

            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);
            var shaderPreloadArray = serializedGraphicsObject.FindProperty(INCLUDE_SHADER_PROPERTY);

            string shaderPath = GetTargetShaderPath();
            
            AssetDatabase.Refresh();
            var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderPath);

            if (shaderVariants == null)
            {
                Debug.Log($"Shader variants null Path: {shaderPath}");
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