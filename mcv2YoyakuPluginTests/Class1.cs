using Common;
using mcv2.Model;
using mcv2YoyakuPlugin;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SitePlugin;

namespace mcv2YoyakuPluginTests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void 保存されたユーザーデータが正しく復元されるか()
        {
            var userStore = new TestUserStore();
            var loggerMock = new Mock<ILogger>();
            var ioMock = new Mock<IIo>();
            var usersFile = "adsdddrfgfCXriytff\tuser1\tnick1\t2020/09/20 21:21:00\tFalse\tf1631b64-6572-4530-abaf-21707f15d893\r\n"
                + "dsddgdgs\tuser2\tnick2\t2020/09/21 00:37:11\tTrue\tEF6C381A-CA8C-4800-82A7-013B7CD1C3A1\r\n";
            ioMock.Setup(x => x.ReadFile(It.IsAny<string>())).Returns(usersFile);
            var optionsMock = new Mock<ICoreOptions>();
            optionsMock.Setup(x => x.SettingsDirPath).Returns("settings");
            var pluginManager = new PluginManager();
            var sitePluginManager = new Mock<ISitePluginManager>().Object;
            var model = new mcv2.Model.Model(pluginManager, sitePluginManager, userStore, loggerMock.Object, ioMock.Object, optionsMock.Object);
            var pluginMock = new Mock<YoyakuPluginMain>
            {
                CallBase = true,
            };

            var pluginModel = new mcv2YoyakuPlugin.Model(new DynamicOptions(), model);
            pluginMock.Protected().Setup<mcv2YoyakuPlugin.Model>("CreateModel").Returns(pluginModel);
            pluginManager.AddPlugin(model, pluginMock.Object);

            Assert.AreEqual(2, pluginModel.RegisteredUsers.Count);
            var user1 = pluginModel.RegisteredUsers[0];
            Assert.AreEqual("user1", user1.Name);
            var user2 = pluginModel.RegisteredUsers[1];
            Assert.AreEqual("user2", user2.Name);
        }
    }
}
