using SitePlugin;
using SitePluginCommon;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    internal class McvInfoCommentViewModel : McvCommentViewModelBase
    {

        public McvInfoCommentViewModel(InfoMessage info, ConnectionViewModel connVm, IOptions options, SitePluginId siteContextGuid)
            : base(options, connVm, siteContextGuid)
        {
            MessageItems = Common.MessagePartFactory.CreateMessageItems(info.Text);
            PostTime = info.CreatedAt.ToString("HH:mm:ss");
        }

        public override SolidColorBrush Background => new SolidColorBrush(Options.InfoBackColor);
        public override SolidColorBrush Foreground => new SolidColorBrush(Options.InfoForeColor);
        public override IEnumerable<IMessagePart> MessageItems { get; }
        public override IEnumerable<IMessagePart> NameItems { get; }
    }
}
