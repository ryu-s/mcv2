using mcv2.MainViewPlugin;
using Moq;
using NUnit.Framework;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeLiveSitePlugin;

namespace mcv2MainViewPluginTests
{
    [TestFixture]
    class Class1
    {
        [Test]
        public void ニックネームを付けたら名前が切り替わるか()
        {
            var commentMock = new Mock<IYouTubeLiveComment>();
            commentMock.Setup(x => x.NameItems).Returns(Common.MessagePartFactory.CreateMessageItems("name"));
            var connVmMock = new Mock<IMainViewConnectionStatus>();
            var userMock = new Mock<IUserViewModel>();
            userMock.Setup(x => x.UsernameItems).Returns(Common.MessagePartFactory.CreateMessageItems("name"));
            var optionsMock = new Mock<IOptions>();
            var ytOptions = new YouTubeLiveSiteOptions();
            var siteContextGuid = new SitePluginId(Guid.NewGuid());
            var vm = new McvYouTubeLiveCommentViewModel2(commentMock.Object, connVmMock.Object, userMock.Object, optionsMock.Object, ytOptions, siteContextGuid);

            Assert.AreEqual(Common.MessagePartFactory.CreateMessageItems("name"), vm.NameItems);

            userMock.Setup(x => x.Nickname).Returns("nick");
            userMock.Raise(c => c.PropertyChanged += null, new PropertyChangedEventArgs(nameof(userMock.Object.Nickname)));

            Assert.AreEqual(Common.MessagePartFactory.CreateMessageItems("nick"), vm.NameItems);
        }
    }
}
