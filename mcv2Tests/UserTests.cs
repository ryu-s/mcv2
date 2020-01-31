using Common;
using mcv2;
using mcv2.Model;
using Moq;
using NUnit.Framework;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcv2Tests
{
    [TestFixture]
    class UserTests
    {
        [Test]
        public void ニックネームを変更する()
        {
            var sitePluginManager = new Mock<ISitePluginManager>().Object;
            var userStore = new TestUserStore();
            var loggerMock = new Mock<ILogger>();
            var ioMock = new Mock<IIo>();
            var optionsMock = new Mock<ICoreOptions>();
            var model = new Model(new PluginManager(), sitePluginManager, userStore, loggerMock.Object, ioMock.Object, optionsMock.Object);
            var siteGuid = new SitePluginId(new Guid());
            var userId = "userid";
            var res = model.GetData(new RequestUser(siteGuid, userId)) as ResponseUser;

            Assert.IsNotNull(res);
            Assert.IsNull(res.User.Nickname);

            model.SetRequest(new PluginId(), new RequestChangeUserStatus(siteGuid, userId)
            {
                Nickname = "nick",
            });

            var res2 = model.GetData(new RequestUser(siteGuid, userId)) as ResponseUser;
            Assert.AreEqual("nick", res2.User.Nickname);
        }
        [Test]
        public void NGユーザーにする()
        {
            var sitePluginManager = new Mock<ISitePluginManager>().Object;
            var userStore = new TestUserStore();
            var loggerMock = new Mock<ILogger>();
            var ioMock = new Mock<IIo>();
            var optionsMock = new Mock<ICoreOptions>();
            var model = new Model(new PluginManager(), sitePluginManager, userStore, loggerMock.Object, ioMock.Object, optionsMock.Object);
            var siteGuid = new SitePluginId(new Guid());
            var userId = "userid";
            var res = model.GetData(new RequestUser(siteGuid, userId)) as ResponseUser;

            Assert.IsNotNull(res);
            Assert.IsFalse(res.User.IsNgUser);

            model.SetRequest(new PluginId(), new RequestChangeUserStatus(siteGuid, userId)
            {
                IsNgUser = true,
            });

            var res2 = model.GetData(new RequestUser(siteGuid, userId)) as ResponseUser;
            Assert.IsTrue(res2.User.IsNgUser);
        }
        [Test]
        public void サイトNGユーザーにする()
        {
            var sitePluginManager = new Mock<ISitePluginManager>().Object;
            var userStore = new TestUserStore();
            var loggerMock = new Mock<ILogger>();
            var ioMock = new Mock<IIo>();
            var optionsMock = new Mock<ICoreOptions>();
            var model = new Model(new PluginManager(), sitePluginManager, userStore, loggerMock.Object, ioMock.Object, optionsMock.Object);
            var siteGuid = new SitePluginId(new Guid());
            var userId = "userid";
            var res = model.GetData(new RequestUser(siteGuid, userId)) as ResponseUser;

            Assert.IsNotNull(res);
            Assert.IsFalse(res.User.IsSiteNgUser);

            model.SetRequest(new PluginId(), new RequestChangeUserStatus(siteGuid, userId)
            {
                IsSiteNgUser = true,
            });

            var res2 = model.GetData(new RequestUser(siteGuid, userId)) as ResponseUser;
            Assert.IsTrue(res2.User.IsSiteNgUser);
        }
    }
}
