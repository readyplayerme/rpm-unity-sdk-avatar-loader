using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    ///     This class is responsible for requesting and downloading a 2D render of an avatar from a URL.
    /// </summary>
    public class AvatarRenderDownloader : IOperation<AvatarContext>
    {
        private const string TAG = nameof(AvatarRenderDownloader);
        private const string RENDER_URL = "https://render.readyplayer.me/render";
        private const string INVALID_RENDER_URL_ERROR = "Not a valid Avatar Render Url. Check render settings";
        private const string RENDERS = "renders";
        private readonly string[] renderExtensions = { ".png", ".jpg" };

        public int Timeout { get; set; }

        /// <summary>
        ///     An <see cref="Action" /> callback that can be used to subscribe to <see cref="WebRequestDispatcher" />
        ///     <c>ProgressChanged</c> events.
        /// </summary>
        public Action<float> ProgressChanged { get; set; }

        /// <summary>
        ///     Executes the operation to request and download the 2D render and returns the updated context.
        /// </summary>
        /// <param name="context">A container for all the data related to the Avatar model.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <returns>The updated <c>AvatarContext</c>.</returns>
        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            try
            {
                context.Data = await RequestAvatarRenderUrl(context.Bytes, token);
                SDKLogger.Log(TAG, "Avatar Render Downloaded");
                return context;
            }
            catch (CustomException exception)
            {
                throw new CustomException(FailureType.AvatarRenderError, exception.Message);
            }
        }

        /// <summary>
        ///     Requests an avatar render URL asynchronously
        /// </summary>
        /// <param name="payload">The binary data of the avatar model .glb file.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        public async Task<Texture2D> RequestAvatarRenderUrl(byte[] payload, CancellationToken token = new CancellationToken())
        {
            string response;
            var dispatcher = new WebRequestDispatcher();
            dispatcher.ProgressChanged += ProgressChanged;

            try
            {
                response = await dispatcher.Dispatch(RENDER_URL, payload, token);

            }
            catch (CustomException exception)
            {
                throw new CustomException(exception.FailureType, exception.Message);
            }

            return await Parse(response, token);
        }

        /// <summary>
        ///     This method parses the json response <c>string<c> to get a URL and makes a request to download the texture.
        /// </summary>
        /// <param name="json">The reponse data as a json string.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        private async Task<Texture2D> Parse(string json, CancellationToken token)
        {
            try
            {
                JObject renderData = JObject.Parse(json);
                var avatarRenderUrl = renderData[RENDERS][0].ToString();

                if (string.IsNullOrEmpty(avatarRenderUrl) || !ValidateRenderUrl(avatarRenderUrl))
                {
                    throw new CustomException(FailureType.AvatarRenderError, INVALID_RENDER_URL_ERROR);
                }

                var webRequestDispatcher = new WebRequestDispatcher();
                return await webRequestDispatcher.DownloadTexture(avatarRenderUrl, token);
            }
            catch (Exception exception)
            {
                throw new CustomException(FailureType.AvatarRenderError, exception.Message);
            }
        }

        /// <summary>
        ///     Checks that the avatar render URL is valid.
        /// </summary>
        /// <param name="renderUrl"></param>
        /// <returns>A <c>bool</c> if the render URL is valid.</returns>
        private bool ValidateRenderUrl(string renderUrl)
        {
            var url = renderUrl.ToLower();
            return renderExtensions.Any(extension => url.EndsWith(extension));
        }
    }
}
