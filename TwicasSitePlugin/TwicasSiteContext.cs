using System;
using Common;
using System.Windows.Threading;
using SitePlugin;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Diagnostics;
using SitePluginCommon;

namespace TwicasSitePlugin
{
    public class TwicasSiteContext2 : SiteContextBase
    {
        public override SitePluginId Guid { get; } = new SitePluginId(new System.Guid("8649A30C-D9C8-4ADB-862D-E0DAAEA24CE2"));
        public override string DisplayName => "Twicas";
        public override SiteType SiteType => SiteType.Twicas;

        public override ICommentProvider2 CreateCommentProvider()
        {
            return new TwicasCommentProvider2(new TwicasServer(), _logger, _siteOptions)
            {
                SiteContextGuid = Guid,
            };
        }

        public override bool IsValidInput(string input)
        {
            return Tools.IsValidUrl(input);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isIdOk">URLだけでなく、IDのみでもtrueを返すか</param>
        /// <returns></returns>
        public bool IsValidInput(string input, bool isIdOk)
        {
            if (!isIdOk)
            {
                return Tools.IsValidUrl(input);
            }
            else
            {
                return Tools.IsValidUrl(input) || Tools.IsValidUserId(input);
            }
        }

        private TwicasSiteOptions _siteOptions;
        public override void LoadOptions(string path, IIo io)
        {
            _siteOptions = new TwicasSiteOptions();
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
        private readonly ILogger _logger;
        public TwicasSiteContext2(ILogger logger)
            : base(logger)
        {
        }
    }
}
