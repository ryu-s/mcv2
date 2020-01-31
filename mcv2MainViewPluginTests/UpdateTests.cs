using mcv2MainViewPlugin.Update;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcv2MainViewPluginTests
{
    [TestFixture]
    class ToolsTests
    {
        [TestCase(true, "1")]
        [TestCase(true, "1.2.3")]
        [TestCase(true, "99.102.306")]
        [TestCase(false, "")]
        [TestCase(false, "v1.2.3")]
        public void IsValidVersionNumberTest(bool expected, string version)
        {
            Assert.AreEqual(expected, Tools.IsValidVersionNumber(version));
        }
        [TestCase(true, "0", "1")]
        [TestCase(true, "0.0.1", "0.0.2")]
        [TestCase(true, "10.100.1000", "10.101.2")]
        [TestCase(false, "0.0.1", "0.0.1")]
        [TestCase(false, "0.0.10", "0.0.1")]
        [TestCase(false, "", "0.0.1")]
        [TestCase(false, "0.0.1", "")]
        [TestCase(true, "0.1","0.1.1")]
        public void Test(bool expected, string aVersion, string bVersion)
        {
            Assert.AreEqual(expected, Tools.NeedUpdate(aVersion, bVersion));
        }
    }
}
