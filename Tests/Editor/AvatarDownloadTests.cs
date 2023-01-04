using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class AvatarDownloadTests
    {
        [TearDown]
        public void Cleanup()
        {
            TestUtils.DeleteDirectoryIfExists($"{TestUtils.TestAvatarDirectory}/{TestUtils.TEST_AVATAR_GUID}", true);
            TestUtils.DeleteDirectoryIfExists($"{TestUtils.TestAvatarDirectory}/{TestUtils.TEST_WRONG_GUID}", true);
        }

        [Test]
        public async Task Download_Avatar_Into_File()
        {
            byte[] bytes;
            var avatarDownloader = new AvatarDownloader();

            try
            {
                bytes = await avatarDownloader.DownloadIntoFile(TestUtils.Uri.ModelUrl, TestUtils.Uri.LocalModelPath);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.NotNull(bytes);
            Assert.IsTrue(File.Exists(TestUtils.Uri.LocalModelPath));
        }

        [Test]
        public async Task Download_Avatar_Into_Memory()
        {
            byte[] bytes;

            var avatarDownloader = new AvatarDownloader();
            try
            {
                bytes = await avatarDownloader.DownloadIntoMemory(TestUtils.Uri.ModelUrl);
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.NotNull(bytes);
            Assert.IsFalse(File.Exists(TestUtils.Uri.LocalModelPath));
        }


        [Test]
        public async Task Fail_Download_Avatar_Into_File()
        {
            var avatarDownloader = new AvatarDownloader();

            try
            {
                await avatarDownloader
                    .DownloadIntoFile(TestUtils.WrongUri.ModelUrl, TestUtils.WrongUri.LocalModelPath);

            }
            catch (CustomException exception)
            {
                Assert.AreEqual(FailureType.ModelDownloadError, exception.FailureType);
                return;
            }

            Assert.Fail("Download into file should fail.");
        }

        [Test]
        public async Task Fail_Download_Avatar_Into_Memory()
        {
            var avatarDownloader = new AvatarDownloader();

            try
            {
                await avatarDownloader.DownloadIntoMemory(TestUtils.WrongUri.ModelUrl);
            }
            catch (CustomException exception)
            {
                Assert.AreEqual(FailureType.ModelDownloadError, exception.FailureType);
                return;
            }

            Assert.Fail("Download should fail for wrong uri.");
        }

        [Test]
        public async Task Check_Progress_Download_Avatar_Into_File()
        {
            var currentProgress = 0f;
            var cumulativeProgress = 0f;

            var avatarDownloader = new AvatarDownloader();
            avatarDownloader.ProgressChanged = (progress) =>
            {
                currentProgress = progress;
                cumulativeProgress += progress;
            };

            try
            {
                await avatarDownloader.DownloadIntoFile(TestUtils.Uri.ModelUrl, TestUtils.Uri.LocalModelPath);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.AreEqual(1, currentProgress);
            Assert.GreaterOrEqual(cumulativeProgress, 1);
        }
    }
}
