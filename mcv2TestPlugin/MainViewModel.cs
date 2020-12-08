using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using mcv2;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TwitchSitePlugin;
using WhowatchSitePlugin;
using YouTubeLiveIF;
using YouTubeLiveSitePlugin;

namespace mcv2TestPlugin
{
    class YouTubeLiveComment : IYouTubeLiveComment
    {
        public IEnumerable<IMessagePart> NameItems { get; set; }
        public IEnumerable<IMessagePart> CommentItems { get; set; }
        public IMessageImage UserIcon { get; set; }
        public DateTime PostedAt { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
        public YouTubeLiveMessageType YouTubeLiveMessageType { get; set; } = YouTubeLiveMessageType.Comment;
        public string Raw { get; } = "";
        public SiteType SiteType { get; } = SiteType.YouTubeLive;
        public string Info { get; }
        public UserType UserType { get; }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        public YouTubeLiveComment(string name, string comment, DateTime postedAt, string id, string userId)
        {
            NameItems = Common.MessagePartFactory.CreateMessageItems(name);
            CommentItems = Common.MessagePartFactory.CreateMessageItems(comment);
            PostedAt = postedAt;
            Id = id;
            UserId = userId;
            Info = "";
            UserType = new Member();
        }
    }
    class YouTubeLiveSuperChat : IYouTubeLiveSuperchat
    {
        public IEnumerable<IMessagePart> NameItems { get; }
        public IEnumerable<IMessagePart> CommentItems { get; }
        public IMessageImage UserIcon { get; }
        public DateTime PostedAt { get; }
        public string Id { get; }
        public string UserId { get; }
        public string PurchaseAmount { get; }
        public YouTubeLiveMessageType YouTubeLiveMessageType { get; } = YouTubeLiveMessageType.Superchat;
        public string Raw { get; }
        public SiteType SiteType { get; } = SiteType.YouTubeLive;

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        public YouTubeLiveSuperChat(string name, string comment, DateTime postedAt, string id, string userId, string purchaseAmount)
        {
            NameItems = Common.MessagePartFactory.CreateMessageItems(name);
            CommentItems = Common.MessagePartFactory.CreateMessageItems(comment);
            PostedAt = postedAt;
            Id = id;
            UserId = userId;
            PurchaseAmount = purchaseAmount;
        }
    }
    class TwitchComment : ITwitchComment
    {
        public string Id { get; }
        public bool IsDisplayNameSame => DisplayName == UserName;
        public string DisplayName { get; }
        public string UserName { get; }
        public IEnumerable<IMessagePart> CommentItems { get; }
        public string PostTime { get; }
        public string UserId { get; }
        public IMessageImage UserIcon { get; }
        public TwitchMessageType TwitchMessageType { get; } = TwitchMessageType.Comment;
        public string Raw { get; }
        public SiteType SiteType { get; } = SiteType.Twitch;

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        public TwitchComment(string name, string comment, DateTime postedAt, string id, string userId)
        {
            UserName = name;
            DisplayName = name;

            CommentItems = Common.MessagePartFactory.CreateMessageItems(comment);
            PostTime = postedAt.ToString("HH:mm:ss");
            Id = id;
            UserId = userId;
        }
    }
    class WhowatchComment : IWhowatchComment
    {
        public string UserName { get; }
        public string Comment { get; }
        public string UserPath { get; }
        public string UserId { get; }
        public string Id { get; }
        public DateTime PostedAt { get; }
        public IMessageImage UserIcon { get; }
        public string AccountName { get; }
        public WhowatchMessageType WhowatchMessageType { get; } = WhowatchMessageType.Comment;
        public string Raw { get; }
        public SiteType SiteType { get; } = SiteType.Whowatch;

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        public WhowatchComment(string name, string comment, DateTime postedAt, string id, string userId)
        {
            UserName = name;
            UserPath = name;

            Comment = comment;
            PostedAt = postedAt;
            Id = id;
            UserId = userId;
        }
    }
    class Metadata : IMessageMetadata2
    {
        public bool IsNgUser { get; set; }
        public bool IsSiteNgUser { get; set; }
        public bool IsFirstComment { get; set; }
        public bool IsInitialComment { get; set; }
        public bool Is184 { get; set; }
        public SitePluginId SiteContextGuid { get; set; }
        public string UserId { get; set; }
        public IEnumerable<IMessagePart> UserName { get; set; }
        public SiteType SiteType => SiteType.Unknown;

