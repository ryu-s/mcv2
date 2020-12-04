using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace TestSitePlugin
{
    class TestMetadata2 : IMessageMetadata2
    {
        public bool IsNgUser { get; } = false;
        public bool IsSiteNgUser { get; } = false;
        public bool IsFirstComment { get; } = false;
        public bool IsInitialComment { get; } = false;
        public bool Is184 { get; } = false;
        public SitePluginId SiteContextGuid { get; set; }
        public string UserId { get; }
        public IEnumerable<IMessagePart> UserName { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public SiteType SiteType => SiteType.Unknown;
        public string? NewNickname { get; }

        public TestMetadata2(string userId)
        {
            UserId = userId;
        }
    }
}
