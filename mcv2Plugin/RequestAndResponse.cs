using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;

namespace mcv2
{
    public interface IRequest : IMessage
    {
        RequestId Id { get; }
    }
    public interface IRequestAsync : IMessage
    {
        RequestId Id { get; }
    }

    public interface IResponse : IMessage
    {
        RequestId RequestId { get; }
    }
    public interface IErrorResponse : IResponse { }
    /// <summary>
    /// 仮のエラー。エラー値を未実装の場合に仮としてこれを返す。
    /// </summary>
    public class Error : IErrorResponse
    {
        public RequestId RequestId { get; }
        public Error(RequestId reqId)
        {
            RequestId = reqId;
        }
    }
    public class UnknownRequestError : IErrorResponse
    {
        public RequestId RequestId { get; }
        public UnknownRequestError(RequestId reqId)
        {
            RequestId = reqId;
        }
    }
    public class RequestAddComment : IRequest
    {
        public RequestId Id { get; } = new RequestId();
        public ConnectionId ConnectionId { get; }
        public ISiteMessage Message { get; }
        public IMessageMetadata2 Metadata { get; }

        public RequestAddComment(ConnectionId connectionId, ISiteMessage message, IMessageMetadata2 metadata)
        {
            ConnectionId = connectionId;
            Message = message;
            Metadata = metadata;
        }
    }
    public class RequestUpdateMetadata : IRequest
    {
        public RequestId Id { get; } = new RequestId();
        public ConnectionId ConnectionId { get; }
        public string? Name { get; }
        public IMetadata? Metadata { get; }
        public RequestUpdateMetadata(ConnectionId connectionId, string? name, IMetadata? metadata)
        {
            ConnectionId = connectionId;
            Name = name;
            Metadata = metadata;
        }
    }
    public class RequestConnectionStatus : IRequest
    {
        public ConnectionId ConnectionId { get; }

        public RequestConnectionStatus(ConnectionId connectionId)
        {
            ConnectionId = connectionId;
        }

        public RequestId Id { get; } = new RequestId();
    }
    public class ResponseAddConnection : IResponse
    {
        public ResponseAddConnection(RequestId reqId, ConnectionId connectionId, string input, string name, SitePluginId? selectedSite, Guid? selectedBrowser, bool connect)
        {
            RequestId = reqId;
            ConnectionId = connectionId;
            Input = input;
            Name = name;
            Site = selectedSite;
            Browser = selectedBrowser;
            Connect = connect;
        }

        public RequestId RequestId { get; }
        public ConnectionId ConnectionId { get; }
        public string Input { get; }
        public string Name { get; }
        public SitePluginId? Site { get; }
        public Guid? Browser { get; }
        public bool Connect { get; }
    }


    public class RequestAddConnection : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class RequestRemoveConnection : IRequest
    {
        public ConnectionId ConnectionId { get; }
        public RequestId Id { get; } = new RequestId();

        public RequestRemoveConnection(ConnectionId id)
        {
            ConnectionId = id;
        }
    }
    public class RequestConnectionIds : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class RequestAppClose : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class RequestChangeConnectionStatus : IRequest, IConnectionStatusDiff
    {
        public ConnectionId ConnectionId { get; }

        public RequestChangeConnectionStatus(ConnectionId connectionId)
        {
            ConnectionId = connectionId;
        }

        public string? Name { get; set; }
        public string? Input { get; set; }
        public SitePluginId? Site { get; set; }
        public Guid? Browser { get; set; }
        public bool? IsConnected { get; set; }
        public RequestId Id { get; } = new RequestId();
        public override string ToString()
        {
            return Raw;
        }
        public string Raw
        {
            get
            {
                var list = new List<string>();
                if (Name != null)
                {
                    var s = $"\"Name\":\"{Name}\"";
                    list.Add(s);
                }
                if (Input != null)
                {
                    var s = $"\"Input\":\"{Input}\"";
                    list.Add(s);
                }
                if (Site != null)
                {
                    var s = $"\"Site\":\"{Site}\"";
                    list.Add(s);
                }
                if (Browser != null)
                {
                    var s = $"\"Browser\":\"{Browser}\"";
                    list.Add(s);
                }
                if (IsConnected != null)
                {
                    var s = $"\"Connect\":\"{IsConnected}\"";
                    list.Add(s);
                }
                return $"{{\"Type\":\"Request\",\"Request\":\"ChangeConnectionStatus\",\"Id\":\"{Id}\",\"Data\":{{{string.Join(",", list)}}}}}";
            }
        }

        public string? LoggedInUserName { get; set; }
    }
    public class ResponseCoreBuildType : IResponse
    {
        public ResponseCoreBuildType(RequestId id, string buildType)
        {
            RequestId = id;
            BuildType = buildType;
        }
        public RequestId RequestId { get; }
        public string BuildType { get; }
    }

