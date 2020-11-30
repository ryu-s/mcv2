using SitePlugin;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using WhowatchSitePlugin;

namespace mcv2.MainViewPlugin
{
    class McvWhowatchViewModelBase : McvCommentViewModelBase
    {
        private readonly WhowatchSiteOptions _ytOptions;

        public override SolidColorBrush Background
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.WhowatchBackColor);
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
                    return new SolidColorBrush(Options.WhowatchForeColor);
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
        public McvWhowatchViewModelBase(IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, WhowatchSiteOptions ytOptions, SitePluginId siteContextGuid)
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
    internal class McvWhowatchCommentViewModel : McvWhowatchViewModelBase
    {
        public McvWhowatchCommentViewModel(WhowatchSitePlugin.IWhowatchComment comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, WhowatchSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = Common.MessagePartFactory.CreateMessageItems(comment.Comment);
            UserId = comment.UserId;
            Id = comment.Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
        }
    }
    internal class McvWhowatchItemViewModel : McvWhowatchViewModelBase
    {
        private readonly WhowatchSiteOptions _ytOptions;

        public override SolidColorBrush Background
        {
            get
            {
                return new SolidColorBrush(_ytOptions.ItemBackColor);
            }
        }
        public override SolidColorBrush Foreground
        {
            get
            {
                return new SolidColorBrush(_ytOptions.ItemForeColor);
            }
        }
        public McvWhowatchItemViewModel(WhowatchSitePlugin.IWhowatchItem comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, WhowatchSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = Common.MessagePartFactory.CreateMessageItems(comment.Comment);
            UserId = comment.UserId.ToString();
            Id = comment.Id.ToString();
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
            Info = $"{comment.ItemName}×{comment.ItemCount}";
            _ytOptions = ytOptions;
        }
    }
}
