using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using YouTubeLiveSitePlugin;

namespace YouTubeLiveSitePluginTests
{
    [TestFixture]
    class DataCreatorTests
    {
        [Test]
        public void Test()
        {
            var ytInitialData = Tools.GetSampleData("YtInitialData_loggedin.txt");
            var s = new DataCreator(ytInitialData, "", new System.Net.CookieContainer());
            Assert.AreEqual("Q2pnS0RRb0xZblZvV2tOek9Vc3pVR01xSndvWVZVTXRhRTAyV1VwMVRsbFdRVzFWVjNobFNYSTVSbVZCRWd0aWRXaGFRM001U3pOUVl4QUNHQVElM0Q=", s.GetParams());
        }
        [Test]
        public void Test1()
        {
            var ytInitialData = Tools.GetSampleData("YtInitialData_loggedin.txt");
            var s = new DataCreator(ytInitialData, "", new System.Net.CookieContainer());
            Assert.IsTrue(s.IsLoggedIn());
        }
        [Test]
        public void Test2()
        {
            var ytInitialData = Tools.GetSampleData("YtInitialData_notloggedin.txt");
            var s = new DataCreator(ytInitialData, "", new System.Net.CookieContainer());
            Assert.IsFalse(s.IsLoggedIn());
        }
        [Test]
        public void CreateSapiSidTest()
        {
            var cc = new CookieContainer();
            cc.Add(new Cookie("SAPISID", "COtU1Ybg06yul57D/Ao67jPtKJQlXS7tqq", "/", "youtube.com"));
            var ytInitialData = Tools.GetSampleData("YtInitialData_loggedin.txt");
            var sMock = new Mock<DataCreator>(ytInitialData, "", cc)
            {
                CallBase = true,
            };
            sMock.Setup(x => x.GetCurrentUnixTime()).Returns(1607248074);
            var s = sMock.Object;
            Assert.AreEqual("1607248074_f4eefda6be8cc175f9c681605e62e8ca3cd2beeb", s.CreateHash());
        }
        [Test]
        public void Test4()
        {
            var ytInitialData = Tools.GetSampleData("YtInitialData_loggedin.txt");
            var cc = new CookieContainer();
            cc.Add(new Cookie("SAPISID", "COtU1Ybg06yul57D/Ao67jPtKJQlXS7tqq", "/", "youtube.com"));
            var s = new DataCreator(ytInitialData, "", cc);
            s.Create("abc");
        }
    }
}
