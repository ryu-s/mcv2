using System;
using System.Collections.Generic;
using System.Text;
using mcv2;
using mcv2.MainViewPlugin;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SitePlugin;

namespace mcv2MainViewPluginTests
{
    [TestFixture]
    class UserTests
    {
        [Test]
        public void NotifyUserAddedが来たらユーザーが追加される()
        {
            var pluginHostMock = new Mock<IPluginHost>();
            var pluginHost = pluginHostMock.Object;
            var pluginMock = new Mock<Plugin>() { CallBase = true };
            var plugin = pluginMock.Object;
            var vm = new MainViewModel(plugin, plugin, new DynamicOptionsTest());
            pluginMock.Setup(x => x.CreateMainViewModel(It.IsAny<IModel>(), It.IsAny<IConnectionModel>(), It.IsAny<DynamicOptionsTest>())).Returns(vm);
            pluginMock.Protected().Setup<string>("GetSettingsFilePath").Returns("");
            pluginMock.Setup(m => m.ShowSettingView()).Callback(() => { });
            plugin.Host = pluginHost;
            plugin.OnLoaded();

            //Arrange
            var siteId = new SitePluginId(Guid.NewGuid());
            var userId = "abc";
            plugin.SetNotify(new NotifyUserAdded(siteId, userId));

            //Assert
            Assert.AreEqual(vm.Users[0].UserId, userId);
        }
        [Test]
        public void ニックネームが変更されたら変更が反映されてPropertyChangedも発生する()
        {
            var siteId = new SitePluginId(Guid.NewGuid());
            var userId = "abc";
            var userMock = new Mock<IMcvUser>();

            var pluginHostMock = new Mock<IPluginHost>();
            pluginHostMock.Setup(x => x.GetData(It.IsAny<RequestUser>())).Returns(new ResponseUser(new RequestId(), userMock.Object));
            var pluginHost = pluginHostMock.Object;
            var pluginMock = new Mock<Plugin>() { CallBase = true };
            var plugin = pluginMock.Object;
            var vm = new MainViewModel(plugin, plugin, new DynamicOptionsTest());
            pluginMock.Setup(x => x.CreateMainViewModel(It.IsAny<IModel>(), It.IsAny<IConnectionModel>(), It.IsAny<DynamicOptionsTest>())).Returns(vm);
            pluginMock.Protected().Setup<string>("GetSettingsFilePath").Returns("");
            pluginMock.Setup(m => m.ShowSettingView()).Callback(() => { });
            plugin.Host = pluginHost;
            plugin.OnLoaded();

            //Arrange
            var b = false;
            var newNick = "newnick";
            userMock.Setup(u => u.Nickname).Returns(newNick);
            plugin.SetNotify(new NotifyUserAdded(siteId, userId));
            var user = vm.Users[0];
            user.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(user.Nickname))
                {
                    b = true;
                }
            };
            plugin.SetNotify(new NotifyUserChanged(siteId, userId)
            {
                Nickname = newNick,
            });

            //Assert
            Assert.AreEqual(user.Nickname, newNick);
            Assert.IsTrue(b);
        }
    }
    [TestFixture]
    class UserStoreTests
    {
        [Test]
        public void 同じIDが渡されたら同じユーザーを返す()
        {
            var optionsMock = new Mock<IOptions>();
            var hostMock = new Mock<IUserHost>();
            var store = new UserStore(optionsMock.Object, hostMock.Object);
            var siteId = new SitePluginId(Guid.NewGuid());
            var userId = "abc";
            var user1 = store.GetOrCreateUser(siteId, userId);
            var user2 = store.GetOrCreateUser(siteId, userId);
            Assert.AreEqual(user1, user2);
        }
    }
}
