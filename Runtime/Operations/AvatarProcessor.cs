﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.AvatarLoader
{
    /// This class is responsible for making processing the avatar after it has been loaded into a GameObject.
    public class AvatarProcessor : IOperation<AvatarContext>
    {
        private const string TAG = nameof(AvatarProcessor);
        
        public int Timeout { get; set; }
        
        /// An <see cref="Action"/> callback that can be used to subscribe to <see cref="WebRequestDispatcher"/> <c>ProgressChanged</c> events.
        public Action<float> ProgressChanged { get; set; }

        /// Executes the operation to process the avatar <see cref="GameObject"/>.
        public Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (context.Data is GameObject)
            {
                context = ProcessAvatarGameObject(context);
                ProcessAvatar(context.Data as GameObject, context.Metadata);
                ProgressChanged?.Invoke(1);
                return Task.FromResult(context);
            }

            throw new CustomException(FailureType.AvatarProcessError, $"Avatar postprocess failed. {context.Data} is either null or is not of type GameObject");
        }

        /// Replaces the instance of the avatar GameObject if it already exists and sets the name of the GameObject.
        private AvatarContext ProcessAvatarGameObject(AvatarContext context)
        {
#if UNITY_EDITOR
            if (context.SaveInProjectFolder)
            {
                Object.DestroyImmediate((Object) context.Data);
                AssetDatabase.Refresh();
                var path = $"{DirectoryUtility.GetRelativeProjectPath(context.AvatarUri.Guid)}/{context.AvatarUri.Guid}.glb";
                var avatarAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                context.Data = Object.Instantiate(avatarAsset);
            }
#endif
            GameObject oldInstance = GameObject.Find(context.AvatarUri.Guid);
            if (oldInstance)
            {
                Object.DestroyImmediate(oldInstance);
            }

            ((Object) context.Data).name = context.AvatarUri.Guid;

            return context;
        }
        
        /// This method triggers GameObject changes and setup for the new avatar GameObject.
        public void ProcessAvatar(GameObject avatar, AvatarMetadata avatarMetadata)
        {
            SDKLogger.Log(TAG, "Processing avatar.");

            try
            {
                if (avatar.transform.Find(BONE_HALF_BODY_ROOT))
                {
                    RemoveHalfBodyRoot(avatar);
                }

                if (!avatar.transform.Find(BONE_ARMATURE))
                {
                    AddArmatureBone(avatar);
                }

                if (avatarMetadata.BodyType == BodyType.FullBody)
                {
                    SetupAnimator(avatar, avatarMetadata.OutfitGender);
                }

                RenameChildMeshes(avatar);
            }
            catch (Exception e)
            {
                var message = $"Avatar postprocess failed. {e.Message}";
                SDKLogger.Log(TAG, message);
                throw new CustomException(FailureType.AvatarProcessError, message);
            }
        }


        #region Setup Armature and Animations

        // Animation avatars resource paths
        private const string MASCULINE_ANIMATION_AVATAR_NAME = "AnimationAvatars/MasculineAnimationAvatar";
        private const string FEMININE_ANIMATION_AVATAR_NAME = "AnimationAvatars/FeminineAnimationAvatar";

        // Bone names
        private const string BONE_HIPS = "Hips";
        private const string BONE_ARMATURE = "Armature";
        private const string BONE_HALF_BODY_ROOT = "AvatarRoot";
        
        /// Removes the roo bone to ensure the correct skeleton hierarchy.
        private void RemoveHalfBodyRoot(GameObject avatar)
        {
            Transform root = avatar.transform.Find(BONE_HALF_BODY_ROOT);
            for (var i = root.childCount - 1; i >= 0; --i)
            {
                root.GetChild(i).transform.SetParent(avatar.transform);
            }
            Object.DestroyImmediate(root.gameObject);
        }
        
        /// Adds the root armature bone to ensure the correct skeleton hierarchy.
        private void AddArmatureBone(GameObject avatar)
        {
            SDKLogger.Log(TAG, "Adding armature bone");

            var armature = new GameObject();
            armature.name = BONE_ARMATURE;
            armature.transform.parent = avatar.transform;

            Transform hips = avatar.transform.Find(BONE_HIPS);
            if (hips) hips.parent = armature.transform;
        }
        
        /// Adds an <see cref="Animator"/> component and sets the target <see cref="UnityEngine.Avatar"/>.
        private void SetupAnimator(GameObject avatar, OutfitGender gender)
        {
            SDKLogger.Log(TAG, "Setting up animator");

            var animationAvatarSource = gender == OutfitGender.Masculine
                ? MASCULINE_ANIMATION_AVATAR_NAME
                : FEMININE_ANIMATION_AVATAR_NAME;
            var animationAvatar = Resources.Load<Avatar>(animationAvatarSource);
            var animator = avatar.AddComponent<Animator>();
            animator.avatar = animationAvatar;
            animator.applyRootMotion = true;
        }

        #endregion

        #region Set Component Names

        // Prefix to remove from names for correction
        private const string PREFIX = "Wolf3D_";

        // Default prefixes
        private const string AVATAR_PREFIX = "Avatar";
        private const string RENDERER_PREFIX = "Renderer";
        private const string MATERIAL_PREFIX = "Material";
        private const string SKINNED_MESH_PREFIX = "SkinnedMesh";


        // Texture property IDs
        private static readonly string[] ShaderProperties =
        {
            "baseColorTexture",
            "normalTexture",
            "emissiveTexture",
            "occlusionTexture",
            "metallicRoughnessTexture"
        };
        
        ///     Rename avatar assets.
        /// <remarks>Naming convention is 'Avatar_Type_Name'. This makes it easier to view them in profiler</remarks>

        private void RenameChildMeshes(GameObject avatar)
        {
            SkinnedMeshRenderer[] renderers = avatar.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                var assetName = renderer.name.Replace(PREFIX, "");

                renderer.name = $"{RENDERER_PREFIX}_{assetName}";
                renderer.sharedMaterial.name = $"{MATERIAL_PREFIX}_{assetName}";
                SetTextureNames(renderer, assetName);
                SetMeshName(renderer, assetName);
            }
        }
        
        ///     Set the names of each <see cref="Texture"/>.
        /// <remarks>Naming convention is 'Avatar_PropertyName_AssetName'. This makes it easier to view them in profiler</remarks>
        private void SetTextureNames(Renderer renderer, string assetName)
        {
            foreach (var propertyName in ShaderProperties)
            {
                var propertyID = Shader.PropertyToID(propertyName);

                if (renderer.sharedMaterial.HasProperty(propertyID))
                {
                    Texture texture = renderer.sharedMaterial.GetTexture(propertyID);
                    if (texture != null) texture.name = $"{AVATAR_PREFIX}{propertyName}_{assetName}";
                }
            }
        }

        ///     Set the name of the <see cref="SkinnedMeshRenderer"/>.
        /// <remarks>Naming convention is 'SkinnedMesh_AssetName'. This makes it easier to view in profiler</remarks>
        private void SetMeshName(SkinnedMeshRenderer renderer, string assetName)
        {
            renderer.sharedMesh.name = $"{SKINNED_MESH_PREFIX}_{assetName}";
            renderer.updateWhenOffscreen = true;
        }

        #endregion
    }
}
