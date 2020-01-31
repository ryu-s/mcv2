using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhowatchSitePlugin;

namespace mcv2.Model
{
    public class SitePluginManagerExceptionEventArgs
    {
        public Exception Exception { get; set; }
    }
    public class SiteAddedEventArgs : EventArgs
    {
        public SitePluginId SiteId { get; }
        public string SiteDisplayName { get; }
        public SiteAddedEventArgs(SitePluginId siteId, string displayName)
        {
            SiteId = siteId;
            SiteDisplayName = displayName;
        }
    }
    [Serializable]
    public class ConnectExceptionException : Exception
    {
        public CommentProviderId CommentProviderId { get; set; }
        public ConnectExceptionException() { }
        public ConnectExceptionException(string message) : base(message) { }
        public ConnectExceptionException(string message, Exception inner) : base(message, inner) { }
        protected ConnectExceptionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    class SitePluginAdaptor : ISitePlugin
    {
        private readonly ISiteContext2 _siteContext;

        public void Connect(ConnectionId connectionId, string input, IBrowserProfile2 browser)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(ConnectionId connectionId)
        {
            throw new NotImplementedException();
        }

        public ISiteOptions2 GetSiteOptions()
        {
            throw new NotImplementedException();
        }

        public bool IsValidInput(string input)
        {
            return _siteContext.IsValidInput(input);
        }

        public void SetSiteOptions(ISiteOptions2 siteOptions)
        {
            throw new NotImplementedException();
        }
        public SitePluginAdaptor(ISiteContext2 siteContext)
        {
            _siteContext = siteContext;
        }
    }
    interface ISitePluginManager
    {
        event EventHandler<SiteAddedEventArgs> SiteAdded;
        void Load();
        SiteType GetSiteType(SitePluginId siteId);
        Task ConnectAsync(ConnectionId connectionId, string input, SitePluginId sitePluginId, IBrowserProfile2? browserProfile);
        void Disconnect(ConnectionId connectionId);
        List<SitePluginId> Sites();
        Task<ICurrentUserInfo> GetLoggedInUserName(ConnectionId connectionId,SitePluginId sitePluginId, IBrowserProfile2? browserProfile);
    }
    class SitePluginManager : ISitePluginManager
    {
        public event EventHandler<SitePluginManagerExceptionEventArgs>? ExceptionOccurred;
        public event EventHandler<SiteAddedEventArgs> SiteAdded;

        private readonly List<IMcvSitePlugin> _sitePlugins = new List<IMcvSitePlugin>();
        //private readonly Dictionary<SitePluginId, IMcvCommentProvider> _cpDict = new Dictionary<SitePluginId, IMcvCommentProvider>();
        private readonly Dictionary<CommentProviderId, IMcvCommentProvider> _cpDict = new Dictionary<CommentProviderId, IMcvCommentProvider>();
        private readonly ILogger _logger;

