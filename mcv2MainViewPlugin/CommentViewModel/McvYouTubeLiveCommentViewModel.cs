using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SitePlugin;
using System.Windows.Media;
using System.Windows;
using GalaSoft.MvvmLight;
using System.Linq;

namespace mcv2.MainViewPlugin
{
    class McvYouTubeLiveViewModelBase : McvCommentViewModelBase
    {
        private readonly YouTubeLiveSiteOptions _ytOptions;

        public override SolidColorBrush Background
        {
            get
            {
                if (Options.IsEnabledSiteConnectionColor && Options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(Options.YouTubeLiveBackColor);
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
                    return new SolidColorBrush(Options.YouTubeLiveForeColor);
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
        public McvYouTubeLiveViewModelBase(IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, YouTubeLiveSiteOptions ytOptions, SitePluginId siteContextGuid)
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
    class McvYouTubeLiveCommentViewModel2 : McvYouTubeLiveViewModelBase
    {
        public McvYouTubeLiveCommentViewModel2(YouTubeLiveSitePlugin.IYouTubeLiveComment comment,
            IMainViewConnectionStatus connVm, IUserViewModel user, IOptions options, YouTubeLiveSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            ProtectedMessageItems = comment.CommentItems;
            UserId = comment.UserId;
            Id = comment.Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
            string info;
            if (comment.UserType is YouTubeLiveIF.Member member)
            {
                info = member.TestInfo;
            }
            else
            {
                info = comment.UserType.TypeName;
            }
            Info = info;
        }
    }
    class McvYouTubeLiveSuperChatViewModel2 : McvYouTubeLiveViewModelBase
    {
        private readonly YouTubeLiveSiteOptions _ytOptions;

        public override SolidColorBrush Background
        {
            get
            {
                return new SolidColorBrush(_ytOptions.PaidCommentBackColor);
            }
        }
        public override SolidColorBrush Foreground
        {
            get
            {
                return new SolidColorBrush(_ytOptions.PaidCommentForeColor);
            }
        }
        public McvYouTubeLiveSuperChatViewModel2(YouTubeLiveSitePlugin.IYouTubeLiveSuperchat comment, ConnectionViewModel connVm, UserViewModel user, IOptions options, YouTubeLiveSiteOptions ytOptions, SitePluginId siteContextGuid)
            : base(connVm, user, options, ytOptions, siteContextGuid)
        {
            var list = new List<IMessagePart>();
            var s = comment.PurchaseAmount;
            if (comment.CommentItems.ToList().Count > 0)
                s += Environment.NewLine;
            list.Add(Common.MessagePartFactory.CreateMessageText(s));
            list.AddRange(comment.CommentItems);
            ProtectedMessageItems = list;

            UserId = comment.UserId;
            Id = comment.Id;
            PostTime = comment.PostedAt.ToString("HH:mm:ss");
            ytOptions.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ytOptions.PaidCommentBackColor):
                        RaisePropertyChanged(nameof(Background));
                        break;
                    case nameof(ytOptions.PaidCommentForeColor):
                        RaisePropertyChanged(nameof(Foreground));
                        break;
                }
            };
            _ytOptions = ytOptions;
        }
    }
    //class McvYouTubeLiveCommentViewModel : IMcvCommentViewModel
    //{
    //    private readonly YouTubeLiveSitePlugin.IYouTubeLiveMessage _message;
    //    private readonly IMessageMetadata _metadata;
    //    private readonly IMessageMethods _methods;
    //    private readonly IOptions _options;
    //    public Guid SiteContextGuid { get; }

    //    private void SetNickname(IUser user)
    //    {
    //        if (!string.IsNullOrEmpty(user.Nickname))
    //        {
    //            _nickItems = new List<IMessagePart> { Common.MessagePartFactory.CreateMessageText(user.Nickname) };
    //        }
    //        else
    //        {
    //            _nickItems = null;
    //        }
    //    }
    //    private McvYouTubeLiveCommentViewModel(IMessageMetadata metadata, IMessageMethods methods, IMainViewConnectionStatus connectionStatus, IOptions options)
    //    {
    //        _metadata = metadata;
    //        _methods = methods;

