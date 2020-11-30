using mcv2;
using mcv2.MainViewPlugin;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace mcv2MainViewPluginTests
{
    [TestFixture]
    class UserTests
    {
        [Test]
        public void Test()
        {
            var siteId = new SitePluginId(Guid.NewGuid());
            var userId = "";
            var hostMock = new Mock<IPluginHost>();
            hostMock.Setup(h => h.LoadOptions(It.IsAny<string>())).Returns("");
            var host = hostMock.Object;
            var modelMock = new Mock<mcv2.MainViewPlugin.Plugin> { CallBase = true };
            var model = modelMock.Object;
            modelMock.Protected().Setup<string>("GetSettingsFilePath").Returns("");
            modelMock.Setup(m => m.ShowSettingView()).Callback(() => { });
            var vm = new MainViewModel(model, model, new DynamicOptionsTest());
            modelMock.Setup(m => m.CreateMainViewModel(It.IsAny<IModel>(), It.IsAny<IConnectionModel>(), It.IsAny<DynamicOptionsTest>())).Returns(vm);

            model.Host = host;
            model.OnLoaded();
            model.SetNotify(new NotifyUserAdded(siteId, userId));
            //vm.Usersを作ったらテストする
            var a = vm;
        }
    }
}