    public class RequestCoreBuildType : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class ResponseCoreVersion : IResponse
    {
        public ResponseCoreVersion(RequestId id, string version)
        {
            RequestId = id;
            Version = version;
        }
        public RequestId RequestId { get; }
        public string Version { get; }
    }
    public class RequestLatestVersion : IRequestAsync
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class ResponseLatestVersion : IResponse
    {
        public ResponseLatestVersion(RequestId requestId, string version, string url)
        {
            RequestId = requestId;
            Version = version;
            Url = url;
        }

        public RequestId RequestId { get; }
        public string Version { get; }
        public string Url { get; }
    }
    public class ResponseUnknownRequest : IResponse
    {
        public ResponseUnknownRequest(RequestId requestId)
        {
            RequestId = requestId;
        }
        public RequestId RequestId { get; }
    }
    public class RequestUpdate : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class RequestCheckUpdate : IRequest
    {
        public RequestId Id { get; } = new RequestId();

    }
    public class ResponseCheckUpdate : IResponse
    {
        public RequestId RequestId { get; }
        public ResponseCheckUpdate(RequestId requestId)
        {
            RequestId = requestId;
        }
    }
    public class RequestLoadedPlugins : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class ResponseLoadedPlugins : IResponse
    {
        public RequestId RequestId { get; }
        public List<LoadedPlugin> Plugins { get; }

        public ResponseLoadedPlugins(RequestId requestId, List<LoadedPlugin> loadedPlugins)
        {
            RequestId = requestId;
            Plugins = loadedPlugins;
        }
    }
    public class LoadedPlugin
    {
        public PluginId PluginId { get; }

        public LoadedPlugin(PluginId pluginId, string name)
        {
            PluginId = pluginId;
            Name = name;
        }

        public string Name { get; }
    }
    public class RequestShowPluginSettingView : IRequest
    {
        public PluginId TargetPluginId { get; }

        public RequestShowPluginSettingView(PluginId pluginId)
        {
            TargetPluginId = pluginId;
        }

        public RequestId Id { get; } = new RequestId();
    }
    public class RequestCoreVersion : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class RequestUpdateCoreAsync : IRequestAsync
    {
        public RequestUpdateCoreAsync(string latestVersionUrl)
        {
            LatestVersionUrl = latestVersionUrl;
        }
        public RequestId Id { get; } = new RequestId();
        public string LatestVersionUrl { get; }
    }
    public class ResponseConnectionIds : IResponse
    {
        public RequestId RequestId { get; }
        public ConnectionId[] Ids { get; }

        public ResponseConnectionIds(RequestId id, ConnectionId[] ids)
        {
            RequestId = id;
            Ids = ids;
        }
    }
    public class ResponseConnectionStatusAdded : IResponse, IConnectionStatus
    {
        public ResponseConnectionStatusAdded(RequestId id, IConnectionStatus connectionStatus)
        {
            RequestId = id;
            ConnectionId = connectionStatus.ConnectionId;
            Name = connectionStatus.Name;
            Input = connectionStatus.Input;
            Site = connectionStatus.Site;
            Browser = connectionStatus.Browser;
            IsConnected = connectionStatus.IsConnected;
            LoggedInUserName = connectionStatus.LoggedInUserName;
        }


        public RequestId RequestId { get; }
        public ConnectionId ConnectionId { get; }
        public string Name { get; }
        public string Input { get; }
        public SitePluginId Site { get; }
        public Guid Browser { get; }
        public bool IsConnected { get; }
        public string LoggedInUserName { get; }

        public override string ToString()
        {
            return $"{{\"type\":\"Response\",\"Response\":\"ConnectionStatusAdded\",\"Id\":\"{RequestId}\"}}";
        }
    }
    public class ResponseConnectionStatusChanged : IResponse, IConnectionStatusDiff
    {
        public ResponseConnectionStatusChanged(RequestId id, IConnectionStatusDiff connectionStatus)
        {
            RequestId = id;
            ConnectionId = connectionStatus.ConnectionId;
            Name = connectionStatus.Name;
            Input = connectionStatus.Input;
            Site = connectionStatus.Site;
            Browser = connectionStatus.Browser;
            IsConnected = connectionStatus.IsConnected;
            LoggedInUserName = connectionStatus.LoggedInUserName;
        }


        public RequestId RequestId { get; }
        public ConnectionId ConnectionId { get; }
        public string? Name { get; }
        public string? Input { get; }
        public SitePluginId? Site { get; }
        public Guid? Browser { get; }
        public bool? IsConnected { get; }
        public bool? Disconnect { get; }
        public bool HasChanged { get; }
        public string? LoggedInUserName { get; }

        public override string ToString()
        {
            return $"{{\"type\":\"Response\",\"Response\":\"ConnectionStatusAdded\",\"Id\":\"{RequestId}\"}}";
        }
    }

    #region User
    public class RequestUser : IRequest
    {
        public SitePluginId SiteGuid { get; }
        public string UserId { get; }
        public RequestId Id { get; } = new RequestId();

        public RequestUser(SitePluginId siteGuid, string userId)
        {
            SiteGuid = siteGuid;
            UserId = userId;
        }
    }
    public class ResponseUser : IResponse
    {
        public RequestId RequestId { get; }
        public IMcvUser User { get; }
        public ResponseUser(RequestId id, IMcvUser user)
        {
            RequestId = id;
            User = user;
        }
    }
    public class RequestChangeUserStatus : IRequest, IMcvUserDiff
    {
        public SitePluginId SiteId { get; }
        public string UserId { get; }

