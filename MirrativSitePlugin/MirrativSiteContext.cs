using Common;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MirrativSitePlugin
{
    public class MirrativSiteContext2 : SiteContextBase
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("6DAFA768-280D-4E70-8494-FD5F31812EF5"));
        public override string DisplayName => "Mirrativ";
        public override SiteType SiteType => SiteType.Mirrativ;

        public override ICommentProvider CreateCommentProvider()
        {
            return new MirrativCommentProvider33(_server, _logger, _siteOptions)
            {
                SiteContextGuid = Guid,
            };
        }
        private MirrativSiteOptions _siteOptions;
        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new MirrativSiteOptions();
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
            return Tools.IsValidLiveId(input) || Tools.IsValidUserId(input);
        }
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        public MirrativSiteContext2(IDataServer server, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
        }
    }
}
