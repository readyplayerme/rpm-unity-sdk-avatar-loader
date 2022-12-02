using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using ReadyPlayerMe.Core.Editor;
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
        private const string URP_TYPE_NAME = "UniversalRenderPipelineAsset";
        private const string SHADER_SESSION_CHECK = "SHADER_SESSION_CHECK";

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            if (SessionState.GetBool(SHADER_SESSION_CHECK, false)) return;
            SessionState.SetBool(SHADER_SESSION_CHECK, true);
            
            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted)
                Thread.Sleep(100);
            EditorApplication.update += CheckAndUpdatePreloadShaders;
        }
        
        private static void CheckAndUpdatePreloadShaders()
        {
            EditorApplication.update -= CheckAndUpdatePreloadShaders;
            if (IsMissingVariants())
            {
                AddRemovePreloadShaders();
            }
        }

        public static void AddRemovePreloadShaders()
        {
            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);
            var shaderPreloadArray = serializedGraphicsObject.FindProperty(INCLUDE_SHADER_PROPERTY);

            AssetDatabase.Refresh();
            var newPreloadArray = CreateNewPreloadedArray(shaderPreloadArray);
            shaderPreloadArray.ClearArray();

            for (int i = 0; i < newPreloadArray.Length; i++)
            {
                shaderPreloadArray.InsertArrayElementAtIndex(i);
                SerializedProperty shaderInArray = shaderPreloadArray.GetArrayElementAtIndex(i);
                shaderInArray.objectReferenceValue = newPreloadArray[i];
            }
      
            serializedGraphicsObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
        
        
        public static bool IsMissingVariants()
        {
            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);

            var shaderPreloadArray = serializedGraphicsObject.FindProperty(INCLUDE_SHADER_PROPERTY);
            
            var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(GetTargetShaderPath());
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

        private static Object[] CreateNewPreloadedArray(SerializedProperty shaderPreloadArray)
        {
            var collectionsToRemove = GetShaderCollectionsToRemove();
            var index = 0;
            var newPreloadList = new List<Object>();
            foreach (SerializedProperty shaderInclude in shaderPreloadArray)
            {
                if (shaderInclude.objectReferenceValue == null || collectionsToRemove.Contains(shaderInclude.objectReferenceValue.name))
                {
                    if (shaderInclude.objectReferenceValue == null)
                    {
                        Debug.Log($"Removing Empty Preloaded Shader Slot");
                    }
                    else
                    {
                        Debug.Log($"Removing Shader Variant: {shaderInclude.objectReferenceValue.name}");
                    }
                    continue;
                }
                newPreloadList.Add(shaderInclude.objectReferenceValue);
                index++;
            }
            
            var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(GetTargetShaderPath());

            if (shaderVariants == null)
            {
                Debug.Log($"Shader variants Path is null: {GetTargetShaderPath()}");
            }
            if (!newPreloadList.Contains(shaderVariants))
            {
                newPreloadList.Add(shaderVariants);
            }
            return newPreloadList.ToArray();
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
                if (renderPipelineType.Contains(URP_TYPE_NAME))
                {
                    return RenderPipeline.URP;
                }
            }
            return RenderPipeline.Standard;
        }
    }
}