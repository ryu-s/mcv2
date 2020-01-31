using Common;
using SitePlugin;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TestSitePlugin
{
    public class TestSiteContext2 : ISiteContext2
    {
        public SitePluginId Guid { get; } = new SitePluginId(new System.Guid("609B4057-A5CE-49BA-A30F-211C4DFE838E"));
        public string DisplayName => "Test";

        public SiteType SiteType => SiteType.Unknown;

        public ICommentProvider2 CreateCommentProvider()
        {
            return new TestCommentProvider2(_logger)
            {
                SiteContextGuid = Guid,
            };
        }
        public void Init()
        {
        }

        public bool IsValidInput(string input)
        {
            return true;
        }

        public void LoadOptions(string path, IIo io)
        {
        }

        public void Save()
        {
        }

        public void SaveOptions(string path, IIo io)
        {
        }
        private readonly ILogger _logger;
        public TestSiteContext2(ILogger logger)
        {
            _logger = logger;
        }
    }
}
