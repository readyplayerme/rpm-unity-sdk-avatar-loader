using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.AvatarLoader.Tests;
using UnityEngine;

namespace ReadyPlayerMe.Tests
{
    public class AvatarImportTests
    {
        [Test]
        public async Task Import_Avatar_From_Path()
        {
            GameObject avatar;
            var importer = new GltFastAvatarImporter();
            try
            {
                avatar = await importer.ImportModel(LocalTestAssets.MultiMeshMaleAvatarGlbPath, new CancellationToken());
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.IsNotNull(avatar);
        }

        [Test]
        public async Task Import_Half_Body_Avatar_From_Path()
        {
            GameObject avatar;
            var importer = new GltFastAvatarImporter();
            try
            {
                avatar = await importer.ImportModel(LocalTestAssets.MultiMeshHalfBodyAvatarGlbPath, new CancellationToken());
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.IsNotNull(avatar);
        }

        [Test]
        public async Task Fail_Import_Avatar_From_Path()
        {
            var importer = new GltFastAvatarImporter();
            try
            {
                await importer.ImportModel(TestUtils.MockAvatarGlbWrongPath, new CancellationToken());
            }
            catch (CustomException exception)
            {
                Assert.AreEqual(FailureType.ModelImportError, exception.FailureType);
                return;
            }

            Assert.Fail();
        }


        [Test]
        public async Task Import_Avatar_Multi_Mesh_From_Bytes()
        {
            GameObject avatar;
            var bytes = File.ReadAllBytes(LocalTestAssets.MultiMeshMaleAvatarGlbPath);
            var importer = new GltFastAvatarImporter();
            try
            {
                avatar = await importer.ImportModel(bytes, new CancellationToken());
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.IsNotNull(avatar);
        }

        [Test]
        public async Task Import_Avatar_Single_Mesh_From_Bytes()
        {
            GameObject avatar;
            var bytes = File.ReadAllBytes(LocalTestAssets.SingleMeshFemaleAvatarGlbPath);
            var importer = new GltFastAvatarImporter();
            try
            {
                avatar = await importer.ImportModel(bytes, new CancellationToken());
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.IsNotNull(avatar);
        }

        [Test]
        public async Task Import_Halfbody_Multi_Mesh_Avatar_From_Bytes()
        {
            GameObject avatar;
            var bytes = File.ReadAllBytes(LocalTestAssets.MultiMeshHalfBodyAvatarGlbPath);
            var importer = new GltFastAvatarImporter();
            try
            {
                avatar = await importer.ImportModel(bytes, new CancellationToken());
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.IsNotNull(avatar);
        }

        [Test]
        public async Task Import_Halfbody_Single_Mesh_Avatar_From_Bytes()
        {
            GameObject avatar;
            var bytes = File.ReadAllBytes(LocalTestAssets.SingleMeshHalfBodyAvatarGlbPath);
            var importer = new GltFastAvatarImporter();
            try
            {
                avatar = await importer.ImportModel(bytes, new CancellationToken());
            }
            catch (CustomException exception)
            {
                Assert.Fail(exception.Message);
                throw;
            }

            Assert.IsNotNull(avatar);
        }

        [Test]
        public async Task Fail_Import_Avatar_From_Bytes()
        {
            var importer = new GltFastAvatarImporter();
            try
            {
                await importer.ImportModel(new byte[] { }, new CancellationToken());
            }
            catch (CustomException exception)
            {
                Assert.AreEqual(FailureType.ModelImportError, exception.FailureType);
                return;
            }

            Assert.Fail();
        }
    }
}
