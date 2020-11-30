using System.Collections.Generic;
using SitePlugin;
using System.Windows.Media;
namespace mcv2.MainViewPlugin
{
    class McvMirrativViewModelBase : McvCommentViewModelBase
    {
        private readonly MirrativSiteOptions _ytOptions;

        public override SolidColorBrush Background
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.MirrativBackColor);
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
                    return new SolidColorBrush(Options.MirrativForeColor);
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
        public McvMirrativViewModelBase(IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, MirrativSiteOptions ytOptions, SitePluginId siteContextGuid)
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
    class McvMirrativCommentViewModel : McvMirrativViewModelBase
    {
        public McvMirrativCommentViewModel(MirrativSitePlugin.IMirrativComment comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, MirrativSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = Common.MessagePartFactory.CreateMessageItems(comment.Text);
            UserId = comment.UserId;
            Id = comment.Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
        }
    }
    class McvMirrativJoinViewModel : McvMirrativViewModelBase
    {
        public McvMirrativJoinViewModel(MirrativSitePlugin.IMirrativJoinRoom comment,
    IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, MirrativSiteOptions ytOptions, SitePluginId siteContextGuid)
    : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = Common.MessagePartFactory.CreateMessageItems(comment.Text);
            UserId = comment.UserId;
            Id = comment.Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
        }
    }
    class McvMirrativItemViewModel : McvMirrativViewModelBase
    {
        public McvMirrativItemViewModel(MirrativSitePlugin.IMirrativItem mirItem,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, MirrativSiteOptions mirrativSiteOptions, SitePluginId siteContextGuid)
              : base(connVm, user, options, mirrativSiteOptions, siteContextGuid)
        {
            ProtectedMessageItems = Common.MessagePartFactory.CreateMessageItems(mirItem.Text);
            UserId = mirItem.UserId;
            Id = mirItem.Id;
            PostTime = mirItem.PostedAt.ToString("HH:mm:ss");
        }
    }
}