        public string? NewNickname { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    class YouTubeLiveCommentInput
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Input { get; set; }
        public void Clear()
        {
            var props = typeof(YouTubeLiveCommentInput).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.SetMethod is null) continue;
                prop.SetValue(this, "");
            }
        }
    }
    class YouTubeLiveSuperChatInput
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Input { get; set; }
        public string PurchaseAmount { get; set; }
        public void Clear()
        {
            var props = typeof(YouTubeLiveSuperChatInput).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.SetMethod is null) continue;
                prop.SetValue(this, "");
            }
        }
    }
    class TwitchCommentInput
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Input { get; set; }
        public void Clear()
        {
            var props = typeof(TwitchCommentInput).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.SetMethod is null) continue;
                prop.SetValue(this, "");
            }
        }
    }
    class WhowatchCommentInput
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Input { get; set; }
        public void Clear()
        {
            var props = typeof(WhowatchCommentInput).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.SetMethod is null) continue;
                prop.SetValue(this, "");
            }
        }
    }
    class UserViewModel : ViewModelBase
    {
        private readonly IPluginMain _host;
        public string Id { get; }
        public SitePluginId SiteId { get; }
        public string? Nickname
        {
            get
            {
                var res = (ResponseUser)_host.GetData(new RequestUser(SiteId, Id));
                return res.User.Nickname;
            }
            set
            {
                _host.SetRequest(new RequestChangeUserStatus(SiteId, Id)
                {
                    Nickname = value,
                });
                RaisePropertyChanged();
            }
        }
        public IEnumerable<IMessagePart> Name
        {
            get
            {
                var res = (ResponseUser)_host.GetData(new RequestUser(SiteId, Id));
                return res.User.Name;
            }
            set
            {
                _host.SetRequest(new RequestChangeUserStatus(SiteId, Id)
                {
                    Name = value,
                });
                RaisePropertyChanged();
            }
        }
        public bool IsNgUser
        {
            get
            {
                var res = (ResponseUser)_host.GetData(new RequestUser(SiteId, Id));
                return res.User.IsNgUser;
            }
            set
            {
                _host.SetRequest(new RequestChangeUserStatus(SiteId, Id)
                {
                    IsNgUser = value,
                });
                RaisePropertyChanged();
            }
        }
        public bool IsSiteNgUser
        {
            get
            {
                var res = (ResponseUser)_host.GetData(new RequestUser(SiteId, Id));
                return res.User.IsSiteNgUser;
            }
            set
            {
                _host.SetRequest(new RequestChangeUserStatus(SiteId, Id)
                {
                    IsSiteNgUser = value,
                });
                RaisePropertyChanged();
            }
        }
        public UserViewModel(SitePluginId siteId, string userId, IPluginMain host)
        {
            Id = userId;
            SiteId = siteId;

            //Nickname = mcvUser.Nickname;
            _host = host;
        }
    }
    class MainViewModel : ViewModelBase
    {
        private readonly IPluginMain _pluginMain;
        private ConnectionViewModel? _selectedConnection;
        public ObservableCollection<UserViewModel> Users { get; } = new ObservableCollection<UserViewModel>();
        private Dictionary<(SitePluginId, string), UserViewModel> _userDict = new Dictionary<(SitePluginId, string), UserViewModel>();
        public void AddUser(IMcvUser mcvUser)
        {
            var user = new UserViewModel(mcvUser.SiteId, mcvUser.Id, _pluginMain);
            _userDict.Add((mcvUser.SiteId, mcvUser.Id), user);
            Users.Add(user);
        }
        public void ChangeUser(IMcvUserDiff diff)
        {
            var user = _userDict[(diff.SiteId, diff.UserId)];
            if (diff.Nickname != null)
            {
                user.Nickname = diff.Nickname;
            }
            if (diff.Name != null)
            {
                user.Name = diff.Name;
            }
            if (diff.IsNgUser != null)
            {
                user.IsNgUser = diff.IsNgUser.Value;
            }
            if (diff.IsSiteNgUser != null)
            {
                user.IsSiteNgUser = diff.IsSiteNgUser.Value;
            }
        }
        public ConnectionViewModel? SelectedConnection
        {
            get
            {
                return _selectedConnection;
            }
            set
            {
                _selectedConnection = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanPostYouTubeComment));
            }
        }
        public YouTubeLiveCommentInput YouTubeLiveCommentInput { get; } = new YouTubeLiveCommentInput();
        public YouTubeLiveSuperChatInput YouTubeLiveSuperChatInput { get; } = new YouTubeLiveSuperChatInput();
        public TwitchCommentInput TwitchCommentInput { get; } = new TwitchCommentInput();
        public WhowatchCommentInput WhowatchCommentInput { get; } = new WhowatchCommentInput();
        public ICommand PostYouTubeCommentCommand { get; }
        public ICommand PostYouTubeSuperChatCommand { get; }
        public ICommand PostTwitchCommentCommand { get; }
        public ICommand PostWhowatchCommentCommand { get; }
        public ICommand AddNewConnectionCommand { get; }
        public MainViewModel(IPluginMain pluginMain)
        {
            PostYouTubeCommentCommand = new RelayCommand(PostYouTubeComment);
            PostYouTubeSuperChatCommand = new RelayCommand(PostYouTubeSuperChat);
            PostTwitchCommentCommand = new RelayCommand(PostTwitchComment);
            PostWhowatchCommentCommand = new RelayCommand(PostWhowatchComment);
            AddNewConnectionCommand = new RelayCommand(AddConnection);
            _pluginMain = pluginMain;
        }
        private void AddConnection()
        {
            var req = new RequestAddConnection();
            _pluginMain.SetRequest(req);
        }
        private void PostYouTubeComment()
        {

            var connId = SelectedConnection.ConnectionId;
            var input = YouTubeLiveCommentInput.Input;
            var userId = YouTubeLiveCommentInput.UserId;
            var name = YouTubeLiveCommentInput.UserName;
            var commentId = Guid.NewGuid().ToString();
            var message = new YouTubeLiveComment(name, input, DateTime.Now, commentId, userId);
            var isNgUser = false;
            var isSiteNgUser = false;
            var metadata = new Metadata()
            {
                SiteContextGuid = SelectedConnection.SiteGuid,
                UserId = userId,
                UserName = Common.MessagePartFactory.CreateMessageItems(name),
                IsNgUser = isNgUser,
                IsSiteNgUser = isSiteNgUser,
            };
            _pluginMain.PostComment(connId, message, metadata);
        }
        private void PostYouTubeSuperChat()
        {
            var connId = SelectedConnection.ConnectionId;
            var input = YouTubeLiveSuperChatInput.Input;
            var userId = YouTubeLiveSuperChatInput.UserId;
            var name = YouTubeLiveSuperChatInput.UserName;
            var amount = YouTubeLiveSuperChatInput.PurchaseAmount;
            var commentId = Guid.NewGuid().ToString();
            var message = new YouTubeLiveSuperChat(name, input, DateTime.Now, commentId, userId, amount);
            var isNgUser = false;
            var isSiteNgUser = false;
            var metadata = new Metadata()
            {
                SiteContextGuid = SelectedConnection.SiteGuid!,//Metadataを取得するようなタイミングならSitePluginIdがnullなんてことはない
                UserId = userId,
                UserName = Common.MessagePartFactory.CreateMessageItems(name),
                IsNgUser = isNgUser,
                IsSiteNgUser = isSiteNgUser,
            };
            _pluginMain.PostComment(connId, message, metadata);
        }
        public bool CanPostYouTubeComment
        {
            get
            {
                return SelectedConnection != null;
            }
        }
        private void PostTwitchComment()
        {
            var connId = SelectedConnection.ConnectionId;
            var input = TwitchCommentInput.Input;
            var userId = TwitchCommentInput.UserId;
            var name = TwitchCommentInput.UserName;
            var commentId = Guid.NewGuid().ToString();
            var message = new TwitchComment(name, input, DateTime.Now, commentId, userId);
            var isNgUser = false;
            var isSiteNgUser = false;
            var metadata = new Metadata()
            {
                SiteContextGuid = SelectedConnection.SiteGuid!,
                UserId = userId,
                UserName = Common.MessagePartFactory.CreateMessageItems(name),
                IsNgUser = isNgUser,
                IsSiteNgUser = isSiteNgUser,
            };
            _pluginMain.PostComment(connId, message, metadata);
        }
        private void PostWhowatchComment()
        {
            var connId = SelectedConnection.ConnectionId;
            var input = WhowatchCommentInput.Input;
            var userId = WhowatchCommentInput.UserId;
            var name = WhowatchCommentInput.UserName;
            var commentId = Guid.NewGuid().ToString();
            var message = new WhowatchComment(name, input, DateTime.Now, commentId, userId);
            var isNgUser = false;
            var isSiteNgUser = false;
            var metadata = new Metadata()
            {
                SiteContextGuid = SelectedConnection.SiteGuid!,
                UserId = userId,
                UserName = Common.MessagePartFactory.CreateMessageItems(name),
                IsNgUser = isNgUser,
                IsSiteNgUser = isSiteNgUser,
            };
            _pluginMain.PostComment(connId, message, metadata);
        }
        public ObservableCollection<ConnectionViewModel> Connections { get; } = new ObservableCollection<ConnectionViewModel>();
        private Dictionary<ConnectionId, ConnectionViewModel> _connDict = new Dictionary<ConnectionId, ConnectionViewModel>();
        public void AddConnection(ConnectionId connectionId, string displayName, SitePluginId? siteGuid)
        {
            var conn = new ConnectionViewModel(connectionId)
            {
                DisplayName = displayName,
                SiteGuid = siteGuid,
            };
            Connections.Add(conn);
            _connDict.Add(connectionId, conn);
        }
        public void RemoveConnection(ConnectionId connectionId)
        {
            var conn = _connDict[connectionId];
            Connections.Remove(conn);
            _connDict.Remove(connectionId);
        }
        public void ChangeConnectionName(ConnectionId connectionId, string? newDisplayName, SitePluginId? siteGuid)
        {
            var conn = _connDict[connectionId];
            if (newDisplayName != null)
            {
                conn.DisplayName = newDisplayName;
            }
            if (siteGuid != null)
            {
                conn.SiteGuid = siteGuid;
            }
        }
    }
}
