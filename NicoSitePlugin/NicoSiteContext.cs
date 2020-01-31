using Common;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
namespace NicoSitePlugin
{
    public class NicoSiteContext2 : SiteContextBase2, INicoSiteContext2
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("5A477452-FF28-4977-9064-3A4BC7C63252"));
        public override string DisplayName => "ニコ生";
        public override SiteType SiteType => SiteType.NicoLive;

        private INicoCommentProvider2 GetNicoCommentProvider()
        {
            return new NicoCommentProvider2(_siteOptions, _server, _logger)
            {
                SiteContextGuid = Guid,
            };
        }
        public override ICommentProvider2 CreateCommentProvider()
        {
            return GetNicoCommentProvider();
        }
        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new NicoSiteOptions();
            try
            {
                var s = io.ReadFile(path);

                _siteOptions.Deserialize(s);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _logger.LogException(ex, "", path);
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
                _logger.LogException(ex, "", path);
            }
        }

        public override bool IsValidInput(string input)
        {
            return NicoCommentProvider2.IsValidInput(_siteOptions, _server, _logger, input, Guid);
        }
        INicoSiteOptions INicoSiteContext2.GetNicoSiteOptions()
        {
            return _siteOptions;
        }
        INicoCommentProvider2 INicoSiteContext2.CreateNicoCommentProvider()
        {
            return GetNicoCommentProvider();
        }
        private NicoSiteOptions _siteOptions;
        private readonly IDataSource _server;
        private readonly ILogger _logger;

        public NicoSiteContext2(IDataSource server, Func<string, int, int, ISplitBuffer, IStreamSocket> _, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
        }
    }
}
