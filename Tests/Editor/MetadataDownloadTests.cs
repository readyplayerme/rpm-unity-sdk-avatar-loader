using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Loader;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class MetadataDownloadTests
    {
        private static async Task TestMetadataAPI(string url, BodyType bodyType, OutfitGender outfitGender)
        {
            AvatarMetadata metadata;

            var metadataDownloader = new MetadataDownloader();
            try
            {
                metadata = await metadataDownloader.Download(url);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.AreEqual(bodyType, metadata.BodyType);
            Assert.AreEqual(outfitGender, metadata.OutfitGender);
        }

        [TearDown]
        public void Cleanup()
        {
            TestUtils.DeleteDirectoryIfExists($"{TestUtils.TestAvatarDirectory}/{TestUtils.TEST_AVATAR_GUID}", true);
        }

        [Test]
        public async Task Download_Metadata_Into_File()
        {
            AvatarMetadata metadata;

            var metadataDownloader = new MetadataDownloader();
            try
            {
                metadata = await metadataDownloader.Download(TestUtils.JSON_FEMININE_FULL_BODY);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            metadataDownloader.SaveToFile(metadata, TestUtils.TEST_AVATAR_GUID, TestUtils.TestJsonFilePath, false);

            Assert.AreEqual(true, File.Exists(TestUtils.TestJsonFilePath));
        }

        [Test]
        public async Task Download_Metadata_Into_Memory()
        {
            var metadataDownloader = new MetadataDownloader();
            try
            {
                await metadataDownloader.Download(TestUtils.JSON_FEMININE_FULL_BODY);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            Assert.Pass();
        }

        [Test]
        public async Task Fail_Download_Metadata_Into_Memory_With_Wrong_JSON()
        {
            var metadataDownloader = new MetadataDownloader();
            try
            {
                await metadataDownloader.Download(TestUtils.WRONG_JSON_URL);
            }
            catch (CustomException exception)
            {
                Assert.AreEqual(FailureType.MetadataParseError, exception.FailureType);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public async Task Downloaded_Metadata_Last_Modified_Is_Not_Default_Value()
        {
            AvatarMetadata metadata;
            var metadataDownloader = new MetadataDownloader();
            try
            {
                metadata = await metadataDownloader.Download(TestUtils.JSON_FEMININE_FULL_BODY);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            Assert.AreNotEqual(default(DateTime), metadata.LastModified);
        }

        [Test]
        public void Load_Metadata_From_File()
        {
            var avatarMetadata = new AvatarMetadata();

            var metadataDownloader = new MetadataDownloader();
            metadataDownloader.SaveToFile(avatarMetadata, TestUtils.TEST_AVATAR_GUID, TestUtils.TestJsonFilePath, false);

            var metadata = metadataDownloader.LoadFromFile(TestUtils.TestJsonFilePath, true);

            Assert.AreNotSame(new AvatarMetadata(), metadata);
        }

        [Test]
        public async Task Check_Metadata_API_Feminine_Full_Body()
        {
            await TestMetadataAPI(TestUtils.JSON_FEMININE_FULL_BODY, BodyType.FullBody, OutfitGender.Feminine);
        }

        [Test]
        public async Task Check_Metadata_API_Masculine_Full_Body()
        {
            await TestMetadataAPI(TestUtils.JSON_MASCULINE_FULL_BODY, BodyType.FullBody, OutfitGender.Masculine);
        }

        [Test]
        public async Task Check_Metadata_API_Feminine_Half_Body()
        {
            await TestMetadataAPI(TestUtils.JSON_FEMININE_HALF_BODY, BodyType.HalfBody, OutfitGender.None);
        }

        [Test]
        public async Task Check_Metadata_API_Masculine_Half_Body()
        {
            await TestMetadataAPI(TestUtils.JSON_MASCULINE_HALF_BODY, BodyType.HalfBody, OutfitGender.None);
        }

        /*
        // TODO: These fail in CI due to curl error timeout fires
        [UnityTest]
        public IEnumerator Fail_Download_Metadata_Into_Memory_With_Timeout()
        {
            var failureType = FailureType.None;
            
            var metadataDownloader = new MetadataDownloader();
            metadataDownloader.Timeout = 1;
            metadataDownloader.OnFailed = (type, _) => failureType = type;
            metadataDownloader.DownloadIntoMemory(TestUtils.LARGE_JSON_URL).Run();

            yield return new WaitUntil(() => failureType != FailureType.None);
            
            Assert.AreEqual(FailureType.MetadataDownloadError,failureType);
        }
        
        [UnityTest]
        public IEnumerator Fail_Download_Metadata_Into_File_With_Timeout()
        {
            var failureType = FailureType.None;
            
            var metadataDownloader = new MetadataDownloader();
            metadataDownloader.Timeout = 1;
            metadataDownloader.OnFailed = (type, _) => failureType = type;
            metadataDownloader.DownloadIntoFile(TestUtils.LARGE_JSON_URL, TestUtils.TestJsonFilePath).Run();
            
            yield return new WaitUntil(() => failureType != FailureType.None);
            
            Assert.AreEqual(FailureType.MetadataDownloadError,failureType);
        }
        */

        /*
        // TODO: Enable these tests when half-body avatar metadata is updated
        [UnityTest]
        public IEnumerator Check_Metadata_API_Feminine_Half_Body()
        {
            yield return TestMetadataAPI(TestUtils.JSON_FEMININE_HALF_BODY, BodyType.HalfBody, OutfitGender.Feminine);
        }
        
        [UnityTest]
        public IEnumerator Check_Metadata_API_Masculine_Half_Body()
        {
            yield return TestMetadataAPI(TestUtils.JSON_MASCULINE_HALF_BODY, BodyType.HalfBody, OutfitGender.Masculine);
        }
        */
    }
}
