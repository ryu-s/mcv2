using System;
using SitePlugin;
using Common;
using System.Diagnostics;
using System.Windows.Controls;
using SitePluginCommon;

namespace YouTubeLiveSitePlugin
{
    public class YouTubeLiveSiteContext : SiteContextBase, IYouTubeSiteContext
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("F1631B64-6572-4530-ABAF-21707F15D893"));

        public override string DisplayName => "YouTubeLive";
        public override SiteType SiteType => SiteType.YouTubeLive;
        public override ICommentProvider CreateCommentProvider()
        {
            //return new YouTubeCommentProvider(connectionName, _options, _siteOptions);
            return new CommentProvider(_server, _siteOptions, _logger)
            {
                SiteContextGuid = Guid,
            };
        }

        public override void LoadOptions(string path, IIo io)
        {
            try
            {
                var s = io.ReadFile(path);
                if (s == null) return;
                _siteOptions.Deserialize(s);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _logger.LogException(ex, "", $"path={path}");
            }
        }

        public override void SaveOptions(string path, IIo io)
        {
            try
            {
                var s = _siteOptions.Serialize();
                io.WriteFile(path, s);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _logger.LogException(ex, "", $"path={path}");
            }
        }

        public override bool IsValidInput(string input)
        {
            var resolver = new VidResolver();
            return resolver.IsValidInput(input);
        }

        private readonly IYouTubeLiveServer _server;
        private readonly ILogger _logger;
        private YouTubeLiveSiteOptions _siteOptions;
        public YouTubeLiveSiteContext(IYouTubeLiveServer server, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
            _siteOptions = new YouTubeLiveSiteOptions();
        }
    }
}
