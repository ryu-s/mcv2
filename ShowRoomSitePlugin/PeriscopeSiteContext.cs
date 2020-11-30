using Common;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ShowRoomSitePlugin
{
    public class ShowRoomSiteContext2 : SiteContextBase
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("C64FBE36-029E-483D-AA56-F1906C42B43B"));
        public override string DisplayName => "SHOWROOM";
        public override SiteType SiteType => SiteType.ShowRoom;

        public override ICommentProvider CreateCommentProvider()
        {
            return new ShowRoomCommentProvider2(_server, _logger, _siteOptions)
            {
                SiteContextGuid = Guid,
            };
        }
        private ShowRoomSiteOptions _siteOptions;
        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new ShowRoomSiteOptions();
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
            var liveId = Tools.ExtractLiveId(input);
            return !string.IsNullOrEmpty(liveId);
        }
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        public ShowRoomSiteContext2(IDataServer server, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
        }
    }
}
