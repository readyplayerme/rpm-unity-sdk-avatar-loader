using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class GltFastAvatarImporter : IOperation<AvatarContext>
    {
        private const string TAG = nameof(GltFastAvatarImporter);
        public int Timeout { get; set; }
        public Action<float> ProgressChanged { get; set; }

        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (context.Bytes == null)
            {
                throw new NullReferenceException();
            }

            context.Data = await ImportModel(context.Bytes, token);
            return context;
        }

        public async Task<GameObject> ImportModel(byte[] bytes, CancellationToken token)
        {
            SDKLogger.Log(TAG, "Importing avatar from byte array.");

            try
            {
                GameObject avatar = null;

                var gltf = new GltfImport(deferAgent: new UninterruptedDeferAgent());
                bool success = await gltf.LoadGltfBinary(
                    bytes
                );
                if (success)
                {
                    avatar = new GameObject();
                    avatar.SetActive(false);
                    var customInstantiator = new GltFastGameObjectInstantiator(gltf, avatar.transform);

                    await gltf.InstantiateMainScene(customInstantiator);
                }

                return avatar;
            }
            catch (Exception exception)
            {
                throw Fail(exception.Message);
            }
        }

        public async Task<GameObject> ImportModel(string path, CancellationToken token)
        {
            SDKLogger.Log(TAG, $"Importing avatar from path {path}");

            try
            {
                GameObject avatar = null;

                byte[] data = File.ReadAllBytes(path);

                var gltf = new GltfImport(deferAgent: new UninterruptedDeferAgent());

                bool success = await gltf.LoadGltfBinary(
                    data,
                    new Uri(path)
                );

                if (success)
                {
                    avatar = new GameObject();
                    avatar.SetActive(false);
                    var customInstantiator = new GltFastGameObjectInstantiator(gltf, avatar.transform);

                    await gltf.InstantiateMainScene(customInstantiator);
                }

                return avatar;
            }
            catch (Exception exception)
            {
                throw Fail(exception.Message);
            }
        }

        private void OnProgressChanged(float progress) => ProgressChanged?.Invoke(progress);

        private Exception Fail(string error)
        {
            var message = $"Failed to import glb model from bytes. {error}";
            SDKLogger.Log(TAG, message);
            throw new CustomException(FailureType.ModelImportError, message);
        }
    }
}
