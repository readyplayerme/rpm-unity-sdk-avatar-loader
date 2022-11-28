﻿using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class ShaderHelper 
{
    private const string INCLUDE_SHADER_PROPERTY = "m_PreloadedShaders";
    private const string GRAPHICS_SETTING_PATH = "ProjectSettings/GraphicsSettings.asset";
#if DISABLE_AUTO_INSTALLER
    private const string STANDARD_SHADERS = "Assets/Ready Player Me/Avatar Loader/Shaders/DefaultglTFastShaders.shadervariants";
#else
    private const string STANDARD_SHADERS = "Packages/com.readyplayerme.avatarloader/Shaders/DefaultglTFastShaders.shadervariants";
#endif
        
#if DISABLE_AUTO_INSTALLER
    private const string URP_SHADERS = "Assets/Ready Player Me/Avatar Loader/Shaders/DefaultglTFastShadersURP.shadervariants";
#else
    private const string URP_SHADERS = "Packages/com.readyplayerme.avatarloader/Shaders/DefaultglTFastShadersURP.shadervariants";
#endif

    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        Debug.Log("SHADER HELPER INIT");
        ModuleInstaller.ModuleInstallComplete += AddPreloadShaders;
    }
    
    public static void AddPreloadShaders()
    {
        Debug.Log("Run add preload shaders");
        var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
        var serializedGraphicsObject = new SerializedObject(graphicsSettings);

        SerializedProperty shaderIncludeArray = serializedGraphicsObject.FindProperty(INCLUDE_SHADER_PROPERTY);
        if (shaderIncludeArray == null)
            return;

        var renderPipelineAsset = GraphicsSettings.defaultRenderPipeline;
        string shaderPath = renderPipelineAsset == null ? STANDARD_SHADERS :  URP_SHADERS;
        AssetDatabase.Refresh();
        var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderPath);

        if (shaderVariants == null)
        {
            Debug.Log($"Shader variants null Path: {shaderPath}");
            serializedGraphicsObject.Dispose();
            return;
        }

        var newArrayIndex = shaderIncludeArray.arraySize;
        var shadersIncluded = false;
        var serializedVariants = new SerializedObject(shaderVariants);
        
        foreach (SerializedProperty shaderInclude in shaderIncludeArray)
        {
            if (shaderInclude.objectReferenceValue.name == serializedVariants.targetObject.name)
            {
                Debug.Log("glTFast shader variants found in Graphics Settings->Preloaded Shaders");
                shadersIncluded = true;
                break;
            }
        }
        
        if (!shadersIncluded)
        {
            Debug.Log($"Added {shaderVariants.name} to Preloaded Shaders in Graphics Settings ");

            shaderIncludeArray.InsertArrayElementAtIndex(newArrayIndex);
            SerializedProperty shaderInArray = shaderIncludeArray.GetArrayElementAtIndex(newArrayIndex);
            shaderInArray.objectReferenceValue = shaderVariants;
            serializedGraphicsObject.ApplyModifiedProperties();
        }
        AssetDatabase.SaveAssets();
        serializedVariants.Dispose();
    }
}
