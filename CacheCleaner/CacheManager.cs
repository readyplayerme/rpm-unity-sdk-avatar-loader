using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public static class CacheManager
    {
        private const string TAG = nameof(CacheManager);

        /// Total size of avatar stored in persistent cache. Returns total bytes.
        public static float GetCacheSizeMb()
        {
            var path = DirectoryUtility.GetAvatarsDirectoryPath();
            return GetFolderSizeInMb(path);
        }

        public static float GetDirectorySizeInMb(DirectoryInfo directoryInfo)
        {
            // Add file sizes.
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            var size = fileInfos.Sum(fi => fi.Length);

            // Add subdirectory sizes.
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            size += directoryInfos.Sum(DirectoryUtility.GetDirectorySize);
            return size / 1000000f;
        }

        public static float GetFolderSizeInMb(string folderPath)
        {
            return !Directory.Exists(folderPath) ? 0 : GetDirectorySizeInMb(new DirectoryInfo(folderPath));
        }

        /// Total size of an avatars data stored in persistent cache. Returns total bytes.
        public static float GetAvatarDataSizeInMb(string avatarGuid)
        {
            var path = $"{DirectoryUtility.GetAvatarsDirectoryPath()}/{avatarGuid}";
            return GetFolderSizeInMb(path);
        }

        public static void EnforceCacheLimit(AvatarCacheConfig config, AvatarManifest manifest)
        {
            if (config.limitStrategy == LimitStrategy.CacheSize)
            {
                EnforceCacheSize(config.avatarCacheSizeLimit, manifest);
            }
            else
            {
                EnforceAvatarLimit(config.avatarCountLimit, manifest);
            }
        }

        public static void EnforceAvatarLimit(int avatarLimit, AvatarManifest manifest)
        {
            var currentAvatarCount = AvatarCache.GetAvatarCount();
            if (currentAvatarCount <= avatarLimit)
            {
                SDKLogger.Log(TAG, "Avatar count is below limit.");
                return;
            }
            SDKLogger.Log(TAG, $"{manifest.GetIdsByOldestDate().Length}");
            var queue = new Queue<string>(manifest.GetIdsByOldestDate());
            var previousAvatarCount = currentAvatarCount;
            while (currentAvatarCount > avatarLimit)
            {
                if (queue.Count == 0)
                {
                    SDKLogger.LogWarning(TAG, "The queue is empty. Cannot delete more avatars.");
                    break;
                }
                var avatarId = queue.Dequeue();
                AvatarCache.DeleteAvatarFolder(avatarId);
                manifest.RemoveAvatar(avatarId);
                currentAvatarCount--;
            }
            manifest.Save();
            SDKLogger.Log(TAG, $"{previousAvatarCount - currentAvatarCount} avatars deleted.");
        }

        public static void EnforceCacheSize(float cacheSizeLimitMb, AvatarManifest manifest)
        {
            var currentCacheSize = GetCacheSizeMb();
            if (currentCacheSize <= cacheSizeLimitMb)
            {
                Debug.Log("Avatar cache size is below limit.");
                return;
            }
            SDKLogger.Log(TAG, $"{manifest.GetIdsByOldestDate().Length}");
            var queue = new Queue<string>(manifest.GetIdsByOldestDate());
            var previousCacheSize = currentCacheSize;
            var avatarsDeleted = 0;
            while (currentCacheSize > cacheSizeLimitMb)
            {
                if (queue.Count == 0)
                {
                    SDKLogger.LogWarning(TAG, "The queue is empty. Cannot delete more avatars.");
                    break;
                }

                var avatarId = queue.Dequeue();
                var avatarSize = GetAvatarDataSizeInMb(avatarId);
                AvatarCache.DeleteAvatarFolder(avatarId);
                manifest.RemoveAvatar(avatarId);
                currentCacheSize -= avatarSize;
                avatarsDeleted++;
            }
            manifest.Save();
            SDKLogger.Log(TAG, $"{avatarsDeleted} avatars and {previousCacheSize - currentCacheSize} MB deleted.");

        }
    }
}
