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
        private readonly ISiteContext _siteContext;

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
        public SitePluginAdaptor(ISiteContext siteContext)
        {
            _siteContext = siteContext;
        }
    }
    interface ISitePluginManager
    {
        event EventHandler<SiteAddedEventArgs> SiteAdded;
        event EventHandler<RequestUpdateMetadata> MetadataUpdated;
        event EventHandler<RequestAddComment> MessageReceived;
        void Load();
        SiteType GetSiteType(SitePluginId siteId);
        Task ConnectAsync(ConnectionId connectionId, string input, SitePluginId sitePluginId, IBrowserProfile2? browserProfile);
        void Disconnect(ConnectionId connectionId);
        List<(SitePluginId, string displayName)> Sites();
        Task<ICurrentUserInfo> GetLoggedInUserName(ConnectionId connectionId, SitePluginId sitePluginId, IBrowserProfile2? browserProfile);
        SitePluginId? GetValidSite(string input);
    }
    class SitePluginManager : ISitePluginManager
    {
        public event EventHandler<SitePluginManagerExceptionEventArgs>? ExceptionOccurred;
        public event EventHandler<SiteAddedEventArgs> SiteAdded;
        public event EventHandler<RequestUpdateMetadata> MetadataUpdated;
        public event EventHandler<RequestAddComment> MessageReceived;

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
        Dictionary<SitePluginId, ISiteContext> _siteDict = new Dictionary<SitePluginId, ISiteContext>();
        private void AddSite(ISiteContext siteContext)
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
        private Dictionary<ICommentProvider2, ConnectionId> _cpIdDict = new Dictionary<ICommentProvider2, ConnectionId>();
        public async Task ConnectAsync(ConnectionId connectionId, string input, SitePluginId sitePluginId, IBrowserProfile2? browserProfile)
        {
            var cp = GetOrCreateCommentProvider(connectionId, sitePluginId);
            _cpIdDict.Add(cp, connectionId);
            cp.MessageReceived += Cp_MessageReceived;
            cp.MetadataUpdated += Cp_MetadataUpdated;
            try
            {
                await cp.ConnectAsync(input, browserProfile);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                _cpIdDict.Remove(cp);
                cp.MessageReceived -= Cp_MessageReceived;
                cp.MetadataUpdated -= Cp_MetadataUpdated;
            }
        }

        private void Cp_MetadataUpdated(object? sender, IMetadata e)
        {
            var cp = sender as ICommentProvider2;
            if (cp == null) return;
            var connectionId = _cpIdDict[cp];
            var req = new RequestUpdateMetadata(connectionId, null, e);
            MetadataUpdated?.Invoke(this, req);
        }

        private void Cp_MessageReceived(object? sender, IMessageContext2 e)
        {
            var cp = sender as ICommentProvider2;
            if (cp == null) return;
            var connectionId = _cpIdDict[cp];
            var req = new RequestAddComment(connectionId, e.Message, e.Metadata);
            MessageReceived?.Invoke(this, req);
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

        public List<(SitePluginId, string)> Sites()
        {
            return _siteDict.Select(kv => (kv.Key, kv.Value.DisplayName)).ToList();
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
        public SitePluginId? GetValidSite(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            foreach (var (id, site) in _siteDict)
            {
                if (site.IsValidInput(input))
                {
                    return id;
                }
            }
            return null;
        }
    }
}