        public void Load()
        {
            AddSite(new YouTubeLiveSitePlugin.Test2.YouTubeLiveSiteContext2(new YouTubeLiveSitePlugin.Test2.YouTubeLiveServer(), _logger));
            AddSite(new TwitchSitePlugin.TwitchSiteContext2(new TwitchSitePlugin.TwitchServer(), _logger));
            AddSite(new WhowatchSitePlugin.WhowatchSiteContext2(_logger));
            AddSite(new TwicasSitePlugin.TwicasSiteContext2(_logger));
            AddSite(new BigoSitePlugin.BigoSiteContext(new BigoSitePlugin.BigoServer(), _logger));
            AddSite(new LineLiveSitePlugin.LineLiveSiteContext(new LineLiveSitePlugin.LineLiveServer(), _logger));
            AddSite(new MildomSitePlugin.MildomSiteContext2(new MildomSitePlugin.MildomServer(), _logger));
            AddSite(new MirrativSitePlugin.MirrativSiteContext2(new MirrativSitePlugin.MirrativServer(), _logger));
            AddSite(new NicoSitePlugin.NicoSiteContext2(new NicoSitePlugin.DataSource(), (addr, port, size, buffer) => new NicoSitePlugin.StreamSocket(addr, port, size, buffer), _logger));
            AddSite(new OpenrecSitePlugin.OpenrecSiteContext2(_logger));
            AddSite(new ShowRoomSitePlugin.ShowRoomSiteContext2(new ShowRoomSitePlugin.ShowRoomServer(), _logger));
            AddSite(new PeriscopeSitePlugin.PeriscopeSiteContext2(new PeriscopeSitePlugin.PeriscopeServer(), _logger));
#if DEBUG
            AddSite(new TestSitePlugin.TestSiteContext2(_logger));
#endif
        }
        Dictionary<SitePluginId, ISiteContext2> _siteDict = new Dictionary<SitePluginId, ISiteContext2>();
        private void AddSite(ISiteContext2 siteContext)
        {
            //サイトプラグインをどうやって読み込もうか。
            //現在のファイル形式だとSiteContextでないと型が分からない
            //siteContext.Init
            siteContext.LoadOptions(System.IO.Path.Combine("settings", siteContext.DisplayName + ".txt"), _io);
            AddSite(new SitePluginAdaptor(siteContext));
            _siteDict.Add(siteContext.Guid, siteContext);
            SiteAdded?.Invoke(this, new SiteAddedEventArgs(siteContext.Guid, siteContext.DisplayName));
        }
        public void AddSite(ISitePlugin sitePlugin)
        {

        }
        public void SetOptions(ISiteOptions siteOptions)
        {
            foreach (var cp in _cpDict.Values)
            {
                cp.SetOptions(siteOptions);
            }
        }
        public void SetSite(ConnectionId connectionId, SitePluginId sitePluginId)
        {

        }
        Dictionary<ConnectionId, (SitePluginId, ICommentProvider2)> _dict = new Dictionary<ConnectionId, (SitePluginId, ICommentProvider2)>();

        public IIo _io;

        public ICommentProvider2 GetOrCreateCommentProvider(ConnectionId connectionId, SitePluginId sitePluginId)
        {
            if (_dict.ContainsKey(connectionId))
            {
                var (s, cp) = _dict[connectionId];
                if (s == sitePluginId)
                {
                    return cp;
                }
                else
                {
                    //別のサイトに変わっている。古いのを除去。
                    _dict.Remove(connectionId);
                }
            }
            var cp2 = CreateCommentProvider(sitePluginId);
            _dict.Add(connectionId, (sitePluginId, cp2));
            return cp2;
        }
        public ICommentProvider2? GetCommentProvider(ConnectionId connectionId)
        {
            if (_dict.ContainsKey(connectionId))
            {
                var (_, cp) = _dict[connectionId];
                return cp;
            }
            else
            {
                return null;
            }
        }
        private ICommentProvider2 CreateCommentProvider(SitePluginId sitePluginId)
        {
            var site = _siteDict[sitePluginId];
            return site.CreateCommentProvider();
        }

        public async Task ConnectAsync(ConnectionId connectionId, string input, SitePluginId sitePluginId, IBrowserProfile2? browserProfile)
        {
            var cp = GetOrCreateCommentProvider(connectionId, sitePluginId);
            try
            {
                await cp.ConnectAsync(input, browserProfile);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        public SiteType GetSiteType(SitePluginId siteId)
        {
            var sitePlugin = _siteDict[siteId];
            return sitePlugin.SiteType;
        }

        public void Disconnect(ConnectionId connectionId)
        {
            var cp = GetCommentProvider(connectionId);
            cp?.Disconnect();
        }

        public List<SitePluginId> Sites()
        {
            return _siteDict.Keys.ToList();
        }

        public SitePluginManager(ILogger logger, IIo io)
        {
            _logger = logger;
            _io = io;
        }
        public Task<ICurrentUserInfo> GetLoggedInUserName(ConnectionId connectionId, SitePluginId sitePluginId, IBrowserProfile2? browserProfile)
        {
            var cp = GetOrCreateCommentProvider(connectionId, sitePluginId);
            return cp.GetCurrentUserInfo(browserProfile);
        }
    }
}
