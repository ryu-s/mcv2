using SitePlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;

namespace mcv2.MainViewPlugin
{
    interface IModel
    {
        PluginId Id { get; }
        void SendRequest(IRequest req);
        //Task SendRequestAsync(IRequestAsync req);
        IResponse GetData(IRequest req);
        string GetCoreVersion();
        string GetCoreBuildType();
        void RequestShowPluginSettingView(PluginId id);
        void SendError(Exception ex, string message, string rawData);
        UserViewModel GetUser(SitePluginId siteContextGuid, string userId);
        Task<(string? version, string? url)> GetLatestVersionAsync();
        Task RequestUpdateCoreAsync(string latestVersionUrl);
        SiteType GetSiteType(SitePluginId siteId);
        ConnectionId? GetCurrentConnection();
        void SetCurrentConnection(ConnectionId connectionId);
        Task PostCommentAsync(ICommentDataToPost commentData);
        void RemoveSelectedConnection();
        ConnectionData GetConnection(ConnectionId connectionId);
        SitePluginId? GetSelectedSite(ConnectionId connectionId);
        Guid? GetSelectedBrowser(ConnectionId connectionId);
        event EventHandler<INotify> NotifyReceived;
    }
    interface IConnectionModel
    {
        void SetIsSelected(ConnectionId id, bool value);
        bool GetIsSelected(ConnectionId id);
        void SetNeedSave(ConnectionId id, bool value);
        bool GetNeedSave(ConnectionId id);
    }
}
