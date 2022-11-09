using GLTFast;
using GLTFast.Logging;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class GltFastGameObjectInstantiator : GameObjectInstantiator
    {
        public GltFastGameObjectInstantiator(
            IGltfReadable gltf,
            Transform parent,
            ICodeLogger logger = null,
            InstantiationSettings settings = null
        )
            : base(gltf, parent, logger, settings)
        {
        }

        /// <inheritdoc />
        public override void AddPrimitive(
            uint nodeIndex,
            string meshName,
            Mesh mesh,
            int[] materialIndices,
            uint[] joints = null,
            uint? rootJoint = null,
            float[] morphTargetWeights = null,
            int primitiveNumeration = 0
        )
        {
            if ((settings.mask & ComponentType.Mesh) == 0)
            {
                return;
            }

            GameObject meshGo;
            if (primitiveNumeration == 0)
            {
                // Use Node GameObject for first Primitive
                meshGo = nodes[nodeIndex];
                // Ready Player Me - Parent mesh to Avatar root game object
                meshGo.transform.SetParent(parent.transform);
            }
            else
            {
                meshGo = new GameObject(meshName);
                meshGo.transform.SetParent(nodes[nodeIndex].transform, false);
                meshGo.layer = settings.layer;
            }

            Renderer renderer;

            var hasMorphTargets = mesh.blendShapeCount > 0;
            if (joints == null && !hasMorphTargets)
            {
                var mf = meshGo.AddComponent<MeshFilter>();
                mf.mesh = mesh;
                var mr = meshGo.AddComponent<MeshRenderer>();
                renderer = mr;
            }
            else
            {
                var smr = meshGo.AddComponent<SkinnedMeshRenderer>();
                smr.updateWhenOffscreen = settings.skinUpdateWhenOffscreen;
                if (joints != null)
                {
                    var bones = new Transform[joints.Length];
                    for (var j = 0; j < bones.Length; j++)
                    {
                        var jointIndex = joints[j];
                        bones[j] = nodes[jointIndex].transform;
                    }
                    smr.bones = bones;
                    if (rootJoint.HasValue)
                    {
                        smr.rootBone = nodes[rootJoint.Value].transform;
                    }
                }
                smr.sharedMesh = mesh;
                if (morphTargetWeights != null)
                {
                    for (var i = 0; i < morphTargetWeights.Length; i++)
                    {
                        var weight = morphTargetWeights[i];
                        smr.SetBlendShapeWeight(i, weight);
                    }
                }
                renderer = smr;
            }

            var materials = new Material[materialIndices.Length];
            for (var index = 0; index < materials.Length; index++)
            {
                var material = gltf.GetMaterial(materialIndices[index]) ?? gltf.GetDefaultMaterial();
                materials[index] = material;
            }

            renderer.sharedMaterials = materials;
        }
    }
}
