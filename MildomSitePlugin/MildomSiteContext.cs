using Common;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MildomSitePlugin
{
    public class MildomSiteContext2 : SiteContextBase2
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("DBBA654F-0A5D-41CC-8153-5DB2D5869BCF"));
        public override string DisplayName => "Mildom";
        public override SiteType SiteType => SiteType.Mildom;

        public override ICommentProvider2 CreateCommentProvider()
        {
            return new MildomCommentProvider2(_server, _logger, _siteOptions)
            {
                SiteContextGuid = Guid,
            };
        }
        private MildomSiteOptions _siteOptions;
        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new MildomSiteOptions();
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
            return Tools.IsValidRoomUrl(input);
        }
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        public MildomSiteContext2(IDataServer server, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
        }
    }
}
