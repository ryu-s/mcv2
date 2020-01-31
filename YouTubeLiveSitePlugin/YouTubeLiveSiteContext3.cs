using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeLiveSitePlugin.Test2;

namespace YouTubeLiveSitePlugin
{
    public class YouTubeLiveSitePlugin : ISitePlugin
    {
        Dictionary<ConnectionId, CommentProvider2> _dict = new Dictionary<ConnectionId, CommentProvider2>();
        private readonly IYouTubeLibeServer _server;
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
        private CommentProvider2 GetOrCreateCommentProvider(ConnectionId connectionId)
        {
            if (!_dict.TryGetValue(connectionId, out var cp))
            {
                cp = new CommentProvider2(_server, Convert(_siteOptions), _logger);
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
        public YouTubeLiveSitePlugin(IYouTubeLibeServer server, ILogger logger)
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
