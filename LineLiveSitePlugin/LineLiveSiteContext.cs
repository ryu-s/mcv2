using Common;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace LineLiveSitePlugin
{
    public class LineLiveSiteContext : SiteContextBase2
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("36F139CA-EAB9-45B1-8CDC-AD47A4051BD3"));
        public override string DisplayName => "LINELIVE";
        public override SiteType SiteType => SiteType.LineLive;

        public override ICommentProvider2 CreateCommentProvider()
        {
            return new LineLiveCommentProvider(_server, _logger, _siteOptions)
            {
                SiteContextGuid = Guid,
            };
        }
        private LineLiveSiteOptions _siteOptions;
        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new LineLiveSiteOptions();
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
            if (string.IsNullOrEmpty(input)) return false;
            //最低限チャンネルIDさえあればコメントを取れるようにした
            //https://live.line.me/channels/2354725/broadcast/8428420
            var b = Regex.IsMatch(input, "line\\.me/channels/\\d+");
            return b;
        }
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        public LineLiveSiteContext(IDataServer server, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
        }
    }
}
