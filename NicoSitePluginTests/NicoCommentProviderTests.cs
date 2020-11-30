using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Moq;
using NicoSitePlugin;
using NUnit.Framework;
using SitePlugin;
using SitePluginCommon;

namespace NicoSitePluginTests
{
    [TestFixture]
    class NicoCommentProviderTests
    {
        [Test]
        public void GetAdCommentWithMessageTest()
        {
            Assert.AreEqual("提供：無料チケマン「無料チケット捨てに来ました！」（200pt）", Tools.GetAdComment("/nicoad {\"latestNicoad\":{\"advertiser\":\"無料チケマン\",\"point\":200,\"message\":\"無料チケット捨てに来ました！\"},\"contributionRanking\":{\"totalPoint\":10800,\"ranking\":[{\"advertiser\":\"クリームパンナちゃん\",\"rank\":1,\"message\":\"かがーさんの楽しい旅♪\"},{\"advertiser\":\"ゲスト\",\"rank\":2},{\"advertiser\":\"ケイジ\",\"rank\":3},{\"advertiser\":\"キエル\",\"rank\":4},{\"advertiser\":\"無料チケマン\",\"rank\":5}]}}"));
        }
        [Test]
        public void GetAdCommentWithoutMessageTest()
        {
            Assert.AreEqual("提供：無料チケマン（200pt）", Tools.GetAdComment("/nicoad {\"latestNicoad\":{\"advertiser\":\"無料チケマン\",\"point\":200},\"contributionRanking\":{\"totalPoint\":10800,\"ranking\":[{\"advertiser\":\"クリームパンナちゃん\",\"rank\":1,\"message\":\"かがーさんの楽しい旅♪\"},{\"advertiser\":\"ゲスト\",\"rank\":2},{\"advertiser\":\"ケイジ\",\"rank\":3},{\"advertiser\":\"キエル\",\"rank\":4},{\"advertiser\":\"無料チケマン\",\"rank\":5}]}}"));
        }
    }
}
