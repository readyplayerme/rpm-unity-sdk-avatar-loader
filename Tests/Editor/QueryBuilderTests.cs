using NUnit.Framework;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class QueryBuilderTests
    {
        private const string ATLAS_AND_MORPHS = "?textureAtlas=512&morphTargets=mouthSmile,ARKit";
        private const string QUALITY_LOW_MESH_LOD = "?quality=low&meshLod=0";
        
        private readonly string[] morphTargetsDefault = { "mouthSmile", "ARKit" };
        
        [Test]
        public void Low_Quality_MeshLod_0()
        {
            var queryBuilder = new QueryBuilder();
            queryBuilder.AddKeyValue(AvatarAPIParameters.QUALITY, "low");
            queryBuilder.AddKeyValue(AvatarAPIParameters.MESH_LOD,"0");
            Assert.AreEqual(queryBuilder.Query, QUALITY_LOW_MESH_LOD);
        }
        
        [Test]
        public void Texture_Atlas_1024_MorphTargets()
        {
            var queryBuilder = new QueryBuilder();
            queryBuilder.AddKeyValue(AvatarAPIParameters.TEXTURE_ATLAS,"1024" );
            queryBuilder.AddKeyValue(AvatarAPIParameters.MORPH_TARGETS,AvatarConfigProcessor.CombineMorphTargetNames(morphTargetsDefault) );
            Assert.AreEqual(queryBuilder.Query, ATLAS_AND_MORPHS);
        }
    }
}
