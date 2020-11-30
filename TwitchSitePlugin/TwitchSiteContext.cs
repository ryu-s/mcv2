using System;
using System.Linq;
using System.Text;
using SitePlugin;
using System.Diagnostics;
using System.Windows.Threading;
using Common;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using SitePluginCommon;

namespace TwitchSitePlugin
{
    public class TwitchSiteContext2 : SiteContextBase
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("22F7824A-EA1B-411E-85CA-6C9E6BE94E39"));
        public override string DisplayName => "Twitch";

        public override SiteType SiteType => SiteType.Twitch;

        public override ICommentProvider CreateCommentProvider()
        {
            return new TwitchCommentProvider2(_server, _logger, _siteOptions)
            {
                SiteContextGuid = Guid,
            };
        }
        private TwitchSiteOptions _siteOptions;
        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new TwitchSiteOptions();
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
        public override void Init()
        {
            base.Init();
        }
        public override void Save()
        {
            base.Save();
        }
        public override bool IsValidInput(string input)
        {
            //チャンネル名だけ来られても他のサイトのものの可能性があるからfalse
            //"twitch.tv/"の後に文字列があったらtrueとする。
            var b = Regex.IsMatch(input, "twitch\\.tv/[a-zA-Z0-9_]+");
            return b;
        }
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        public TwitchSiteContext2(IDataServer server, ILogger logger)
            : base(logger)
        {
            _server = server;
            _logger = logger;
        }
    }
}
