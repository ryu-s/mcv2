using Common;
using mcv2;
using mcv2.Model;
using Moq;
using NUnit.Framework;
using SitePlugin;

namespace mcv2Tests
{
    [TestFixture]
    public class ConnectionTests
    {
        [Test]
        public void ConnectionのInputを書き換える()
        {
            var pluginManager = new PluginManager();
            var sitePluginManager = new Mock<ISitePluginManager>().Object;
            var pluginId = new PluginId();
            var loggerMock = new Mock<ILogger>();
            var ioMock = new Mock<IIo>();
            var coreOptionsMock = new Mock<ICoreOptions>();
            var userStoreMock = new Mock<mcv2.Model.IUserStore>();
            var model = new Model(pluginManager, sitePluginManager, userStoreMock.Object, loggerMock.Object, ioMock.Object, coreOptionsMock.Object);

            var pluginMock = new Mock<IMcvPluginV1>();
            pluginMock.Setup(x => x.Id).Returns(pluginId);
            pluginManager.AddPlugin(model, pluginMock.Object);

            //追加
            model.SetRequest(pluginId, new RequestAddConnection());

            //ConnectionIdを取得
            var l2 = model.GetData(new RequestConnectionIds()) as ResponseConnectionIds;
            var connId = l2.Ids[0];

            //初期値は""
            var l3 = model.GetData(new RequestConnectionStatus(connId)) as ResponseConnectionStatusAdded;
            Assert.AreEqual("", l3.Input);

            //Inputを書き換え
            model.SetRequest(pluginId, new RequestChangeConnectionStatus(connId)
            {
                Input = "abc"
            });

            //反映されているか
            var l4 = model.GetData(new RequestConnectionStatus(connId)) as ResponseConnectionStatusAdded;
            Assert.AreEqual("abc", l4.Input);
        }
        [Test]
        public void Connectionを追加する()
        {
            var pluginManager = new PluginManager();
            var sitePluginManager = new Mock<ISitePluginManager>().Object;
            var pluginId = new PluginId();
            var loggerMock = new Mock<ILogger>();
            var ioMock = new Mock<IIo>();
            var coreOptionsMock = new Mock<ICoreOptions>();
            var userStoreMock = new Mock<mcv2.Model.IUserStore>();
            var model = new Model(pluginManager, sitePluginManager, userStoreMock.Object, loggerMock.Object, ioMock.Object, coreOptionsMock.Object);

            var pluginMock = new Mock<IMcvPluginV1>();
            pluginMock.Setup(x => x.Id).Returns(pluginId);
            pluginManager.AddPlugin(model, pluginMock.Object);

            //追加前は0
            var l1 = model.GetData(new RequestConnectionIds()) as ResponseConnectionIds;
            Assert.AreEqual(0, l1.Ids.Length);

            //追加
            model.SetRequest(pluginId, new RequestAddConnection());

            //追加後は1
            var l2 = model.GetData(new RequestConnectionIds()) as ResponseConnectionIds;
            Assert.AreEqual(1, l2.Ids.Length);
        }
        [Test]
        public void Connectionを削除する()
        {
            var pluginManager = new PluginManager();
            var sitePluginManager = new Mock<ISitePluginManager>().Object;
            var pluginId = new PluginId();
            var loggerMock = new Mock<ILogger>();
            var ioMock = new Mock<IIo>();
            var coreOptionsMock = new Mock<ICoreOptions>();
            var userStoreMock = new Mock<mcv2.Model.IUserStore>();
            var model = new Model(pluginManager, sitePluginManager, userStoreMock.Object, loggerMock.Object, ioMock.Object, coreOptionsMock.Object);

            var pluginMock = new Mock<IMcvPluginV1>();
            pluginMock.Setup(x => x.Id).Returns(pluginId);
            pluginManager.AddPlugin(model, pluginMock.Object);

            //追加
            model.SetRequest(pluginId, new RequestAddConnection());

            //追加後は1
            var l2 = model.GetData(new RequestConnectionIds()) as ResponseConnectionIds;
            Assert.AreEqual(1, l2.Ids.Length);

            var connId = l2.Ids[0];
            model.SetRequest(pluginId, new RequestRemoveConnection(connId));

            //削除後は0
            var l3 = model.GetData(new RequestConnectionIds()) as ResponseConnectionIds;
            Assert.AreEqual(0, l3.Ids.Length);
        }
    }
}
