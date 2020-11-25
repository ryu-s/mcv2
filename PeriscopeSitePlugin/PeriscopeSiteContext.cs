using Common;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PeriscopeSitePlugin
{
    public class PeriscopeSiteContext2 : SiteContextBase
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("FB468FFA-D0E5-4423-968C-5B9E1D258730"));
        public override string DisplayName => "Periscope";
        public override SiteType SiteType => SiteType.Periscope;

        public override ICommentProvider2 CreateCommentProvider()
        {
            return new PeriscopeCommentProvider99(_server, _logger, _siteOptions)
            {
                SiteContextGuid = Guid,
            };
        }
        private PeriscopeSiteOptions _siteOptions;
        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new PeriscopeSiteOptions();
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
            var (channelId, liveId) = Tools.ExtractChannelNameAndLiveId(input);
            return !string.IsNullOrEmpty(channelId) || !string.IsNullOrEmpty(liveId);
        }
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        public PeriscopeSiteContext2(IDataServer server, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
        }
    }
}
