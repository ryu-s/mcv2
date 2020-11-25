using System;
using System.Linq;
using System.Text;
using SitePlugin;
using Common;
using System.Windows.Controls;
using System.Diagnostics;
using SitePluginCommon;

namespace OpenrecSitePlugin
{
    public class OpenrecSiteContext2 : SiteContextBase
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("F4434012-3E68-4DD9-B2A8-F2BD7D601723"));
        public override string DisplayName => "OPENREC";
        public override SiteType SiteType => SiteType.Openrec;

        public override ICommentProvider2 CreateCommentProvider()
        {
            return new CommentProvider2(_siteOptions, _logger)
            {
                SiteContextGuid = Guid,
            };
        }

        public override bool IsValidInput(string input)
        {
            return Tools.IsValidUrl(input);
        }

        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new OpenrecSiteOptions();
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
                _logger.LogException(ex, "", path);
            }
        }
        private OpenrecSiteOptions _siteOptions;
        private ILogger _logger;

        public OpenrecSiteContext2(ILogger logger)
            : base(logger)
        {
            _logger = logger;
        }
    }
}
