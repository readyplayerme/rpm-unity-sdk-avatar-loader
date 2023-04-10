using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class AvatarConfigProcessorTests
    {
        [Test]
        public void Process_Avatar_Configuration()
        {

            Assert.Pass();
        }

        [Test]
        public void Process_Avatar_Configuration_Default()
        {

            Assert.Pass();
        }

        [Test]
        public void Process_Texture_Size_Limit()
        {
            Assert.Pass();
        }

        [Test]
        public void Process_Texture_Channels()
        {
            var textureChannels = new List<TextureChannel>();
            textureChannels.Add(TextureChannel.BaseColor);
            textureChannels.Add(TextureChannel.Normal);
            var textureChanelParams = "&textureChannels=";
            textureChanelParams += AvatarConfigProcessor.ProcessTextureChannels(textureChannels);
            Assert.AreEqual(textureChanelParams, "&textureChannels=baseColor,normal");
        }

        [Test]
        public void Process_Texture_Channels_None()
        {
            var textureChanelParams = "&textureChanelParams=";
            textureChanelParams += AvatarConfigProcessor.ProcessTextureChannels(new List<TextureChannel>());
            Assert.AreEqual(textureChanelParams, "&textureChannels==none");
        }

        [Test]
        public void Process_Morph_Targets()
        {
            var morphTargets = new List<string>();
            morphTargets.Add("mouthOpen");
            morphTargets.Add("mouthSmile");
            var processedMorphParams = AvatarConfigProcessor.ProcessMorphTargets(morphTargets);
            Assert.AreEqual(processedMorphParams, "&morphTargets=mouthOpen,mouthSmile");
        }

        [Test]
        public void Process_Morph_Targets_None()
        {
            var morphTargets = new List<string>();
            morphTargets.Add("none");
            var processedMorphParams = AvatarConfigProcessor.ProcessMorphTargets(morphTargets);
            Assert.AreEqual(processedMorphParams, "&morphTargets=none");
        }
        
        [Test]
        public void Process_Morph_Targets_Empty()
        {
            var processedMorphParams = AvatarConfigProcessor.ProcessMorphTargets(new List<string>());
            Assert.IsEmpty(processedMorphParams);
        }
    }
}