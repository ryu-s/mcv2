using mcv2;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcv2PluginTests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void NotifyMessageReceivedRawTest()
        {
            var connectionId = new ConnectionId();

            var messageMock = new Mock<ISiteMessage>();
            messageMock.Setup(x => x.Raw).Returns("{}");

            var metadataMock = new Mock<IMessageMetadata2>();

            var userId = "userid";
            var userMock = new Mock<IMcvUser>();
            userMock.Setup(x => x.Id).Returns(userId);
            userMock.Setup(x => x.IsNgUser).Returns(true);
            userMock.Setup(x => x.IsSiteNgUser).Returns(true);
            var notify = new NotifyMessageReceived(connectionId, messageMock.Object, metadataMock.Object, userMock.Object);
            dynamic d = JsonConvert.DeserializeObject(notify.Raw)!;

            Assert.AreEqual("notify", (string)d.type);
        }
        [Test]
        public void Test()
        {
            var s = new ConnectionStatusDiff(new ConnectionId());
            Assert.IsFalse(s.HasChanged());
        }
        [Test]
        public void Test2()
        {
            var s = new ConnectionStatusDiff(new ConnectionId())
            {
                Input = "a",
            };
            Assert.IsTrue(s.HasChanged());
        }
    }
}