        public RequestChangeUserStatus(SitePluginId siteContextGuid, string userId)
        {
            SiteId = siteContextGuid;
            UserId = userId;
        }
        public RequestChangeUserStatus(IMcvUserDiff diff)
        {
            //警告回避のためにnullableじゃない値は手動でセットする。
            SiteId = diff.SiteId;
            UserId = diff.UserId;

            //他の項目はリフレクションでセット。
            var props = diff.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.Name == nameof(SiteId)) continue;
                if (prop.Name == nameof(UserId)) continue;
                prop.SetValue(this, prop.GetValue(diff));
            }
        }
        public RequestId Id { get; } = new RequestId();
        public bool? IsNgUser { get; set; }
        public bool? IsSiteNgUser { get; set; }
        public string? Nickname { get; set; }
        public IEnumerable<IMessagePart>? Name { get; set; }
    }
    #endregion //User

    public class ResponseMainViewPosition : IResponse
    {
        public double X { get; }
        public double Y { get; }
        public RequestId RequestId { get; }
        public ResponseMainViewPosition(RequestId id, double x, double y)
        {
            RequestId = id;
            X = x;
            Y = y;
        }
    }

    public class RequestMainViewPosition : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class ResponseShowPluginSettingView : IResponse
    {
        public RequestId RequestId { get; }

        public ResponseShowPluginSettingView(RequestId id)
        {
            RequestId = id;
        }
    }

    #region PluginList
    /// <summary>
    /// インストール可能なプラグインのリストを取得する
    /// </summary>
    public class RequestPluginList : IRequestAsync
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class ResponsePluginList : IResponse
    {
        public ResponsePluginList(RequestId requestId, List<DownloadablePluginInfo> pluginList)
        {
            RequestId = requestId;
            PluginList = pluginList;
        }

        public RequestId RequestId { get; }
        public List<DownloadablePluginInfo> PluginList { get; }
    }
    public class DownloadablePluginInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Size { get; set; }
        public PluginType Type { get; set; }
        public string LatestVersion { get; set; }
        public string InstalledVersion { get; set; }
    }
    public enum PluginType
    {
        Dll,
        Exe,
    }
    #endregion //PluginList

    #region InstallPlugin
    public class RequestInstallPlugin : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    #endregion //InstallPlugin

    public class RequestPostCommentAsync : IRequestAsync
    {
        public RequestId Id { get; } = new RequestId();
        public ConnectionId ConnectionId { get; }
        public ICommentDataToPost DataToPost { get; }

        public RequestPostCommentAsync(ConnectionId connectionId, ICommentDataToPost dataToPost)
        {
            ConnectionId = connectionId;
            DataToPost = dataToPost;
        }
    }
    public class GetSiteType : IRequest
    {
        public RequestId Id { get; } = new RequestId();
        public SitePluginId SiteId { get; }

        public GetSiteType(SitePluginId siteId)
        {
            SiteId = siteId;
        }
    }
    public class ResponseSiteType : IResponse
    {
        public ResponseSiteType(RequestId requestId, SiteType type)
        {
            RequestId = requestId;
            SiteType = type;
        }

        public SiteType SiteType { get; }
        public RequestId RequestId { get; }
    }
    /// <summary>
    /// BrowserIdからBrowserProfileを取得する
    /// </summary>
    public class RequestBrowser : IRequest
    {
        public RequestId Id { get; } = new RequestId();
        public Guid BrowserId { get; }

        public RequestBrowser(Guid browserId)
        {
            BrowserId = browserId;
        }
    }
    public class ResponseBrowser : IResponse
    {
        public RequestId RequestId { get; }
        public IBrowserProfile2 BrowserProfile { get; }

        public ResponseBrowser(RequestId reqId, ryu_s.BrowserCookie.IBrowserProfile2 browserProfile)
        {
            RequestId = reqId;
            BrowserProfile = browserProfile;
        }
    }
    /// <summary>
    /// 読み込み済みのブラウザを全て取得する
    /// </summary>
    public class RequestBrowsers : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class ResponseBrowsers : IResponse
    {
        public RequestId RequestId { get; }
        public IEnumerable<IBrowserProfile2> BrowserProfiles { get; }

        public ResponseBrowsers(RequestId reqId, IEnumerable<IBrowserProfile2> browserProfiles)
        {
            RequestId = reqId;
            BrowserProfiles = browserProfiles;
        }
    }
    public class RequestSites : IRequest
    {
        public RequestId Id { get; } = new RequestId();
    }
    public class ResponseSites : IResponse
    {
        public RequestId RequestId { get; }
        public IEnumerable<(SitePluginId, string)> Sites { get; }
        public ResponseSites(RequestId reqId, IEnumerable<(SitePluginId, string)> sites)
        {
            RequestId = reqId;
            Sites = sites;
        }
    }
}
