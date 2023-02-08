using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is a simple <see cref="Monobehaviour"/> to serve as an example on how to load a request a 2D render of a Ready Player Me avatar at runtime.
    /// </summary>
    public class MultipleAvatarRenderExample : MonoBehaviour
    {
        [Serializable]
        private class RenderData
        {
            public string url;
            public AvatarRenderScene avatarRenderScene;
            public Image image;
            public bool imageLoaded;
        }

        private const string TAG = nameof(MultipleAvatarRenderExample);

        [SerializeField]
        private RenderData[] dataList;
        [SerializeField]
        private GameObject loadingPanel;

        private readonly string blendShapeMesh = "Wolf3D_Avatar";

        /// A collection of blendshape names and values to pose the face mesh into a smile using blendshapes
        private readonly Dictionary<string, float> blendShapes = new Dictionary<string, float>
        {
            { "mouthSmile", 0.7f },
            { "viseme_aa", 0.5f },
            { "jawOpen", 0.3f }
        };

        private async void Start()
        {
            loadingPanel.SetActive(true);

            foreach (var renderData in dataList)
            {
                var avatarRenderer = new AvatarRenderLoader();
                avatarRenderer.OnCompleted = texture => UpdateSprite(renderData.image, texture);
                avatarRenderer.OnFailed = Fail;
                avatarRenderer.LoadRender(renderData.url, renderData.avatarRenderScene, blendShapeMesh, blendShapes);
            }

            while (dataList.Any(x => !x.imageLoaded))
            {
                await Task.Yield();
            }
            loadingPanel.SetActive(false);
        }

        /// Updates the sprite renderer with the provided render
        private void UpdateSprite(Image image, Texture2D render)
        {
            var sprite = Sprite.Create(render, new Rect(0, 0, render.width, render.height), new Vector2(.5f, .5f));
            image.sprite = sprite;
            image.preserveAspect = true;
            SDKLogger.Log(TAG, "Sprite Updated ");
        }

        private void Fail(FailureType type, string message)
        {
            SDKLogger.Log(TAG, $"Failed with error type: {type} and message: {message}");
        }
    }
}
