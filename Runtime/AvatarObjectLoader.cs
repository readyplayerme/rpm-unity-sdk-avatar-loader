using System;
using GLTFast;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Loader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// The <c>AvatarObjectLoader</c> is responsible for loading the avatar from a url and spawning it as a GameObject in
    /// the scene.
    /// </summary>
    public class AvatarObjectLoader
    {
        private const string TAG = nameof(AvatarObjectLoader);

        private readonly bool avatarCachingEnabled;

        /// Scriptable Object Avatar API request parameters configuration
        public AvatarConfig AvatarConfig;

        public IDeferAgent DeferAgent;
        
        private string avatarUrl;
        private OperationExecutor<AvatarContext> executor;
        private float startTime;

        /// <summary>
        /// This class constructor is used to any required fields.
        /// </summary>
        public AvatarObjectLoader()
        {
            var loaderSettings = AvatarLoaderSettings.LoadSettings();
            avatarCachingEnabled = loaderSettings && loaderSettings.AvatarCachingEnabled;
            AvatarConfig = loaderSettings.AvatarConfig != null ? loaderSettings.AvatarConfig : null;
            if (loaderSettings.gltFastDeferAgent != null)
            {
                DeferAgent = Object.Instantiate(loaderSettings.gltFastDeferAgent).GetComponent<IDeferAgent>();
            }
        }

        /// If true, saves the avatar in the Asset folder.
        public bool SaveInProjectFolder { get; set; }

        /// Set the timeout for download requests
        public int Timeout { get; set; } = 20;

        /// Called upon avatar loader failure.
        public event EventHandler<FailureEventArgs> OnFailed;

        /// Called upon avatar loader progress change.
        public event EventHandler<ProgressChangeEventArgs> OnProgressChanged;

        /// Called upon avatar loader success.
        public event EventHandler<CompletionEventArgs> OnCompleted;

        /// <summary>
        /// Load avatar from a URL.
        /// </summary>
        /// <param name="url">The URL to the avatars .glb file.</param>
        public void LoadAvatar(string url)
        {
            startTime = Time.timeSinceLevelLoad;
            SDKLogger.Log(TAG, $"Started loading avatar with config {(AvatarConfig ? AvatarConfig.name : "None")} from URL {url}");
            avatarUrl = url;
            Load(url);
        }

        /// <summary>
        /// Cancel avatar loading
        /// </summary>
        public void Cancel()
        {
            executor.Cancel();
        }

        /// <summary>
        /// Runs through the process of loading the avatar and creating a game object via the <c>OperationExecutor</c>.
        /// </summary>
        /// <param name="url">The URL to the avatars .glb file.</param>
        private async void Load(string url)
        {
            var context = new AvatarContext();
            context.Url = url;
            context.SaveInProjectFolder = SaveInProjectFolder;
            context.AvatarCachingEnabled = avatarCachingEnabled;
            context.AvatarConfig = AvatarConfig;
            context.ParametersHash = AvatarCache.GetAvatarConfigurationHash(AvatarConfig);

            executor = new OperationExecutor<AvatarContext>(new IOperation<AvatarContext>[]
            {
                new UrlProcessor(),
                new MetadataDownloader(),
                new AvatarDownloader(),
                new GltFastAvatarImporter(DeferAgent),
                new AvatarProcessor()
            });
            executor.ProgressChanged += ProgressChanged;
            executor.Timeout = Timeout;

            ProgressChanged(0, nameof(AvatarObjectLoader));
            try
            {
                context = await executor.Execute(context);
            }
            catch (CustomException exception)
            {
                Failed(executor.IsCancelled ? FailureType.OperationCancelled : exception.FailureType, exception.Message);
                return;
            }

            var avatar = (GameObject) context.Data;
            avatar.SetActive(true);
            OnCompleted?.Invoke(this, new CompletionEventArgs
            {
                Avatar = avatar,
                Url = context.Url,
                Metadata = context.Metadata
            });

            SDKLogger.Log(TAG, $"Avatar loaded in {Time.timeSinceLevelLoad - startTime:F2} seconds.");
        }

        /// <summary>
        /// This function is called everytime the progress changes on a given IOperation.
        /// </summary>
        /// <param name="progress">The progress of the current operation.</param>
        /// <param name="type">The type of operation that it has changed to.</param>
        private void ProgressChanged(float progress, string type)
        {
            OnProgressChanged?.Invoke(this, new ProgressChangeEventArgs
            {
                Operation = type,
                Url = avatarUrl,
                Progress = progress
            });
        }

        /// <summary>
        /// This function is called if the async <c>Load()</c> function fails either due to error or cancellation.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        private void Failed(FailureType type, string message)
        {
            OnFailed?.Invoke(this, new FailureEventArgs
            {
                Type = type,
                Url = avatarUrl,
                Message = message
            });
            SDKLogger.Log(TAG, $"Failed to load avatar. Error type {type}. URL {avatarUrl}. Message {message}");
        }
    }
}
