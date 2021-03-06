﻿using SitePlugin;
using System.Collections.Generic;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class McvNicoViewModelBase : McvCommentViewModelBase
    {
        private readonly NicoSiteOptions _ytOptions;

        public override SolidColorBrush Background
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.NicoLiveBackColor);
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
                    return new SolidColorBrush(Options.NicoLiveForeColor);
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
        public McvNicoViewModelBase(IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, NicoSiteOptions ytOptions, SitePluginId siteContextGuid)
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
    class McvNicoCommentViewModel : McvNicoViewModelBase
    {
        class McvNicoViewModelBase : McvCommentViewModelBase
        {
            private readonly NicoSiteOptions _ytOptions;

            public override SolidColorBrush Background
            {
                get
                {
                    if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                    {
                        return new SolidColorBrush(Options.NicoLiveBackColor);
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
                        return new SolidColorBrush(Options.NicoLiveForeColor);
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
            public McvNicoViewModelBase(IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, NicoSiteOptions ytOptions, SitePluginId siteContextGuid)
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
        public McvNicoCommentViewModel(NicoSitePlugin.INicoComment comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, NicoSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = Common.MessagePartFactory.CreateMessageItems(comment.Text);
            UserId = comment.UserId;
            Id = comment.Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
        }
    }
}
