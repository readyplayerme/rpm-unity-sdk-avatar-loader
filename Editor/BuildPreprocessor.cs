using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class BuildPreprocessor : IPreprocessBuildWithReport
    {

        #region Constants
    private const string ADD_SHADER_VARIANTS = "Add And Build";
        private const string BUILD_WARNING = "Build Warning";
        private const string SUBDOMAIN_WARNING =
            "It looks like the glTFast Shader Variants are missing from the Graphics Settings/Preloaded Shader list list. This can cause errors when loading Ready Player Me avatars at runtime. Would you like to add them now before building?";
        private const string CONTINUE_WITH_DEMO = "Build without Variants";

    #endregion

        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!Application.isBatchMode && ShaderHelper.IsMissingVariants())
            {
                var addShaderVariants = EditorUtility.DisplayDialog(BUILD_WARNING,
                    SUBDOMAIN_WARNING,
                    ADD_SHADER_VARIANTS,
                    CONTINUE_WITH_DEMO);

                if (addShaderVariants)
                {
                    ShaderHelper.AddRemovePreloadShaders();
                }
                else
                {
                    Debug.LogWarning("Building without adding glTFast Shader Variants");
                }
            }
        }
    }
}