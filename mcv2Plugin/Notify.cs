using SitePlugin;
using System;
using System.Collections;
using System.Collections.Generic;

namespace mcv2
{
#nullable enable
    public interface INotify : IMessage
    {

    }
    public class NotifySiteAdded : INotify
    {
        public string SiteName { get; set; }
        public SitePluginId SiteId { get; set; }
    }
    public class NotifyMessageReceived : INotify
    {
        public ConnectionId ConnectionId { get; set; }
        public ISiteMessage Message { get; set; }
        public IMessageMetadata2 Metadata { get; set; }
        public IMcvUser? User { get; set; }
        public NotifyMessageReceived(ConnectionId connectionId, ISiteMessage message, IMessageMetadata2 metadata, IMcvUser? user)
        {
            ConnectionId = connectionId;
            Message = message;
            Metadata = metadata;
            User = user;
        }
        public string Raw
        {
            get
            {
                return $"{{\"type\":\"notify\",\"notify\":\"\",\"data\":{{\"message\":{Message.Raw},\"metadata\":{{\"userid\":\"{User.Id}\",\"nguser\":{Metadata.IsSiteNgUser.ToString().ToLower()},\"sitenguser\":{Metadata.IsSiteNgUser.ToString().ToLower()}}}}}}}";
            }
        }
    }
    public class NotifyUserChanged : INotify
    {
        public SitePluginId SiteContextGuid { get; }
        public string UserId { get; }
        public IEnumerable<IMessagePart>? Name { get; set; }
        public string? Nickname { get; set; }
        public bool? IsNgUser { get; set; }
        public bool? IsSiteNgUser { get; set; }
        public NotifyUserChanged(SitePluginId siteContextGuid, string userId)
        {
            SiteContextGuid = siteContextGuid;
            UserId = userId;
        }
        public override string? ToString()
        {
            //"abc -> xyz"みたいにできたら面白いかも
            return base.ToString();
        }
    }
    public class NotifyMetadataUpdated : INotify
    {
        public ConnectionId ConnectionId { get; }
        public string? Name { get; }
        public IMetadata? Metadata { get; }
        public NotifyMetadataUpdated(ConnectionId Id, string? name, IMetadata? metadata)
        {
            ConnectionId = Id;
            Name = name;
            Metadata = metadata;
        }
    }
    public class NotifyBrowserAdded : INotify
    {
        public Guid BrowserId { get; set; }
        public string BrowserName { get; set; }
        public string ProfileName { get; set; }
    }
    public class NotifyConnectionStatusChanged : INotify, IConnectionStatusDiff
    {
        public ConnectionId ConnectionId { get; }
        public string? Name { get; }
        public string? Input { get; }
        public SitePluginId? Site { get; }
        public Guid? Browser { get; }
        public bool? IsConnected { get; }
        public string? LoggedInUserName { get; }

        public NotifyConnectionStatusChanged(IConnectionStatusDiff connSt)
        {
            ConnectionId = connSt.ConnectionId;
            Name = connSt.Name;
            Input = connSt.Input;
            Site = connSt.Site;
            Browser = connSt.Browser;
            IsConnected = connSt.IsConnected;
            LoggedInUserName = connSt.LoggedInUserName;
        }
    }
    public class NotifyAddConnection : INotify, IConnectionStatus
    {
        public NotifyAddConnection(ConnectionId connectionId, string input, string name, SitePluginId selectedSite, Guid selectedBrowser, bool connect, string loggedInUserName)
        {
            ConnectionId = connectionId;
            Input = input;
            Name = name;
            SelectedSite = selectedSite;
            SelectedBrowser = selectedBrowser;
            Connect = connect;
            LoggedInUserName = loggedInUserName;
        }
        public ConnectionId ConnectionId { get; }
        public string Input { get; }
        public string Name { get; }
        public SitePluginId SelectedSite { get; }
        public Guid SelectedBrowser { get; }
        public bool Connect { get; }

        SitePluginId IConnectionStatus.Site => SelectedSite;
        Guid IConnectionStatus.Browser => SelectedBrowser;
        bool IConnectionStatus.IsConnected => Connect;
        public string LoggedInUserName { get; }
    }
    public class NotifyRemoveConnection : INotify
    {
        public ConnectionId ConnectionId { get; }

        public NotifyRemoveConnection(ConnectionId connId)
        {
            ConnectionId = connId;
        }
    }
    public class NotifyPluginAdded : INotify
    {
        public PluginId PluginId { get; set; }
        public string PluginName { get; set; }
    }
    public class NotifyUserAdded : INotify
    {
        public NotifyUserAdded(SitePluginId siteContextGuid, string userId)
        {
            SiteContextGuid = siteContextGuid;
            UserId = userId;
        }

        public SitePluginId SiteContextGuid { get; }
        public string UserId { get; }
    }
    public class NotifyDownloadCoreProgress : INotify
    {
        public NotifyDownloadCoreProgress(long bytesReceived, long totalBytesToReceive, int progressPercentage)
        {
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
            ProgressPercentage = progressPercentage;
        }

        public long BytesReceived { get; }
        public long TotalBytesToReceive { get; }
        public int ProgressPercentage { get; }
    }
    public class NotifyCloseApp : INotify
    {
    }
#nullable disable
}
