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
        AddPreloadShaders();
    }
    
    public static void AddPreloadShaders()
    {
        Debug.Log("RUNNING PRELOAD SHADERS");
        var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
        var serializedGraphicsObject = new SerializedObject(graphicsSettings);
        
        SerializedProperty shaderIncludeArray = serializedGraphicsObject.FindProperty(INCLUDE_SHADER_PROPERTY);

        var renderPipelineAsset = GraphicsSettings.defaultRenderPipeline;
        string shaderPath = renderPipelineAsset == null ? STANDARD_SHADERS :  URP_SHADERS;

        var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderPath);
        var newArrayIndex = shaderIncludeArray.arraySize;
        
        var shadersIncluded = false;
        for (int i = 0; i < shaderIncludeArray.arraySize; i++)
        {
            var shaderCollection = shaderIncludeArray.GetArrayElementAtIndex(i);
            shadersIncluded = shaderCollection.name == shaderVariants.name;
        }
        
        if (shadersIncluded)
        {
            shaderIncludeArray.InsertArrayElementAtIndex(newArrayIndex);
            SerializedProperty shaderInArray = shaderIncludeArray.GetArrayElementAtIndex(newArrayIndex);
            shaderInArray.objectReferenceValue = shaderVariants;
            serializedGraphicsObject.ApplyModifiedProperties();
        }

        AssetDatabase.SaveAssets();
    }
}
