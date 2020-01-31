using System;
using SitePlugin;
using Common;
using System.Diagnostics;
using System.Windows.Controls;
using SitePluginCommon;

namespace YouTubeLiveSitePlugin.Test2
{
    public class YouTubeLiveSiteContext2 : SiteContextBase2, IYouTubeSiteContext2
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("F1631B64-6572-4530-ABAF-21707F15D893"));

        public override string DisplayName => "YouTubeLive";
        public override SiteType SiteType => SiteType.YouTubeLive;
        public override ICommentProvider2 CreateCommentProvider()
        {
            //return new YouTubeCommentProvider(connectionName, _options, _siteOptions);
            return new Test2.CommentProvider2(_server, _siteOptions, _logger)
            {
                SiteContextGuid = Guid,
            };
        }

        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new YouTubeLiveSiteOptions();
            try
            {
                var s = io.ReadFile(path);

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

        private readonly IYouTubeLibeServer _server;
        private readonly ILogger _logger;
        private Test2.YouTubeLiveSiteOptions _siteOptions;
        public YouTubeLiveSiteContext2(IYouTubeLibeServer server, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
        }
    }
}
