﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Moq;
using NUnit.Framework;
using ryu_s.BrowserCookie;
using SitePlugin;
using SitePluginCommon;
using TwitchSitePlugin;

namespace TwitchSitePluginTests
{
    [TestFixture]
    class TwitchCommentProviderTests
    {
        class C : TwitchCommentProvider2
        {
            public bool IsLoggedin { get; set; }
            protected override bool IsLoggedIn()
            {
                return IsLoggedin;
            }
            protected override string GetChannelName(string input)
            {
                return "";
            }
            protected override CookieContainer GetCookieContainer(IBrowserProfile2 browserProfile)
            {
                return new CookieContainer();
            }
            protected override string GetRandomGuestUsername()
            {
                return "";
            }
            public IMessageProvider MessageProvider { get; set; }
            protected override IMessageProvider CreateMessageProvider()
            {
                return MessageProvider;
            }
            public IMetadataProvider MetadataProvider { get; set; }
            protected override IMetadataProvider CreateMetadataProvider(string channelName)
            {
                return MetadataProvider;
            }
            public ICommentData CommentData { get; set; }
            //protected override ICommentData ParsePrivMsg(Result result)
            //{
            //    return CommentData;
            //}
            public C(IDataServer server, ILogger logger, TwitchSiteOptions siteOptions)
                : base(server, logger, siteOptions)
            {
            }
        }
        class MessageProvider : IMessageProvider
        {
            public event EventHandler<string> Received;
            public event EventHandler Opened;
            TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
            public void Disconnect()
            {
                _tcs.SetResult(null);
            }

            public Task ReceiveAsync()
            {
                return _tcs.Task;
            }

            public Task SendAsync(string s)
            {
                throw new NotImplementedException();
            }
            public void SetResult(string raw)
            {
                Received?.Invoke(this, raw);
            }
        }
        class MetadataProvider : IMetadataProvider
        {
            public event EventHandler<Stream> MetadataUpdated;

            public void Disconnect()
            {
            }

            public Task ReceiveAsync()
            {
                return Task.CompletedTask;
            }
        }
        [Test]
        public async Task Test()
        {
            //テスト案
            //ログイン済み、未ログインそれぞれの場合にそれぞれの接続コマンドが送信されるか
            //サーバから送られてくるコマンドに対する反応は適切か。PINGの時はPONGが返されるか、PRIVMSGだったらCommentReceivedが発生するか
            var data = TestHelper.GetSampleData("Streams.txt");
            var raw = "@badges=subscriber/12,partner/1;color=#FF0000;display-name=harutomaru;emotes=189031:20-31,60-71/588715:33-58/635709:73-82;id=9029a587-81b0-4705-8607-38cba9b762d6;mod=0;room-id=39587048;subscriber=1;tmi-sent-ts=1518062412116;turbo=0;user-id=72777405;user-type= :harutomaru!harutomaru@harutomaru.tmi.twitch.tv PRIVMSG #mimicchi :@bwscar221 おざまぁぁぁす！ mimicchiHage haruto1Harutomarubakayarou mimicchiHage bwscarDead";
            var userid = "72777405";
            var serverMock = new Mock<IDataServer>();
            serverMock.Setup(s => s.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).Returns(Task.FromResult(data));
            var loggerMock = new Mock<ILogger>();
            var siteOptions = new TwitchSiteOptions
            {
                NeedAutoSubNickname = true
            };
            var browserProfileMock = new Mock<IBrowserProfile2>();
            var messageProvider = new MessageProvider();
            var commentDataMock = new Mock<ICommentData>();
            var commentProvider = new C(serverMock.Object, loggerMock.Object, siteOptions)
            {
                MessageProvider = messageProvider,
                MetadataProvider = new MetadataProvider(),
                CommentData = commentDataMock.Object,
            };
            IMessageContext2 actual = null;
            commentProvider.MessageReceived += (s, e) =>
            {
                actual = e;
                commentProvider.Disconnect();
            };
            var t = commentProvider.ConnectAsync("", browserProfileMock.Object);
            messageProvider.SetResult(raw);
            await t;
            var comment = actual.Message as ITwitchComment;
            Assert.AreEqual(TwitchMessageType.Comment, comment.TwitchMessageType);
            Assert.AreEqual("9029a587-81b0-4705-8607-38cba9b762d6", comment.Id);
            return;
        }
        [Test]
        public async Task 自動コテハン登録が機能するか()
        {
            var data = TestHelper.GetSampleData("Streams.txt");
            var raw = "@badges=subscriber/12,partner/1;color=#FF0000;display-name=harutomaru;id=9029a587-81b0-4705-8607-38cba9b762d6;mod=0;room-id=39587048;subscriber=1;tmi-sent-ts=1518062412116;turbo=0;user-id=72777405;user-type= :harutomaru!harutomaru@harutomaru.tmi.twitch.tv PRIVMSG #mimicchi :あいう @コテハン えお";
            var userid = "72777405";
            var serverMock = new Mock<IDataServer>();
            serverMock.Setup(s => s.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).Returns(Task.FromResult(data));
            var loggerMock = new Mock<ILogger>();
            var siteOptions = new TwitchSiteOptions
            {
                NeedAutoSubNickname = true
            };
            var browserProfileMock = new Mock<IBrowserProfile2>();
            var messageProvider = new MessageProvider();
            var commentDataMock = new Mock<ICommentData>();
            var commentProvider = new C(serverMock.Object, loggerMock.Object, siteOptions)
            {
                MessageProvider = messageProvider,
                MetadataProvider = new MetadataProvider(),
                CommentData = commentDataMock.Object,
            };
            IMessageContext2 actual = null;
            commentProvider.MessageReceived += (s, e) =>
            {
                actual = e;
                commentProvider.Disconnect();
            };
            var t = commentProvider.ConnectAsync("", browserProfileMock.Object);
            messageProvider.SetResult(raw);
            await t;
            //Assert.AreEqual("コテハン", user.Nickname);
            //2020/11/05、自動コテハン機能未実装
            Assert.Fail();
        }
        [Test]
        public async Task GetCurrentUserInfoTest()
        {
            var serverMock = new Mock<IDataServer>();
            var server = serverMock.Object;

            var loggerMock = new Mock<ILogger>();
            var logger = loggerMock.Object;

            var siteOptions = new TwitchSiteOptions
            {
                NeedAutoSubNickname = true
            };

            var browserMock = new Mock<IBrowserProfile2>();
            var cookies = new List<Cookie>
            {
                new Cookie("login","abc","/","twitch.tv"),
                new Cookie("twilight-user","{%22authToken%22:%22rkpavglsbv6ovec0qj2l5r5q0mnlm4%22%2C%22displayName%22:%22aokpz%22%2C%22id%22:%22223620888%22%2C%22login%22:%22kv501k%22%2C%22roles%22:{%22isStaff%22:false}%2C%22version%22:2}","/","twitch.tv"),
            };
            browserMock.Setup(b => b.GetCookieCollection(It.IsAny<string>())).Returns(cookies);
            var browser = browserMock.Object;

            var cp = new TwitchCommentProvider2(server, logger, siteOptions);
            var info = await cp.GetCurrentUserInfo(browser);
            Assert.AreEqual("aokpz", info.Username);
            Assert.IsTrue(info.IsLoggedIn);

        }
    }
}
