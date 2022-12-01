using UnityEditor;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class Postprocessor : AssetPostprocessor
    {
        #region Animation Settings

#if DISABLE_AUTO_INSTALLER
        private const string ANIMATION_ASSET_PATH = "Assets/Avatar Loader/Resources/Animations";
#else
        private const string ANIMATION_ASSET_PATH = "Assets/Ready Player Me/Resources";
#endif

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
