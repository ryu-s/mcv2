﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LineLiveSitePlugin;
using SitePlugin;
using Moq;
using Common;
using ryu_s.BrowserCookie;
using System.Net;
using SitePluginCommon;

namespace LineLiveSitePluginTests
{
    [TestFixture]
    class Class2
    {
        [Test]
        public async Task Test()
        {
            var serverMock = new Mock<IDataServer>();
            serverMock.Setup(s => s.GetAsync("https://live-api.line-apps.com/web/v2.5/billing/gift/loves")).Returns(Task.FromResult(TestHelper.GetSampleData("Loves.txt")));
            serverMock.Setup(s => s.GetAsync("https://live-api.line-apps.com/app/v2/channel/2577702/broadcast/8680302")).Returns(Task.FromResult(TestHelper.GetSampleData("LiveInfo.txt")));
            var loggerMock = new Mock<ILogger>();
            var browserProfileMock = new Mock<IBrowserProfile2>();
            browserProfileMock.Setup(b => b.GetCookieCollection(It.IsAny<string>())).Returns(new List<Cookie>());
            var context = new LineLiveSitePlugin.LineLiveSiteContext(serverMock.Object, loggerMock.Object);

            var commentProvider = context.CreateCommentProvider();

            commentProvider.MessageReceived += (s, e) =>
            {

            };
            await commentProvider.ConnectAsync("https://live-api.line-apps.com/app/v2/channel/2577702/broadcast/8680302", browserProfileMock.Object);



        }

    }
}