    //        ConnectionStatus = connectionStatus;
    //        _options = options;
    //        ConnectionStatus.PropertyChanged += (s, e) =>
    //        {
    //            switch (e.PropertyName)
    //            {
    //                case nameof(ConnectionStatus.Name):
    //                    RaisePropertyChanged(nameof(ConnectionStatus));
    //                    break;
    //                case nameof(ConnectionStatus.BackColor):
    //                    RaisePropertyChanged(nameof(Background));
    //                    break;
    //                case nameof(ConnectionStatus.ForeColor):
    //                    RaisePropertyChanged(nameof(Foreground));
    //                    break;
    //            }
    //        };
    //        options.PropertyChanged += (s, e) =>
    //        {
    //            switch (e.PropertyName)
    //            {
    //                case nameof(options.IsEnabledSiteConnectionColor):
    //                    RaisePropertyChanged(nameof(Background));
    //                    RaisePropertyChanged(nameof(Foreground));
    //                    break;
    //            }
    //        };
    //        _metadata.PropertyChanged += (s, e) =>
    //        {
    //            switch (e.PropertyName)
    //            {
    //                case nameof(_metadata.BackColor):
    //                    RaisePropertyChanged(nameof(Background));
    //                    break;
    //                case nameof(_metadata.ForeColor):
    //                    RaisePropertyChanged(nameof(Foreground));
    //                    break;
    //                case nameof(_metadata.FontFamily):
    //                    RaisePropertyChanged(nameof(FontFamily));
    //                    break;
    //                case nameof(_metadata.FontStyle):
    //                    RaisePropertyChanged(nameof(FontStyle));
    //                    break;
    //                case nameof(_metadata.FontWeight):
    //                    RaisePropertyChanged(nameof(FontWeight));
    //                    break;
    //                case nameof(_metadata.FontSize):
    //                    RaisePropertyChanged(nameof(FontSize));
    //                    break;
    //                case nameof(_metadata.IsNameWrapping):
    //                    RaisePropertyChanged(nameof(UserNameWrapping));
    //                    break;
    //                case nameof(_metadata.IsVisible):
    //                    RaisePropertyChanged(nameof(IsVisible));
    //                    break;
    //            }
    //        };
    //        if (_metadata.User != null)
    //        {
    //            var user = _metadata.User;
    //            user.PropertyChanged += (s, e) =>
    //            {
    //                switch (e.PropertyName)
    //                {
    //                    case nameof(user.Nickname):
    //                        SetNickname(user);
    //                        RaisePropertyChanged(nameof(NameItems));
    //                        break;
    //                }
    //            };
    //            SetNickname(user);
    //        }
    //    }
    //    public McvYouTubeLiveCommentViewModel(YouTubeLiveSitePlugin.IYouTubeLiveComment comment, IMessageMetadata metadata, IMessageMethods methods, IMainViewConnectionStatus connectionStatus, IOptions options, YouTubeLiveSiteOptions ytOptions)
    //        : this(metadata, methods, connectionStatus, options)
    //    {
    //        _message = comment;

    //        _nameItems = comment.NameItems;
    //        MessageItems = comment.CommentItems;
    //        Thumbnail = comment.UserIcon;
    //        Id = comment.Id.ToString();
    //        PostTime = comment.PostedAt.ToString("HH:mm:ss");
    //    }
    //    public McvYouTubeLiveCommentViewModel(YouTubeLiveSitePlugin.IYouTubeLiveSuperchat item, IMessageMetadata metadata, IMessageMethods methods, IMainViewConnectionStatus connectionStatus, IOptions options)
    //        : this(metadata, methods, connectionStatus, options)
    //    {
    //        var comment = item;
    //        _message = comment;

