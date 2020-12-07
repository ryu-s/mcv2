using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace mcv2.MainViewPlugin
{
    /// <summary>
    /// MainView上でのみ使用する各ユーザのデータ
    /// </summary>
    class MainViewUserData
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public string UserId { get; }

        public MainViewUserData(string userId)
        {
            UserId = userId;
        }
    }
    interface IUserHost
    {
        bool GetIsNgUser(SitePluginId siteContextGuid, string userId);
        void SetIsNgUser(SitePluginId siteContextGuid, string userId, bool value);
        bool GetIsSiteNgUser(SitePluginId siteContextGuid, string userId);
        void SetIsSiteNgUser(SitePluginId siteContextGuid, string userId, bool value);
        string GetNickname(SitePluginId siteContextGuid, string userId);
        void SetNickname(SitePluginId siteContextGuid, string userId, string? value);
        void SendError(Exception ex, string message, string rawData);
    }
    public class ConnectionData
    {
        public ConnectionId Id { get; }
        public bool IsSelected { get; set; }
        public SitePluginId SelectedSite { get; set; }
        public string Input { get; set; }
        public string Name { get; set; }
        public Guid SelectedBrowser { get; set; }
        public bool NeedSave { get; set; }
        public string LoggedInUserName { get; set; }

        public ConnectionData(ConnectionId id)
        {
            Id = id;
        }
    }
    [Export(typeof(IMcvPluginV1))]
    public class Plugin : IMcvPluginV1, IModel, IUserHost, IConnectionModel
    {
        MainViewModel _vm;
        public string Name => "MainView";
        public IPluginHost Host { get; set; }
        public PluginId Id { get; } = new PluginId();
        ///PluginId IModel.Id { get; }

        readonly Dictionary<ConnectionId, ConnectionData> _connDict = new Dictionary<ConnectionId, ConnectionData>();

        DynamicOptionsTest _options;
        UserStore _userStore;
        Dispatcher _dispatcher;
        internal virtual MainViewModel CreateMainViewModel(IModel model, IConnectionModel connectionModel, DynamicOptionsTest options)
        {
            return new MainViewModel(model, connectionModel, options);
        }
        internal virtual DynamicOptionsTest CreateOptions()
        {
            return new DynamicOptionsTest();
        }
        public void OnLoaded()
        {
            var optionsStr = Host.LoadOptions(GetSettingsFilePath());
            _options = CreateOptions();
            _options.Deserialize(optionsStr);

            _userStore = new UserStore(_options, this);

            _dispatcher = Dispatcher.CurrentDispatcher;

            _vm = CreateMainViewModel(this, this, _options);
            if (Host.GetData(new RequestLoadedPlugins()) is ResponseLoadedPlugins resPlugins)
            {
                foreach (var (pluginId, pluginName) in resPlugins.Plugins.Select(x => (x.PluginId, x.Name)))
                {
                    AddPluginToMenuItem(pluginId, pluginName);
                }
            }
            else
            {
                Host.SetError("Host.GetData(IRequest)がRequestLoadedPluginsを正常に処理できていない");
            }
            ShowSettingView();
        }
        private void AddPluginToMenuItem(PluginId pluginId, string pluginName)
        {
            if (Id == pluginId)
            {
                return;
            }
            _vm.AddPlugin(pluginId, pluginName);
        }
        public void OnClosing()
        {

        }
        protected virtual string GetSettingsFilePath()
        {
            var dir = Host.SettingsDirPath;
            string name;
            if (File.Exists(Path.Combine(dir, $"options.txt")))
            {
                name = "options";
            }
            else
            {
                name = $"{this.Name}";
            }
            return Path.Combine(dir, $"{name}.txt");
        }
        public void SetNotify(INotify notify)
        {
            switch (notify)
            {
                case NotifyAddConnection addConn:
                    var conn = new ConnectionData(addConn.ConnectionId)
                    {
                        IsSelected = false,
                        Name = addConn.Name,
                        Input = addConn.Input,
                        SelectedSite = addConn.SelectedSite,
                        SelectedBrowser = addConn.SelectedBrowser,
                        LoggedInUserName = addConn.LoggedInUserName,
                    };
                    _connDict.Add(addConn.ConnectionId, conn);
                    //_vm.AddNewConnection(addConn.ConnectionId);//, addConn.Name, addConn.Input, addConn.SelectedSite, addConn.SelectedBrowser);
                    _vm.AddNewConnection(conn);
                    break;
                case NotifyConnectionStatusChanged connChanged:
                    _vm.UpdateConnectionStatus(connChanged);
                    break;
                case NotifyRemoveConnection removeConn:
                    var connId = removeConn.ConnectionId;
                    _connDict.Remove(connId);
                    _vm.RemoveConnection(connId);
                    break;
                case NotifySiteAdded siteAdded:
                    var id = siteAdded.SiteId;
                    _vm.AddSite(siteAdded.SiteId, siteAdded.SiteName);
                    break;
                case NotifyBrowserAdded browserAdded:
                    _vm.AddBrowser(browserAdded.BrowserId, browserAdded.BrowserName, browserAdded.ProfileName);
                    break;
                case NotifyMessageReceived messageReceived:
                    try
                    {
                        //TODO:Userがnullの場合にも対応しないと。
                        UserViewModel? user;
                        if (messageReceived.User == null)
                        {
                            user = null;
                        }
                        else
                        {
                            user = _userStore.GetOrCreateUser(messageReceived.Metadata.SiteContextGuid, messageReceived.User.Id);
                            user.Nickname = messageReceived.User.Nickname;
                            user.UsernameItems = messageReceived.User.Name;
                        }
                        _dispatcher.Invoke(() =>
                        {
                            _vm.AddMessage(messageReceived.ConnectionId, messageReceived.Message, messageReceived.Metadata, user);
                        });

                    }
                    catch (Exception _ex)
                    {
                        throw;
                    }
                    break;
                case NotifyMetadataUpdated metadataUpdated:
                    _vm.UpdateMetadata(metadataUpdated.ConnectionId, metadataUpdated.Name, metadataUpdated.Metadata);
                    break;
                case NotifyPluginAdded pluginAdded:
                    AddPluginToMenuItem(pluginAdded.PluginId, pluginAdded.PluginName);
                    break;
                case NotifyUserAdded userAdded:
                    AddUser(userAdded.SiteContextGuid, userAdded.UserId);
                    break;
                case NotifyUserChanged userChanged:
                    ChangeUser(userChanged.SiteId, userChanged.UserId, userChanged.Name, userChanged.Nickname, userChanged.IsNgUser, userChanged.IsSiteNgUser);
                    break;
                case NotifyCloseApp _:
                    var optionsStr = _options.Serialize();
                    Host.SaveOptions(GetSettingsFilePath(), optionsStr);

                    //save Connections
                    var toSaveConnections = _connDict.Values.Where(x => x.NeedSave).ToList();
                    break;
            }
            NotifyReceived?.Invoke(this, notify);
        }
        public event EventHandler<INotify> NotifyReceived;
        public void SetResponse(IResponse res)
        {
            //Debug.WriteLine($"Plugin::SetResponse {res}");
            //switch (res)
            //{
            //    case ResponseConnectionStatusAdded connSt:
            //        {
            //            //_vm.SetConnectionStatus(connSt.ConnectionId, connSt.Name, connSt.Input, connSt.Site, connSt.Browser);
            //            _vm.UpdateConnectionStatus(connSt);
            //        }
            //        break;
            //    default:
            //        Debug.WriteLine($"MainViewPlugin::Plugin::SetResponse() 未対応:{res}");
            //        break;
            //}

        }

        void IModel.SendRequest(IRequest req)
        {
            Host.SetRequest(Id, req);
        }

        IResponse IModel.GetData(IRequest req)
        {
            return Host.GetData(req);
        }
        string IModel.GetCoreVersion()
        {
            var ret = Host.GetData(new RequestCoreVersion());
            if (ret is ResponseCoreVersion version)
            {
                return version.Version;
            }
            else
            {
                return null;
            }
        }
        string IModel.GetCoreBuildType()
        {
            var ret = Host.GetData(new RequestCoreBuildType());
            if (ret is ResponseCoreBuildType version)
            {
                return version.BuildType;
            }
            //throw new BugException("Host.GetData(new RequestCoreBuildType()) returns null");
            return "";
        }
        public async Task<(string? version, string? url)> GetLatestVersionAsync()
        {
            var ret = await Host.GetDataAsync(new RequestLatestVersion());
            if (ret is ResponseLatestVersion version)
            {
                return (version.Version, version.Url);
            }
            else
            {
                return (null, null);
            }
        }

        Task IModel.RequestUpdateCoreAsync(string latestVersionUrl)
        {
            return Host.SetRequestAsync(Id, new RequestUpdateCoreAsync(latestVersionUrl));
        }
        void IModel.RequestShowPluginSettingView(PluginId id)
        {
            var ret = Host.GetData(new RequestShowPluginSettingView(id));
            return;
        }

        public virtual void ShowSettingView()
        {
            var window = new MainWindow
            {
                DataContext = _vm
            };
            window.Show();
        }

        public void SendError(Exception ex, string message, string rawData)
        {
            throw new NotImplementedException();
        }
        public void RemoveSelectedConnection()
        {
        }
        private void ChangeUser(SitePluginId siteContextGuid, string userId, IEnumerable<IMessagePart>? name, string? nickname, bool? isNgUser, bool? isSiteNgUser)
        {
            var user = _userStore.GetOrCreateUser(siteContextGuid, userId);
            if (name != null)
            {
                user.UsernameItems = name;
            }
            if (nickname != null)
            {
                user.Nickname = nickname;
            }
            if (isNgUser.HasValue)
            {
                user.IsNgUser = isNgUser.Value;
            }
            if (isSiteNgUser.HasValue)
            {
                user.IsSiteNgUser = isSiteNgUser.Value;
            }
        }
        private void AddUser(SitePluginId siteContextGuid, string userId)
        {
            var user = _userStore.GetOrCreateUser(siteContextGuid, userId);
        }

        UserViewModel IModel.GetUser(SitePluginId siteContextGuid, string userId)
        {
            return _userStore.GetOrCreateUser(siteContextGuid, userId);
        }
        bool IUserHost.GetIsNgUser(SitePluginId siteContextGuid, string userId)
        {
            var ret = Host.GetData(new RequestUser(siteContextGuid, userId)) as ResponseUser;
            if (ret == null)
            {
                throw new BugException();
            }
            var user = ret.User;
            return user.IsNgUser;
        }

        void IUserHost.SetIsNgUser(SitePluginId siteContextGuid, string userId, bool value)
        {
            Host.SetRequest(Id, new RequestChangeUserStatus(siteContextGuid, userId)
            {
                IsNgUser = value,
            });
        }

        bool IUserHost.GetIsSiteNgUser(SitePluginId siteContextGuid, string userId)
        {
            var ret = Host.GetData(new RequestUser(siteContextGuid, userId)) as ResponseUser;
            var user = ret.User;
            return user.IsSiteNgUser;
        }

        void IUserHost.SetIsSiteNgUser(SitePluginId siteContextGuid, string userId, bool value)
        {
            Host.SetRequest(Id, new RequestChangeUserStatus(siteContextGuid, userId)
            {
                IsSiteNgUser = value,
            });
        }

        string IUserHost.GetNickname(SitePluginId siteContextGuid, string userId)
        {
            var ret = Host.GetData(new RequestUser(siteContextGuid, userId)) as ResponseUser;
            var user = ret.User;
            return user.Nickname;
        }

        void IUserHost.SetNickname(SitePluginId siteContextGuid, string userId, string? value)
        {
            if (value == null) return;
            Host.SetRequest(Id, new RequestChangeUserStatus(siteContextGuid, userId)
            {
                Nickname = value,
            });
        }

        public Task RequestUpdateCoreAsync(string latestVersionUrl)
        {
            return Host.SetRequestAsync(Id, new RequestUpdateCoreAsync(latestVersionUrl));
        }

        SiteType IModel.GetSiteType(SitePluginId siteId)
        {
            var res = Host.GetData(new GetSiteType(siteId)) as ResponseSiteType;
            if (res == null)
            {
                return SiteType.Unknown;
            }
            else
            {
                return res.SiteType;
            }
        }
        Task IModel.PostCommentAsync(ICommentDataToPost commentData)
        {
            if (_currentConnectionId == null)
            {
                throw new BugException($"{_currentConnectionId}がnullの時はコメント投稿できない");
            }
            return Host.SetRequestAsync(Id, new RequestPostCommentAsync(_currentConnectionId, commentData));
        }
        public Task SendRequestAsync(IRequestAsync req)
        {
            return Host.SetRequestAsync(Id, req);
        }
        ConnectionId? _currentConnectionId;
        ConnectionId? IModel.GetCurrentConnection()
        {
            return _currentConnectionId;
        }

        void IModel.SetCurrentConnection(ConnectionId connectionId)
        {
            _currentConnectionId = connectionId;
        }

        void IConnectionModel.SetIsSelected(ConnectionId id, bool value)
        {
            var conn = _connDict[id];
            conn.IsSelected = value;
        }

        bool IConnectionModel.GetIsSelected(ConnectionId id)
        {
            var conn = _connDict[id];
            return conn.IsSelected;
        }
        void IConnectionModel.SetNeedSave(ConnectionId id, bool value)
        {
            var conn = _connDict[id];
            conn.NeedSave = value;
        }

        bool IConnectionModel.GetNeedSave(ConnectionId id)
        {
            var conn = _connDict[id];
            return conn.NeedSave;
        }
        public ConnectionData GetConnection(ConnectionId connectionId)
        {
            return _connDict[connectionId];
        }

        void IModel.SendError(Exception ex, string message, string rawData)
        {
            throw new NotImplementedException();
        }



        void IModel.RemoveSelectedConnection()
        {
            var selectedConnections = _connDict.Values.Where(x => x.IsSelected).ToList();
            foreach (var toRemove in selectedConnections)
            {
                Host.SetRequest(Id, new RequestRemoveConnection(toRemove.Id));
            }
        }

        SitePluginId? IModel.GetSelectedSite(ConnectionId connectionId)
        {
            var conn = GetConnection(connectionId);
            return conn.SelectedSite;
        }

        Guid? IModel.GetSelectedBrowser(ConnectionId connectionId)
        {
            var conn = GetConnection(connectionId);
            return conn.SelectedBrowser;
        }
    }


}
