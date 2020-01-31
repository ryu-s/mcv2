using SitePlugin;
using System;

namespace mcv2.MainViewPlugin
{
    class SiteViewModel
    {
        public string DisplayName { get; }
        public SitePluginId SiteId { get; }
        public SiteViewModel(string displayName, SitePluginId siteId)
        {
            DisplayName = displayName;
            SiteId = siteId;
        }
    }
}
