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
                Debug.Log("RUNNING LOADER POST PROCESSOR");
                // TODO Find a better way
                if (item.Contains("RPM_EditorImage_"))
                {
                    ShaderHelper.AddPreloadShaders();
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
        
    }
}
