using GalaSoft.MvvmLight;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using TwicasSitePlugin;
using TwitchSitePlugin;

namespace mcv2.MainViewPlugin
{

    internal class McvTwicasCommentViewModel : McvCommentViewModelBase
    {
        private TwicasSiteOptions _twitchOptions;

        public McvTwicasCommentViewModel(ITwicasComment twComment, ConnectionViewModel connVm, UserViewModel user, IOptions options, TwicasSiteOptions twitchOptions, SitePluginId siteContextGuid)
            : base(options, user, connVm, siteContextGuid)
        {
            _twitchOptions = twitchOptions;
        }

        public override SolidColorBrush Background
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.TwicasBackColor);
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
                    return new SolidColorBrush(Options.TwicasForeColor);
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
    internal class McvTwitchCommentViewModel : McvCommentViewModelBase
    {
        private TwitchSiteOptions _twitchOptions;

        public McvTwitchCommentViewModel(ITwitchComment twComment, ConnectionViewModel connVm, UserViewModel user, IOptions options, TwitchSiteOptions twitchOptions, SitePluginId siteContextGuid)
            : base(options, user, connVm, siteContextGuid)
        {
            _twitchOptions = twitchOptions;
        }

        public override SolidColorBrush Background
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.TwitchBackColor);
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
                    return new SolidColorBrush(Options.TwitchForeColor);
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
