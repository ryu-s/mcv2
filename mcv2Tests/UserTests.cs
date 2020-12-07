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
    class TestPlugin : IMcvPluginV1
    {
        public event EventHandler<INotify>? NotifyReceived;
        public PluginId Id { get; } = new PluginId();
        public IPluginHost Host { get; set; }
        public string Name { get; } = "";

        public void OnClosing()
        {
        }

        public void OnLoaded()
        {
        }

        public void SetNotify(INotify notify)
        {
            NotifyReceived?.Invoke(this, notify);
        }

        public void SetResponse(IResponse res)
        {
        }

        public void ShowSettingView()
        {
        }
    }
    [TestFixture]
    class UserTests
    {
        [Test]
        public void UserSerializeDeserializeTest()
        {
            var siteId = new SitePluginId(Guid.NewGuid());
            var userId = "abc";
            var user = new McvUser(siteId, userId)
            {
                IsNgUser = true,
                IsSiteNgUser = true,
                Name = new List<IMessagePart>
                  {
                      new MessageImage
                      {
                          Alt = "alt",
                          Height = 12,
                          Width = 88,
                          Url = "https://int-main.net",
                          X = 79,
                          Y = 51,
                      },
                      Common.MessagePartFactory.CreateMessageText("oak"),
                  },
                Nickname = "nick",
            };
            var str = McvUser.Serialize(user);
            var deserialized = McvUser.Deserialize(str);
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(user.Guid, deserialized!.Guid);
            Assert.AreEqual(user.Id, deserialized.Id);
            Assert.AreEqual(user.IsNgUser, deserialized.IsNgUser);
            Assert.AreEqual(user.IsSiteNgUser, deserialized.IsSiteNgUser);
            Assert.AreEqual(user.Name, deserialized.Name);
            Assert.AreEqual(user.Nickname, deserialized.Nickname);
        }
        private Model _model;
        private TestPlugin _testPlugin;
        [SetUp]
        public void Setup()
        {
            var sitePluginManager = new Mock<ISitePluginManager>().Object;
            var userStore = new TestUserStore();
            var loggerMock = new Mock<ILogger>();
            var ioMock = new Mock<IIo>();
            var optionsMock = new Mock<ICoreOptions>();
            _testPlugin = new TestPlugin();
            var pluginManager = new PluginManager();
            _model = new Model(pluginManager, sitePluginManager, userStore, loggerMock.Object, ioMock.Object, optionsMock.Object);
            pluginManager.AddPlugin(_model, _testPlugin);
        }
        [Test]
        public void ユーザー情報を変更したら通知が来るか()
        {
            //Arrange
            var siteId = new SitePluginId(Guid.NewGuid());
            var userId = "userid";
            var newNick = "abc";
            var b = false;
            _testPlugin.NotifyReceived += (s, e) =>
            {
                if (e is NotifyUserChanged changed && changed.Nickname == newNick)
                {
                    b = true;
                }
            };

            //Act
            _model.SetRequest(_testPlugin.Id, new RequestChangeUserStatus(siteId, userId)
            {
                Nickname = newNick,
            });

            //Assert
            Assert.IsTrue(b);
        }
        [Test]
        public void ニックネームを変更する()
        {
            var siteGuid = new SitePluginId(new Guid());
            var userId = "userid";
            var res = _model.GetData(new RequestUser(siteGuid, userId)) as ResponseUser;

            Assert.IsNotNull(res);
            Assert.IsNull(res!.User.Nickname);//ニックネームが設定されていない事を確認

            //Arrange
            _model.SetRequest(new PluginId(), new RequestChangeUserStatus(siteGuid, userId)
            {
                Nickname = "nick",
            });

            //Assert
            var res2 = _model.GetData(new RequestUser(siteGuid, userId)) as ResponseUser;
            Assert.IsNotNull(res2);
            Assert.AreEqual("nick", res2!.User.Nickname);
        }
        [Test]
        public void NGユーザーにする()
        {
            var siteGuid = new SitePluginId(new Guid());
            var userId = "userid";

            //Arrange
            _model.SetRequest(new PluginId(), new RequestChangeUserStatus(siteGuid, userId)
            {
                IsNgUser = true,
            });

            //Assert
            var res2 = (ResponseUser)_model.GetData(new RequestUser(siteGuid, userId));
            Assert.IsTrue(res2.User.IsNgUser);
        }
        [Test]
        public void サイトNGユーザーにする()
        {
            var siteGuid = new SitePluginId(new Guid());
            var userId = "userid";

            //Arrange
            _model.SetRequest(new PluginId(), new RequestChangeUserStatus(siteGuid, userId)
            {
                IsSiteNgUser = true,
            });

            //Assert
            var res = (ResponseUser)_model.GetData(new RequestUser(siteGuid, userId));
            Assert.IsTrue(res.User.IsSiteNgUser);
        }
    }
}
