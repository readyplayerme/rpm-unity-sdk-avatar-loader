using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is responsible for the <see cref="AvatarConfig" />.
    /// </summary>
    public static class AvatarConfigProcessor
    {
        private const string TAG = nameof(AvatarConfigProcessor);

        private const string PARAM_TRUE = "true";
        private const string PARAM_FALSE = "false";

        /// <summary>
        /// This method converts the <see cref="AvatarConfig" /> data and combines it into a <c>string</c> that can be added to
        /// an avatar URL.
        /// </summary>
        /// <param name="avatarConfig">Stores the settings of the <see cref="AvatarConfig" /> to use when requesting the avatar.</param>
        /// <returns>The <see cref="AvatarConfig" /> parameters combined as a <c>string</c>.</returns>
        public static string ProcessAvatarConfiguration(AvatarConfig avatarConfig)
        {
            SDKLogger.Log(TAG, "Processing Avatar Configuration");

            var queryBuilder = new QueryBuilder();
            queryBuilder.AddKeyValue(AvatarAPIParameters.POSE, AvatarConfigMap.Pose[avatarConfig.Pose]);
            queryBuilder.AddKeyValue(AvatarAPIParameters.MESH_LOD, ((int) avatarConfig.MeshLod).ToString());
            queryBuilder.AddKeyValue(AvatarAPIParameters.TEXTURE_ATLAS, AvatarConfigMap.TextureAtlas[avatarConfig.TextureAtlas]);
            queryBuilder.AddKeyValue(AvatarAPIParameters.TEXTURE_SIZE_LIMIT, ProcessTextureSizeLimit(avatarConfig.TextureSizeLimit).ToString());
            queryBuilder.AddKeyValue(AvatarAPIParameters.TEXTURE_CHANNELS, ProcessTextureChannels(avatarConfig.TextureChannel));
            if (avatarConfig.MorphTargets.Count > 0)
            {
                queryBuilder.AddKeyValue(AvatarAPIParameters.MORPH_TARGETS, string.Join(",", avatarConfig.MorphTargets));
            }
            queryBuilder.AddKeyValue(AvatarAPIParameters.USE_HANDS, GetBoolStringValue(avatarConfig.UseHands));
            queryBuilder.AddKeyValue(AvatarAPIParameters.USE_DRACO, GetBoolStringValue(avatarConfig.UseDracoCompression));

            return queryBuilder.GetQuery;
        }

        private static string GetBoolStringValue(bool value)
        {
            return value ? PARAM_TRUE : PARAM_FALSE;
        }

        /// <summary>
        /// Processes the <paramref name="textureSize" /> and ensures it is a valid value.
        /// </summary>
        /// <param name="textureSize">The value to process.</param>
        /// <returns>A validated <c>int</c>/returns>
        public static int ProcessTextureSizeLimit(int textureSize)
        {
            return textureSize % 2 == 0 ? textureSize : textureSize + 1;
        }

        /// <summary>
        /// Combines the <paramref name="channels"/> in into a single valid textureChannel parameter.
        /// </summary>
        /// <param name="channels">A list of texture channel</param>
        /// <returns>A query string of combined texture channels</returns>
        public static string ProcessTextureChannels(IReadOnlyCollection<TextureChannel> channels)
        {
            if (!channels.Any())
            {
                return "none";
            }

            var parameter = string.Join(",", channels.Select(channel =>
            {
                var channelString = channel.ToString();
                return char.ToLowerInvariant(channelString[0]) + channelString.Substring(1);
            }));

            return parameter;
        }

        /// <summary>
        /// Combines the list of strings in <paramref name="targets" /> into a single valid morph target parameter.
        /// </summary>
        /// <param name="targets">A list of morph targets as strings.</param>
        /// <returns>A query string of combined morph targets.</returns>
        public static string ProcessMorphTargets(IReadOnlyCollection<string> targets)
        {
            return targets.Count == 0 ? string.Empty : $"&{AvatarAPIParameters.MORPH_TARGETS}={string.Join(",", targets)}";
        }
    }
}
