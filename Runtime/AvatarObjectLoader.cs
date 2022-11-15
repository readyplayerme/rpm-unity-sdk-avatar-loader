using System;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Loader;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class AvatarObjectLoader
    {
        private const string TAG = nameof(AvatarObjectLoader);

        private readonly bool avatarCachingEnabled;

        /// Scriptable Object Avatar API request parameters configuration
        public AvatarConfig AvatarConfig;
        private string avatarUrl;

        private OperationExecutor<AvatarContext> executor;
        private float startTime;

        public AvatarObjectLoader()
        {
            var loaderSettings = AvatarLoaderSettings.LoadSettings();
            avatarCachingEnabled = loaderSettings && loaderSettings.AvatarCachingEnabled;
            AvatarConfig = loaderSettings ? loaderSettings.AvatarConfig : null;
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
        public event EventHandler<AvatarEventArgs> OnCompleted;

        /// Load avatar from given url
        public void LoadAvatar(string url)
        {
            startTime = Time.timeSinceLevelLoad;
            SDKLogger.Log(TAG, $"Started loading avatar with config {(AvatarConfig ? AvatarConfig.name : "None")} from URL {url}");
            avatarUrl = url;
            Load(url);
        }

        /// Cancel avatar loading
        public void Cancel()
        {
            executor.Cancel();
        }

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
                new GltFastAvatarImporter(),
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
                Failed(exception.FailureType, exception.Message);
                return;
            }

            if (executor.IsCancelled)
            {
                SDKLogger.Log(TAG, "Avatar loading cancelled");
            }
            else
            {
                var avatar = (GameObject) context.Data;
                avatar.SetActive(true);
                OnCompleted?.Invoke(this, new AvatarEventArgs
                {
                    Avatar = avatar,
                    Url = context.Url,
                    Metadata = context.Metadata
                });

                SDKLogger.Log(TAG, $"Avatar loaded in {Time.timeSinceLevelLoad - startTime:F2} seconds.");
            }
        }

        private void ProgressChanged(float progress, string type)
        {
            OnProgressChanged?.Invoke(this, new ProgressChangeEventArgs
            {
                Operation = type,
                Url = avatarUrl,
                Progress = progress
            });
        }

        // TODO: add the messages here
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
