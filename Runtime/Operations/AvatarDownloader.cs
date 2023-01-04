using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarLoader
{
    /// This class is responsible for making a request and downloading an avatar from a URL.
    public class AvatarDownloader : IOperation<AvatarContext>
    {
        private const string TAG = nameof(AvatarDownloader);
        
        /// If true the avatar will download into memory instead of a local file.
        private readonly bool downloadInMemory;


        /// The <c>AvatarDownloader</c> constructor can be used to set <c>downloadInMemory</c>.
        public AvatarDownloader(bool downloadInMemory = false)
        {
            this.downloadInMemory = downloadInMemory;
        }
        
        /// Can be used to set the Timeout used by the <see cref="WebRequestDispatcher"/> when making the web request.
        public int Timeout { get; set; }
        
        /// An <see cref="Action"/> callback that can be used to subscribe to <see cref="WebRequestDispatcher"/> <c>ProgressChanged</c> events.
        public Action<float> ProgressChanged { get; set; }
        
        /// Executes the operation to download the avatar from <c>AvatarContext.AvatarUri</c> and returns the updated context.
        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (context.AvatarUri.Equals(default(AvatarUri)))
            {
                throw new InvalidDataException($"Expected cast {typeof(string)} instead got ");
            }

            if (!context.Metadata.IsUpdated && File.Exists(context.AvatarUri.LocalModelPath))
            {
                SDKLogger.Log(TAG, "Loading model from cache.");
                context.Bytes = File.ReadAllBytes(context.AvatarUri.LocalModelPath);
                return context;
            }

            if (context.Metadata.IsUpdated)
            {
                AvatarCache.ClearAvatar(context.AvatarUri.Guid, context.SaveInProjectFolder);
            }

            if (downloadInMemory)
            {
                context.Bytes = await DownloadIntoMemory(context.AvatarUri.ModelUrl, context.AvatarConfig, token);
                return context;
            }

            context.Bytes = await DownloadIntoFile(context.AvatarUri.ModelUrl, context.AvatarUri.LocalModelPath, context.AvatarConfig, token);
            return context;
        }

        /// An asynchronous task that downloads the avatar into memory and returns the data as a <c>byte[]</c>.
        public async Task<byte[]> DownloadIntoMemory(string url, AvatarConfig avatarConfig = null, CancellationToken token = new CancellationToken())
        {
            if (avatarConfig)
            {
                var parameters = AvatarConfigProcessor.ProcessAvatarConfiguration(avatarConfig);
                url += parameters;
                SDKLogger.Log(TAG, $"Download URL with parameters: {url}");
            }

            SDKLogger.Log(TAG, "Downloading avatar into memory.");

            var dispatcher = new WebRequestDispatcher();
            dispatcher.ProgressChanged = ProgressChanged;

            try
            {
                Response response = await dispatcher.DownloadIntoMemory(url, token, Timeout);
                return response.Data;
            }
            catch (Exception exception)
            {
                throw Fail($"Failed to download glb model into memory. {exception}");
            }
        }
        
        /// An asynchronous task that downloads the avatar into a file stored locally and returns the data as a <c>byte[]</c>.
        public async Task<byte[]> DownloadIntoFile(string url, string path, AvatarConfig avatarConfig = null, CancellationToken token = new CancellationToken())
        {
            if (avatarConfig)
            {
                var parameters = AvatarConfigProcessor.ProcessAvatarConfiguration(avatarConfig);
                url += parameters;
                SDKLogger.Log(TAG, $"Download URL with parameters: {url}");
            }

            SDKLogger.Log(TAG, $"Downloading avatar into file at {path}");

            var dispatcher = new WebRequestDispatcher();
            dispatcher.ProgressChanged = ProgressChanged;

            try
            {
                Response response = await dispatcher.DownloadIntoFile(url, path, token, Timeout);
                return response.Data;
            }
            catch (Exception exception)
            {
                throw Fail($"Failed to download glb model into file. {exception}");
            }
        }

        /// A method used to throw <c>ModelDownloadError</c> exceptions.
        private Exception Fail(string message)
        {
            SDKLogger.Log(TAG, message);
            throw new CustomException(FailureType.ModelDownloadError, message);
        }
    }
}
