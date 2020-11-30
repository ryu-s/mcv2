using SitePlugin;
using System.Collections.Generic;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class McvLiveLiveViewModelBase : McvCommentViewModelBase
    {
        private readonly LineLiveSiteOptions _ytOptions;

        public override SolidColorBrush Background
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.LineLiveBackColor);
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
                    return new SolidColorBrush(Options.LineLiveForeColor);
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
        public McvLiveLiveViewModelBase(IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, LineLiveSiteOptions ytOptions, SitePluginId siteContextGuid)
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
    class McvLiveLiveCommentViewModel : McvLiveLiveViewModelBase
    {
        public McvLiveLiveCommentViewModel(LineLiveSitePlugin.ILineLiveComment comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, LineLiveSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = Common.MessagePartFactory.CreateMessageItems(comment.Text);
            UserId = comment.UserId.ToString();
            //Id = comment..Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
        }
    }
    class McvLiveLiveItemViewModel : McvLiveLiveViewModelBase
    {
        public McvLiveLiveItemViewModel(LineLiveSitePlugin.ILineLiveItem comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, LineLiveSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = comment.CommentItems;
            UserId = comment.UserId.ToString();
            //Id = comment..Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
        }
    }
}
