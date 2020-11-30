using mcv2.MainViewPlugin;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace mcv2MainViewPluginTests
{
    [TestFixture]
    public class YouTubeLiveSiteOptionsTests
    {
        [Test]
        public void YouTubeLiveSiteOptionsSetTest()
        {
            var youTubeLiveSiteOptions = new YouTubeLiveSiteOptions
            {
                IsAllChat = true,
                IsAutoSetNickname = true,
                PaidCommentBackColor = Colors.Red,
                PaidCommentForeColor = Colors.Red,
            };
            var copied = new YouTubeLiveSiteOptionsCopy
            {
                IsAllChat = false,
                IsAutoSetNickname = false,
                PaidCommentBackColor = Colors.Pink,
                PaidCommentForeColor = Colors.Pink,
            };
            youTubeLiveSiteOptions.Set(copied);
            Assert.IsFalse(youTubeLiveSiteOptions.IsAllChat);
            Assert.IsFalse(youTubeLiveSiteOptions.IsAutoSetNickname);
            Assert.AreEqual(Colors.Pink, youTubeLiveSiteOptions.PaidCommentBackColor);
            Assert.AreEqual(Colors.Pink, youTubeLiveSiteOptions.PaidCommentForeColor);
        }
    }
}
