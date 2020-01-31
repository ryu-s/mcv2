using GalaSoft.MvvmLight;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    abstract class McvCommentViewModelBase : ViewModelBase, IMcvCommentViewModel
    {
        protected IOptions Options { get; }
        protected IUserViewModel? User { get; }
        public abstract SolidColorBrush Background { get; }
        public abstract SolidColorBrush Foreground { get; }
        public IMainViewConnectionStatus ConnectionStatus { get; }
        public FontFamily FontFamily => Options.FontFamily;
        public int FontSize => Options.FontSize;
        public FontStyle FontStyle => Options.FontStyle;
        public FontWeight FontWeight => Options.FontWeight;
        public string Id { get; protected set; }
        public string Info { get; protected set; }
        public bool IsVisible
        {
            get
            {
                if (User == null)
                {
                    return true;
                }
                else
                {
                    return !User.IsNgUser && !User.IsSiteNgUser;
                }
            }
        }
        public abstract IEnumerable<IMessagePart> MessageItems { get; }
        public abstract IEnumerable<IMessagePart> NameItems { get; }
        public string PostTime { get; protected set; }
        public IMessageImage Thumbnail { get; protected set; }
        public string UserId { get; protected set; }
        public TextWrapping UserNameWrapping
        {
            get
            {
                if (Options.IsUserNameWrapping)
                {
                    return TextWrapping.Wrap;
                }
                else
                {
                    return TextWrapping.NoWrap;
                }
            }
        }

        public SitePluginId SiteContextGuid { get; }
        public McvCommentViewModelBase(IOptions options, IMainViewConnectionStatus connVm, SitePluginId siteContextGuid)
        {
            Options = options;
            ConnectionStatus = connVm;
            SiteContextGuid = siteContextGuid;
            options.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(options.BackColor):
                        RaisePropertyChanged(nameof(Background));
                        break;
                    case nameof(options.ForeColor):
                        RaisePropertyChanged(nameof(Foreground));
                        break;
                    case nameof(options.FontFamily):
                        RaisePropertyChanged(nameof(FontFamily));
                        break;
                    case nameof(options.FontStyle):
                        RaisePropertyChanged(nameof(FontStyle));
                        break;
                    case nameof(options.FontWeight):
                        RaisePropertyChanged(nameof(FontWeight));
                        break;
                    case nameof(options.FontSize):
                        RaisePropertyChanged(nameof(FontSize));
                        break;
                    case nameof(options.IsUserNameWrapping):
                        RaisePropertyChanged(nameof(UserNameWrapping));
                        break;
                    case nameof(options.IsEnabledSiteConnectionColor):
                        RaisePropertyChanged(nameof(Background));
                        RaisePropertyChanged(nameof(Foreground));
                        break;
                }
            };
            connVm.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ConnectionStatus.BackColor):
                        RaisePropertyChanged(nameof(Background));
                        break;
                    case nameof(ConnectionStatus.ForeColor):
                        RaisePropertyChanged(nameof(Foreground));
                        break;
                }
            };
        }
        public McvCommentViewModelBase(IOptions options, IUserViewModel user, IMainViewConnectionStatus connVm, SitePluginId siteContextGuid)
            : this(options, connVm, siteContextGuid)
        {
            User = user;
            user.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(user.UsernameItems):
                        RaisePropertyChanged(nameof(NameItems));
                        break;
                    case nameof(user.Nickname):
                        RaisePropertyChanged(nameof(NameItems));
                        break;
                    case nameof(user.IsNgUser):
                        RaisePropertyChanged(nameof(IsVisible));
                        break;
                    case nameof(user.IsSiteNgUser):
                        RaisePropertyChanged(nameof(IsVisible));
                        break;
                }
            };
        }
    }
}
