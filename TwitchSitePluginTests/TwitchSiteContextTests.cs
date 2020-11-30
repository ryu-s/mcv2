﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TwitchSitePlugin;
using Moq;
using SitePlugin;
using Common;
using SitePluginCommon;

namespace TwitchSitePluginTests
{
    [TestFixture]
    class TwitchSiteContextTests
    {
        [Test]
        public void TwitchSite_IsValdUrlTest()
        {
            var loggerMock = new Mock<ILogger>();
            var serverMock = new Mock<IDataServer>();
            var webSocket = new Mock<IMessageProvider>();
            var site = new TwitchSiteContext2(serverMock.Object, loggerMock.Object);
            Assert.IsTrue(site.IsValidInput("https://www.twitch.tv/abc"));
            Assert.IsTrue(site.IsValidInput("https://www.twitch.tv/abc?"));
            Assert.IsTrue(site.IsValidInput("https://www.twitch.tv/abc/"));
            Assert.IsFalse(site.IsValidInput("https://www.twitch.tv/"));
            Assert.IsFalse(site.IsValidInput("https://www.twitch.tv/?"));
        }
    }
}
