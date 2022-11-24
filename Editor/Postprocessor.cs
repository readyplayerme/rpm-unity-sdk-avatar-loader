using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class Postprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (var item in importedAssets)
            {
                // TODO Find a better way
                if (item.Contains("RPM_EditorImage_"))
                {
                    UpdateAlwaysIncludedShaderList();
                    AddRpmDefineSymbol();
                    return;
                }
            }
        }

        #region Environment Settings

        private const string RPM_SYMBOL = "READY_PLAYER_ME";

        private static void AddRpmDefineSymbol()
        {
            BuildTargetGroup target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defineString = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            var symbols = new HashSet<string>(defineString.Split(';')) { RPM_SYMBOL };
            var newDefineString = string.Join(";", symbols.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, newDefineString);
        }

        #endregion

        #region Animation Settings

        private const string ANIMATION_ASSET_PATH = "Assets/Plugins/Ready Player Me/Resources/Animations";

        private void OnPreprocessModel()
        {
            var modelImporter = assetImporter as ModelImporter;
            UpdateAnimationFileSettings(modelImporter);
        }

        private void UpdateAnimationFileSettings(ModelImporter modelImporter)
        {
            void SetModelImportData()
            {
                if (modelImporter is null) return;
                modelImporter.useFileScale = false;
                modelImporter.animationType = ModelImporterAnimationType.Human;
            }

            if (assetPath.Contains(ANIMATION_ASSET_PATH))
            {
                SetModelImportData();
            }
        }

        #endregion

        #region Shader Settings

        private const string INCLUDE_SHADER_PROPERTY = "m_PreloadedShaders";
        private const string GRAPHICS_SETTING_PATH = "ProjectSettings/GraphicsSettings.asset";
#if DISABLE_AUTO_INSTALLER
        private const string STANDARD_SHADERS = "Assets/Ready Player Me/Avatar Loader/Shaders/DefaultglTFastShaders.shadervariants";
#else
        private const string STANDARD_SHADERS = "Packages/com.readplayer.me/Shaders/DefaultglTFastShaders.shadervariants";
#endif
        
#if DISABLE_AUTO_INSTALLER
        private const string URP_SHADERS = "Assets/Ready Player Me/Avatar Loader/Shaders/DefaultglTFastShadersURP.shadervariants";
#else
        private const string URP_SHADERS = "Packages/com.readplayer.me/Shaders/DefaultglTFastShadersURP.shadervariants";
#endif
        
#if DISABLE_AUTO_INSTALLER
        private const string HDRP_SHADERS = "Assets/Ready Player Me/Avatar Loader/Shaders/DefaultglTFastShadersURP.shadervariants";
#else
        private const string HDRP_SHADERS = "Packages/com.readplayer.me/Shaders/DefaultglTFastShadersURP.shadervariants";
#endif

        private static void UpdateAlwaysIncludedShaderList()
        {
            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);
            
            SerializedProperty shaderIncludeArray = serializedGraphicsObject.FindProperty(INCLUDE_SHADER_PROPERTY);

            var renderPipelineAsset = GraphicsSettings.defaultRenderPipeline;
            string shaderPath;
            if (renderPipelineAsset == null)
            {
                shaderPath = STANDARD_SHADERS;
            }
            else if (renderPipelineAsset.GetType().Name == "UniversalRenderPipelineAsset")
            {
                shaderPath = URP_SHADERS;
            }
            else
            {
                shaderPath = HDRP_SHADERS;
            }

            var shaderVariants = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderPath);
            var newArrayIndex = shaderIncludeArray.arraySize;
            shaderIncludeArray.InsertArrayElementAtIndex(newArrayIndex);
            SerializedProperty shaderInArray = shaderIncludeArray.GetArrayElementAtIndex(newArrayIndex);
            shaderInArray.objectReferenceValue = shaderVariants;
            serializedGraphicsObject.ApplyModifiedProperties();

            AssetDatabase.SaveAssets();
        }

        #endregion
    }
}
