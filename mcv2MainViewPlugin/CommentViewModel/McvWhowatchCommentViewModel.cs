using SitePlugin;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using WhowatchSitePlugin;

namespace mcv2.MainViewPlugin
{
    internal class McvWhowatchCommentViewModel : McvCommentViewModelBase
    {
        private WhowatchSiteOptions _whowatchOptions;

        public McvWhowatchCommentViewModel(IWhowatchComment comment, ConnectionViewModel connVm, UserViewModel user, IOptions options, WhowatchSiteOptions whowatchOptions, SitePluginId siteContextGuid)
            : base(options, user, connVm, siteContextGuid)
        {
            _whowatchOptions = whowatchOptions;
            MessageItems = Common.MessagePartFactory.CreateMessageItems(comment.Comment);
            UserId = comment.UserId;
            Id = comment.Id;
            PostTime = comment.PostTime;
        }

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
        public override IEnumerable<IMessagePart> MessageItems { get; }
        public override IEnumerable<IMessagePart> NameItems { get; }
    }
}
