using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeLiveSitePlugin
{
    public class YouTubeLiveSitePlugin : ISitePlugin
    {
        Dictionary<ConnectionId, CommentProvider> _dict = new Dictionary<ConnectionId, CommentProvider>();
        private readonly IYouTubeLiveServer _server;
        private readonly ILogger _logger;

        public async void Connect(ConnectionId connectionId, string input, IBrowserProfile2 browser)
        {
            var cp = GetOrCreateCommentProvider(connectionId);
            try
            {
                await cp.ConnectAsync(input, browser);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //disconnectedの通知
            }
        }
        private CommentProvider GetOrCreateCommentProvider(ConnectionId connectionId)
        {
            if (!_dict.TryGetValue(connectionId, out var cp))
            {
                cp = new CommentProvider(_server, Convert(_siteOptions), _logger);
            }
            return cp;
        }
        private IYouTubeLiveSiteOptions Convert(IYouTubeLiveSiteOptions2 siteOptions2)
        {
            throw new NotImplementedException();
        }
        public void Disconnect(ConnectionId connectionId)
        {
            throw new NotImplementedException();
        }

        public ISiteOptions2 GetSiteOptions()
        {
            return _siteOptions;
        }

        public bool IsValidInput(string input)
        {
            throw new NotImplementedException();
        }
        IYouTubeLiveSiteOptions2 _siteOptions;
        public void SetSiteOptions(ISiteOptions2 siteOptions)
        {
            if (siteOptions is IYouTubeLiveSiteOptions2 ytOptions)
            {
                _siteOptions.Set(ytOptions);
            }
        }
        public YouTubeLiveSitePlugin(IYouTubeLiveServer server, ILogger logger)
        {
            _server = server;
            _logger = logger;
            _siteOptions = new YouTubeLiveSiteOptions2();
        }
    }
    class YouTubeLiveSiteOptions2 : DynamicOptionsBase, IYouTubeLiveSiteOptions2
    {
        public void Set(IYouTubeLiveSiteOptions2 options)
        {
        }

        protected override void Init()
        {
        }
    }

}
