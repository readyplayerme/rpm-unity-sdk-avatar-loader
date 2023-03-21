using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using ReadyPlayerMe.Loader;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class MetadataDownloadTests
    {
        // private const string JSON_MASCULINE_FULL_BODY = "https://models.readyplayer.me/64184ac404207164c85216d6.json";
        // private const string JSON_FEMININE_FULL_BODY = "https://models.readyplayer.me/641975b2398f7e86e696913e.json";
        // private const string JSON_MASCULINE_HALF_BODY = "https://models.readyplayer.me/64184ac404207164c85216d6.json";
        // private const string JSON_FEMININE_HALF_BODY = "https://models.readyplayer.me/641975b2398f7e86e696913e.json";
        // private const string CLOUDFRONT_JSON_FEMININE_FULL_BODY =
        //     "https://d1a370nemizbjq.cloudfront.net/7f7f0ab3-c639-4e0c-82b1-2134c03d2af4.json";
        //
        // private const string CLOUDFRONT_JSON_MASCULINE_FULL_BODY =
        //     "https://d1a370nemizbjq.cloudfront.net/fa83d7ac-3fe0-4589-a42e-7b74ea6142e5.json";
        //
        // private const string CLOUDFRONT_JSON_FEMININE_HALF_BODY =
        //     "https://d1a370nemizbjq.cloudfront.net/419f78a1-f9d4-4695-9cc9-4537a6b2f671.json";
        //
        // private const string CLOUDFRONT_JSON_MASCULINE_HALF_BODY =
        //     "https://d1a370nemizbjq.cloudfront.net/b4082a25-1529-4160-b256-b9595fa7f269.json";

        private const string WRONG_JSON_URL =
            "https://gist.githubusercontent.com/srcnalt/2ca44ce804ac28ce8722a93dca3635c9/raw";
        
        
        private static async Task DownloadAndCheckMetadata(string url, BodyType bodyType, OutfitGender outfitGender, string skinTone = "")
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
            //TODO check skinTone once it has been added to AvatarMetadata class 
            //Assert.AreEqual(skinTone, metadata.SkinTone);
        }

        private static async Task DownloadAndCheckCloudfrontMetadata(string url, BodyType bodyType, OutfitGender outfitGender)
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
            TestUtils.DeleteDirectoryIfExists($"{TestUtils.TestAvatarDirectory}/{TestUtils.CLOUDFRONT_TEST_AVATAR_GUID}", true);
        }

        [Test]
        public async Task Download_Metadata_Into_File()
        {
            AvatarMetadata metadata;

            var metadataDownloader = new MetadataDownloader();
            try
            {
                var url = TestAvatars.GetCloudfrontAvatarJsonUrl(BodyType.FullBody, OutfitGender.Feminine);
                metadata = await metadataDownloader.Download(url);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            metadataDownloader.SaveToFile(metadata, TestUtils.CLOUDFRONT_TEST_AVATAR_GUID, TestUtils.TestJsonFilePath, false);

            Assert.AreEqual(true, File.Exists(TestUtils.TestJsonFilePath));
        }

        [Test]
        public async Task Download_Metadata_Into_Memory()
        {
            var metadataDownloader = new MetadataDownloader();
            try
            {
                var url = TestAvatars.GetCloudfrontAvatarJsonUrl(BodyType.FullBody, OutfitGender.Feminine);
                await metadataDownloader.Download(url);
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
                await metadataDownloader.Download(WRONG_JSON_URL);
            }
            catch (CustomException exception)
            {
                Assert.AreEqual(FailureType.MetadataParseError, exception.FailureType);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public async Task Downloaded_Metadata_UpdatedAt_Is_Not_Default_Value()
        {
            AvatarMetadata metadata;
            var metadataDownloader = new MetadataDownloader();
            try
            {
                var url = TestAvatars.GetCloudfrontAvatarJsonUrl(BodyType.FullBody, OutfitGender.Feminine);
                metadata = await metadataDownloader.Download(url);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            Assert.AreNotEqual(default(DateTime), metadata.UpdatedAt);
        }

        [Test]
        public void Load_Metadata_From_File()
        {
            var avatarMetadata = new AvatarMetadata();

            var metadataDownloader = new MetadataDownloader();
            metadataDownloader.SaveToFile(avatarMetadata, TestUtils.CLOUDFRONT_TEST_AVATAR_GUID, TestUtils.TestJsonFilePath, false);

            AvatarMetadata metadata = metadataDownloader.LoadFromFile(TestUtils.TestJsonFilePath);

            Assert.AreNotSame(new AvatarMetadata(), metadata);
        }
        
        [Test]
        public async Task Check_Metadata_Feminine_Full_Body()
        {
            var url = TestAvatars.GetApiAvatarJsonUrl(BodyType.FullBody, OutfitGender.Feminine);
            await DownloadAndCheckMetadata(url, BodyType.FullBody, OutfitGender.Feminine);
        }
        
        [Test]
        public async Task Check_Metadata_Masculine_Full_Body()
        {
            var url = TestAvatars.GetApiAvatarJsonUrl(BodyType.FullBody, OutfitGender.Masculine);
            await DownloadAndCheckMetadata(url, BodyType.FullBody, OutfitGender.Masculine);
        }        
        
        [Test]
        public async Task Check_Metadata_Feminine_Half_Body()
        {
            var url = TestAvatars.GetApiAvatarJsonUrl(BodyType.HalfBody, OutfitGender.Feminine);
            await DownloadAndCheckMetadata(url, BodyType.HalfBody, OutfitGender.Feminine);
        }
        
        [Test]
        public async Task Check_Metadata_Masculine_Half_Body()
        {
            var url = TestAvatars.GetApiAvatarJsonUrl(BodyType.HalfBody, OutfitGender.Masculine);
            await DownloadAndCheckMetadata(url, BodyType.HalfBody, OutfitGender.Masculine);
        }


        [Test]
        public async Task Check_Cloudfront_Metadata_Feminine_Full_Body()
        {
            var url = TestAvatars.GetCloudfrontAvatarJsonUrl(BodyType.FullBody, OutfitGender.Feminine);
            await DownloadAndCheckCloudfrontMetadata(url, BodyType.FullBody, OutfitGender.Feminine);
        }

        [Test]
        public async Task Check_Cloudfront_Metadata_Masculine_Full_Body()
        {
            var url = TestAvatars.GetCloudfrontAvatarJsonUrl(BodyType.FullBody, OutfitGender.Masculine);
            await DownloadAndCheckCloudfrontMetadata(url, BodyType.FullBody, OutfitGender.Masculine);
        }

        [Test]
        public async Task Check_Cloudfront_Metadata_Feminine_Half_Body()
        {
            var url = TestAvatars.GetCloudfrontAvatarJsonUrl(BodyType.HalfBody, OutfitGender.Feminine);
            await DownloadAndCheckCloudfrontMetadata(url, BodyType.HalfBody, OutfitGender.None);
        }

        [Test]
        public async Task Check_Cloudfront_Metadata_Masculine_Half_Body()
        {
            var url = TestAvatars.GetCloudfrontAvatarJsonUrl(BodyType.HalfBody, OutfitGender.Masculine);
            await DownloadAndCheckCloudfrontMetadata(url, BodyType.HalfBody, OutfitGender.None);
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
