using System;
using SitePlugin;
using Common;
using System.Diagnostics;
using System.Windows.Controls;
using SitePluginCommon;

namespace BigoSitePlugin
{
    public class BigoSiteContext : SiteContextBase, IBigoSiteContext
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("F2A72B82-324D-4792-858A-338AE7B34D7F"));

        public override string DisplayName => "Bigo";
        public override SiteType SiteType => SiteType.Bigo;
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
            _siteOptions = new BigoSiteOptions();
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
            return System.Text.RegularExpressions.Regex.IsMatch(input, "bigo\\.tv/\\d+");
        }
        private readonly IBigoServer _server;
        private readonly ILogger _logger;
        private BigoSiteOptions _siteOptions;
        public BigoSiteContext(IBigoServer server, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
        }
    }
}
