using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Linq;

namespace mcv2.Model
{
    class ConnectionHost : IConnectionHost
    {
        private readonly Model _model;
        private readonly ISitePluginManager _sitePluginManager;

        public event EventHandler<ICurrentUserInfo> UserInfoRetrieved;

        public ConnectionHost(Model model, ISitePluginManager sitePluginManager)
        {
            _model = model;
            _sitePluginManager = sitePluginManager;
        }
        private IBrowserProfile2? Convert(Guid? browserProfileId)
        {
            var res = _model.GetData(new RequestBrowser(browserProfileId)) as ResponseBrowser;
            if (res == null) return null;
            return res.BrowserProfile;
        }
        public Task ConnectAsync(ConnectionId connectionId, string input, SitePluginId sitePluginId, Guid? browserProfileId)
        {
            var browserProfile = Convert(browserProfileId);
            return _sitePluginManager.ConnectAsync(connectionId, input, sitePluginId, browserProfile);
        }
        public void Disconnect(ConnectionId connectionId)
        {
            _sitePluginManager.Disconnect(connectionId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId">デフォルト名を取得したいConnectionのConnectionId</param>
        /// <returns></returns>
        public string GetDefaultName(ConnectionId connectionId)
        {
            var list = new List<string>();
            var res = _model.GetData(new RequestConnectionIds()) as ResponseConnectionIds;
            foreach (var id in res!.Ids)
            {
                var res1 = _model.GetData(new RequestConnectionStatus(id)) as ResponseConnectionStatusAdded;
                list.Add(res1!.Name);
            }
            return GetDefaultName(list);
        }
        private string GetDefaultName(IEnumerable<string> existingNames)
        {
            for (var n = 1; ; n++)
            {
                var testName = "#" + n;
                if (!existingNames.Contains(testName))
                {
                    return testName;
                }
            }
        }
        public async void GetLoggedInUserName(ConnectionId connectionId, SitePluginId sitePluginId, Guid? browserProfileId)
        {
            try
            {
                var browserProfile = Convert(browserProfileId);
                var info = await _sitePluginManager.GetLoggedInUserName(connectionId, sitePluginId, browserProfile);
                UserInfoRetrieved?.Invoke(this, info);
            }
            catch (Exception ex)
            {
                _model.SetError(ex);
            }
        }

        public void SetNotify(INotify notify)
        {
            _model.SetNotify(notify);
        }
    }
    /// <summary>
    /// ConnectionとModelの相互依存を無くし、疎結合を目指すためのインターフェース
    /// </summary>
    interface IConnectionHost
    {
        event EventHandler<ICurrentUserInfo> UserInfoRetrieved;
        Task ConnectAsync(ConnectionId connectionId, string input, SitePluginId sitePluginId, Guid? browserProfile);
        //void ConnectionStatusChanged();
        string GetDefaultName(ConnectionId connectionId);
        void Disconnect(ConnectionId connectionId);
        void GetLoggedInUserName(ConnectionId connectionId, SitePluginId sitePluginId, Guid? browserProfile);
        void SetNotify(INotify notify);
    }
    //Connectionはデータ保持クラスにする。
    //プロパティの変更通知をしたい。
    //Connectionの内容を変更する時はSet()を使う。
    //Set()で渡されたデータによって変化があったプロパティの値をhostに通知する
    class Connection2 : IConnectionStatus
    {
        public Connection2(IConnectionHost host)
        {
            _host = host;
            host.UserInfoRetrieved += Host_UserInfoRetrieved;
            Name = host.GetDefaultName(ConnectionId);
            Input = "";
            LoggedInUserName = "";
        }

        private void Host_UserInfoRetrieved(object? sender, ICurrentUserInfo e)
        {
            _host.SetNotify(new NotifyConnectionStatusChanged(new ConnectionStatusDiff(ConnectionId)
            {
                LoggedInUserName = e.Username ?? "",//nullを返したら変更無しと判定されてしまう
            }));
        }

        public IConnectionStatusDiff Set(IConnectionStatusDiff diff)
        {
            return Set(diff.Name, diff.Input, diff.Site, diff.Browser, diff.IsConnected);
        }
        public IConnectionStatusDiff Set(string? name, string? input, SitePluginId? site, Guid? browser, bool? connect)
        {
            var diff = new ConnectionStatusDiff(ConnectionId);
            if (name != null && Name != name)
            {
                diff.Name = Name = name;
            }
            if (input != null && input != Input)
            {
                diff.Input = Input = input;
            }
            if (site != null && site != Site)
            {
                diff.Site = Site = site;
                _host.GetLoggedInUserName(ConnectionId, site, browser ?? Browser);
            }
            if (browser != null && browser != Browser)
            {
                diff.Browser = Browser = browser;
                if (Site != null)
                {
                    //TODO:これだとBrowserがnull（未選択状態）になったタイミングでは発動しない
                    _host.GetLoggedInUserName(ConnectionId, Site, browser);
                }
            }
            if (connect != null && connect != IsConnected)
            {
                diff.IsConnected = IsConnected = connect.Value;
                if (connect == true)
                {
                    Connect();
                }
                else
                {
                    //IsConnectedがfalseになるのは実際に切断されたタイミング
                    Disconnect();
                }
            }
            return diff;
        }
        private async void Connect()
        {
            if (Site == null)
            {
                throw new InvalidOperationException("Site is null");
            }
            await _host.ConnectAsync(ConnectionId, Input, Site, Browser);
            IsConnected = false;
        }
        private void Disconnect()
        {
            _host.Disconnect(ConnectionId);
        }
        private readonly IConnectionHost _host;
        public ConnectionId ConnectionId { get; } = new ConnectionId();
        public string Name { get; private set; }
        public string Input { get; private set; }
        public bool IsConnected { get; private set; }
        public SitePluginId? Site { get; private set; }//SitePluginが無い状況を許容するか？
        public Guid? Browser { get; private set; }//ログインせずにコメントを取れるサイトもあるからブラウザは無い状況はあり得る。
        public string LoggedInUserName { get; private set; }
    }
}
