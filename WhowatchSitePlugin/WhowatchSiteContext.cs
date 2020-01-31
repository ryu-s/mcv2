using Common;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WhowatchSitePlugin
{
    public class WhowatchSiteContext2 : SiteContextBase2
    {
        private IWhowatchSiteOptions _siteOptions;
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("EA695072-BABB-4FC9-AB9F-2F87D829AE7D"));
        public override string DisplayName => "ふわっち";
        public override SiteType SiteType => SiteType.Whowatch;

        public override ICommentProvider2 CreateCommentProvider()
        {
            return new WhowatchCommentProvider2(_server, _siteOptions, _logger)
            {
                SiteContextGuid = Guid,
            };
        }
        public override bool IsValidInput(string input)
        {
            return Tools.IsValidUrl(input);
        }
        protected virtual IWhowatchSiteOptions CreateWhowatchSiteOptions()
        {
            return new WhowatchSiteOptions();
        }
        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = CreateWhowatchSiteOptions();
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
        protected virtual IDataServer CreateServer()
        {
            return new DataServer();
        }
        public WhowatchSiteContext2(ILogger logger)
            : base(logger)
        {
            _server = CreateServer();
            _logger = logger;
        }
    }
}
