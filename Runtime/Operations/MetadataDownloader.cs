using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading;
using ReadyPlayerMe.Core;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarLoader;

namespace ReadyPlayerMe.Loader
{
    /// <summary>
    /// This class is responsible for handling the avatar meta data .json file download, save, and parsing functionality.
    /// </summary>
    public class MetadataDownloader : IOperation<AvatarContext>
    {
        private const string TAG = nameof(MetadataDownloader);

        /// <summary>
        /// Can be used to set the Timeout (in seconds) used by the <see cref="WebRequestDispatcherExtension" /> when making the web request.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// An <see cref="Action" /> callback that can be used to subscribe to <see cref="WebRequestDispatcherExtension" />
        /// <c>ProgressChanged</c> events.
        /// </summary>
        public Action<float> ProgressChanged { get; set; }

        private const string MetadataTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

        /// <summary>
        /// Executes the operation to download the avatar and save to file if saving is enabled.
        /// </summary>
        /// <param name="context">A container for all the data related to the Avatar model.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <returns>The updated <see cref="AvatarContext" />.</returns>
        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (context.AvatarUri.Equals(default(AvatarUri)))
            {
                throw new InvalidDataException($"Expected cast {typeof(string)}");
            }
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                context.Metadata = LoadFromFile(context.AvatarUri.LocalMetadataPath);
            }
            else
            {
                context.Metadata = await Download(context.AvatarUri.MetadataUrl, token);
                context.IsUpdated = context.SaveInProjectFolder || IsUpdated(context.Metadata, context.AvatarUri, context.AvatarCachingEnabled);
                if (context.IsUpdated)
                {
                    SaveToFile(context.Metadata, context.AvatarUri.Guid, context.AvatarUri.LocalMetadataPath, context.SaveInProjectFolder);
                }
            }

            return context;
        }

        /// <summary>
        /// Downloads the avatar meta data and parses the response.
        /// </summary>
        /// <param name="url">The URL of the avatar metadata ending in <c>.json</c></param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <returns>The avatar metadata as a <see cref="AvatarMetadata" /> structure.</returns>
        public async Task<AvatarMetadata> Download(string url, CancellationToken token = new CancellationToken())
        {
            SDKLogger.Log(TAG, "Downloading metadata into memory.");
            var dispatcher = new WebRequestDispatcher();
            dispatcher.ProgressChanged += ProgressChanged;

            try
            {
#if UNITY_WEBGL
                // add random tail to the url to prevent JSON from being loaded from the browser cache
                var response = await dispatcher.DownloadIntoMemory(url + "?tail=" + Guid.NewGuid(), token, Timeout);
#else
                var response = await dispatcher.DownloadIntoMemory(url, token, Timeout);
#endif
                return ParseResponse(response.Text);
            }
            catch (CustomException error)
            {
                string message;
                FailureType failureType;
                if (error.FailureType == FailureType.MetadataParseError)
                {
                    failureType = error.FailureType;
                    message = error.Message;
                }
                else
                {
                    failureType = FailureType.MetadataDownloadError;
                    message = $"Failed to download metadata into memory. {error}";

                }

                SDKLogger.Log(TAG, message);
                throw new CustomException(failureType, message);
            }
        }

        /// <summary>
        /// Saves the avatar metadata to a local file.
        /// </summary>
        /// <param name="metadata">The metadata to save.</param>
        /// <param name="guid">The avatar guid (identifier).</param>
        /// <param name="path">The path to save the file.</param>
        /// <param name="saveInProject">If true it will save in the project folder instead of the persistant data path.</param>
        public void SaveToFile(AvatarMetadata metadata, string guid, string path, bool saveInProject)
        {
            DirectoryUtility.ValidateAvatarSaveDirectory(guid, saveInProject);
            var json = JsonConvert.SerializeObject(metadata);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Loads the avatar metadata from the specified file path.
        /// </summary>
        /// <param name="path">The path to the meta data <c>.json</c> file.</param>
        /// <returns>The loaded <see cref="AvatarMetadata" />.</returns>
        public AvatarMetadata LoadFromFile(string path)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<AvatarMetadata>(json);
            }

            return new AvatarMetadata();
        }

        /// <summary>
        /// This method checks if the avatar model has been updated.
        /// </summary>
        /// <param name="metadata">The latest version of the avatar metadata.</param>
        /// <param name="uri">The uri with all the model and url information.</param>
        /// <param name="avatarCachingEnabled">This flag is used to enable or disable local avatar caching.</param>
        /// <returns>A <c>bool</c> indicating if the avatar has been updated.</returns>
        /// r=
        /// <remarks>
        /// It is used to determine whether an avatar needs to be downloaded again or can instead be loaded from the
        /// locally stored file.
        /// </remarks>
        private bool IsUpdated(AvatarMetadata metadata, AvatarUri uri, bool avatarCachingEnabled)
        {
            AvatarMetadata previousMetadata = LoadFromFile(uri.LocalMetadataPath);
            if (avatarCachingEnabled && metadata.UpdatedAt == previousMetadata.UpdatedAt) return false;
            return true;
        }

        /// <summary>
        /// This method deserializes the response and parses it as an <see cref="AvatarMetadata" /> structure.
        /// </summary>
        /// <param name="response">The response as a json string.</param>
        /// <param name="lastModified">A string representing the date of the last time the metadata was modified.</param>
        /// <returns>The avatar metadata as an <see cref="AvatarMetadata" /> structure.</returns>
        private AvatarMetadata ParseResponse(string response)
        {
            AvatarMetadata metadata = JsonConvert.DeserializeObject<AvatarMetadata>(response, new JsonSerializerSettings() {
                DateFormatString = MetadataTimeFormat
            });
            
            if (metadata.BodyType == BodyType.None)
            {
                throw new CustomException(FailureType.MetadataParseError, "Failed to parse metadata. Unexpected body type.");
            }
            
            SDKLogger.Log(TAG, $"{metadata.BodyType} metadata loading completed.");
            return metadata;
        }
    }
}
