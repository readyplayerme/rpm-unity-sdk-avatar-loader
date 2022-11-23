using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class AvatarRenderLoaderTests
    {
        [UnityTest]
        public IEnumerator RenderLoader_Load()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = (data) => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(url: TestUtils.Uri.ModelUrl, renderScene: TestUtils.RENDER_SCENE);

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(renderTexture);
        }

        [UnityTest]
        public IEnumerator Fail_RenderLoader_Load_Wrong_Url()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = (data) => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(url: TestUtils.WrongUri.ModelUrl, renderScene: TestUtils.RENDER_SCENE);

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.MetadataDownloadError, failureType);
            Assert.IsNull(renderTexture);
        }

        [UnityTest]
        public IEnumerator RenderLoader_Load_With_Correct_BlendShape_Parameters()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = (data) => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(
                url: TestUtils.Uri.ModelUrl,
                renderScene: TestUtils.RENDER_SCENE,
                renderBlendShapeMesh: TestUtils.RENDER_BLENDSHAPE_MESH,
                renderBlendShapes: new Dictionary<string, float> { { TestUtils.RENDER_BLENDSHAPE, 0.5f } }
            );

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(renderTexture);
        }

        [UnityTest]
        public IEnumerator RenderLoader_Load_Incorrect_BlendShape_Mesh_Parameter()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = (data) => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(
                url: TestUtils.Uri.ModelUrl,
                renderScene: TestUtils.RENDER_SCENE,
                renderBlendShapeMesh: TestUtils.RENDER_WRONG_BLENDSHAPE_MESH,
                renderBlendShapes: new Dictionary<string, float> { { TestUtils.RENDER_BLENDSHAPE, 0.5f } }
            );

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(renderTexture);
        }

        [UnityTest]
        public IEnumerator RenderLoader_Load_Incorrect_BlendShape_Shape_Parameter()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = (data) => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(
                url: TestUtils.Uri.ModelUrl,
                renderScene: TestUtils.RENDER_SCENE,
                renderBlendShapeMesh: TestUtils.RENDER_BLENDSHAPE_MESH,
                renderBlendShapes: new Dictionary<string, float> { { TestUtils.RENDER_WRONG_BLENDSHAPE, 0.5f } }
            );

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(renderTexture);
        }
    }
}
