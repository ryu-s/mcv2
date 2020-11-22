using SitePlugin;
using System.Collections.Generic;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class McvOpenrecViewModelBase : McvCommentViewModelBase
    {
        private readonly OpenrecSiteOptions _ytOptions;

        public override SolidColorBrush Background
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.OpenrecBackColor);
                }
                else if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Connection)
                {
                    return new SolidColorBrush(ConnectionStatus.BackColor);
                }
                else
                {
                    return new SolidColorBrush(Options.BackColor);
                }
            }
        }
        public override SolidColorBrush Foreground
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.OpenrecForeColor);
                }
                else if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Connection)
                {
                    return new SolidColorBrush(ConnectionStatus.ForeColor);
                }
                else
                {
                    return new SolidColorBrush(Options.ForeColor);
                }
            }
        }
        public override IEnumerable<IMessagePart> NameItems
        {
            get
            {
                if (!string.IsNullOrEmpty(User.Nickname))
                {
                    return Common.MessagePartFactory.CreateMessageItems(User.Nickname);
                }
                else
                {
                    return User.UsernameItems;
                }
            }
        }
        protected IEnumerable<IMessagePart> ProtectedMessageItems { get; set; }
        public override IEnumerable<IMessagePart> MessageItems => ProtectedMessageItems;
        public McvOpenrecViewModelBase(IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, OpenrecSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(options, user, connVm, siteContextGuid)
        {
            _ytOptions = ytOptions;

            ytOptions.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    default:
                        break;
                }
            };
        }
    }
    class McvOpenrecCommentViewModel : McvOpenrecViewModelBase
    {
        public McvOpenrecCommentViewModel(OpenrecSitePlugin.IOpenrecComment comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, OpenrecSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = comment.MessageItems;
            //UserId = comment.UserId;
            //Id = comment.Id;
            PostTime = comment.PostTime.ToString("HH:mm:ss");
        }
    }
    class McvOpenrecStampViewModel : McvOpenrecViewModelBase
    {
        public McvOpenrecStampViewModel(OpenrecSitePlugin.IOpenrecStamp comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, OpenrecSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            var list = new List<IMessagePart>
            {
                 Common.MessagePartFactory.CreateMessageText(comment.Message),
                 comment.Stamp,
            };
            ProtectedMessageItems = list;
            //UserId = comment.UserId;
            //Id = comment.Id;
            PostTime = comment.PostTime.ToString("HH:mm:ss");
        }
    }
    class McvOpenrecYellViewModel : McvOpenrecViewModelBase
    {
        public McvOpenrecYellViewModel(OpenrecSitePlugin.IOpenrecYell comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, OpenrecSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            var list = new List<IMessagePart>
            {
                Common.MessagePartFactory.CreateMessageText("エールポイント：" + comment.YellPoints + System.Environment.NewLine),
                Common.MessagePartFactory.CreateMessageText(comment.Message),
            };
            ProtectedMessageItems = list;
            //UserId = comment.UserId;
            Id = comment.Id;
            PostTime = comment.PostTime.ToString("HH:mm:ss");
        }
    }
}