    //        _nameItems = comment.NameItems;
    //        MessageItems = comment.CommentItems;
    //        Thumbnail = comment.UserIcon;
    //        Id = comment.Id;
    //        PostTime = comment.PostedAt.ToString("HH:mm:ss");
    //    }
    //    public McvYouTubeLiveCommentViewModel(YouTubeLiveSitePlugin.IYouTubeLiveConnected connected, IMessageMetadata metadata, IMessageMethods methods, IMainViewConnectionStatus connectionStatus, IOptions options)
    //        : this(metadata, methods, connectionStatus, options)
    //    {
    //        _message = connected;
    //        MessageItems = Common.MessagePartFactory.CreateMessageItems(connected.Text);
    //    }
    //    public McvYouTubeLiveCommentViewModel(YouTubeLiveSitePlugin.IYouTubeLiveDisconnected disconnected, IMessageMetadata metadata, IMessageMethods methods, IMainViewConnectionStatus connectionStatus, IOptions options)
    //        : this(metadata, methods, connectionStatus, options)
    //    {
    //        _message = disconnected;
    //        MessageItems = Common.MessagePartFactory.CreateMessageItems(disconnected.Text);
    //    }

    //    public IMainViewConnectionStatus ConnectionStatus { get; }

    //    private IEnumerable<IMessagePart> _nickItems { get; set; }
    //    private IEnumerable<IMessagePart> _nameItems { get; set; }
    //    public IEnumerable<IMessagePart> NameItems
    //    {
    //        get
    //        {
    //            if (_nickItems != null)
    //            {
    //                return _nickItems;
    //            }
    //            else
    //            {
    //                return _nameItems;
    //            }
    //        }
    //    }

    //    public IEnumerable<IMessagePart> MessageItems { get; private set; }

    //    public SolidColorBrush Background
    //    {
    //        get
    //        {
    //            if (_options.IsEnabledSiteConnectionColor && _options.SiteConnectionColorType == SiteConnectionColorType.Site)
    //            {
    //                return new SolidColorBrush(_options.YouTubeLiveBackColor);
    //            }
    //            else if (_options.IsEnabledSiteConnectionColor && _options.SiteConnectionColorType == SiteConnectionColorType.Connection)
    //            {
    //                return new SolidColorBrush(ConnectionStatus.BackColor);
    //            }
    //            else
    //            {
    //                return new SolidColorBrush(_metadata.BackColor);
    //            }
    //        }
    //    }

    //    public ICommentProvider CommentProvider => _metadata.CommentProvider;

    //    public FontFamily FontFamily => _metadata.FontFamily;

    //    public int FontSize => _metadata.FontSize;

    //    public FontStyle FontStyle => _metadata.FontStyle;

    //    public FontWeight FontWeight => _metadata.FontWeight;

    //    public SolidColorBrush Foreground
    //    {
    //        get
    //        {
    //            if (_options.IsEnabledSiteConnectionColor && _options.SiteConnectionColorType == SiteConnectionColorType.Site)
    //            {
    //                return new SolidColorBrush(_options.YouTubeLiveForeColor);
    //            }
    //            else if (_options.IsEnabledSiteConnectionColor && _options.SiteConnectionColorType == SiteConnectionColorType.Connection)
    //            {
    //                return new SolidColorBrush(ConnectionStatus.ForeColor);
    //            }
    //            else
    //            {
    //                return new SolidColorBrush(_metadata.ForeColor);
    //            }
    //        }
    //    }

    //    public string Id { get; private set; }

    //    public string Info { get; private set; }

    //    public bool IsVisible => _metadata.IsVisible;

    //    public string PostTime { get; private set; }

    //    public IMessageImage Thumbnail { get; private set; }

    //    public string UserId => _metadata.User?.UserId;

    //    public TextWrapping UserNameWrapping
    //    {
    //        get
    //        {
    //            if (_metadata.IsNameWrapping)
    //            {
    //                return TextWrapping.Wrap;
    //            }
    //            else
    //            {
    //                return TextWrapping.NoWrap;
    //            }
    //        }
    //    }

    //    public Task AfterCommentAdded()
    //    {
    //        return Task.CompletedTask;
    //    }
    //    #region INotifyPropertyChanged
    //    [NonSerialized]
    //    private System.ComponentModel.PropertyChangedEventHandler _propertyChanged;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
    //    {
    //        add { _propertyChanged += value; }
    //        remove { _propertyChanged -= value; }
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="propertyName"></param>
    //    protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    //    {
    //        _propertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    //    }
    //    #endregion

    //    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    //    {
    //        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    //        return dt.AddSeconds(unixTimeStamp).ToLocalTime();
    //    }
    //}
}
