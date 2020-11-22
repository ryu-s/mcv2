using SitePlugin;
using System.Collections.Generic;
using System.Windows.Media;
namespace mcv2.MainViewPlugin
{
    class McvMildomViewModelBase : McvCommentViewModelBase
    {
        private readonly MildomSiteOptions _ytOptions;

        public override SolidColorBrush Background
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.BigoBackColor);
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
                    return new SolidColorBrush(Options.BigoForeColor);
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
        public McvMildomViewModelBase(IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, MildomSiteOptions ytOptions, SitePluginId siteContextGuid)
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
    class McvMildomCommentViewModel : McvMildomViewModelBase
    {
        public McvMildomCommentViewModel(MildomSitePlugin.IMildomComment comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, MildomSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = comment.CommentItems;
            UserId = comment.UserId;
            //Id = comment.Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
        }
    }
    class McvMildomGiftViewModel : McvMildomViewModelBase
    {
        public McvMildomGiftViewModel(MildomSitePlugin.IMildomGift comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, MildomSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            //TODO:どういう表示にすればいいのか
            //ProtectedMessageItems = comment.GiftName;
            //UserId = comment..UserId;
            //Id = comment.Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
        }
    }
    class McvMildomJoinViewModel : McvMildomViewModelBase
    {
        public McvMildomJoinViewModel(MildomSitePlugin.IMildomJoinRoom comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, MildomSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = comment.CommentItems;
            UserId = comment.UserId;
            //Id = comment.Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
        }
    }
}
