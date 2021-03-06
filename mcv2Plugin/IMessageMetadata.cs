﻿using System.Collections.Generic;
using System.ComponentModel;

namespace SitePlugin
{
    public interface IMessageMetadata2 : INotifyPropertyChanged
    {
        bool IsNgUser { get; }
        bool IsSiteNgUser { get; }
        bool IsFirstComment { get; }
        bool IsInitialComment { get; }
        bool Is184 { get; }
        SitePluginId SiteContextGuid { get; }
        string UserId { get; }
        IEnumerable<IMessagePart> UserName { get; }
        SiteType SiteType { get; }
        string? NewNickname { get; }
    }
}
