﻿using SitePlugin;
using System;
using System.Windows.Media;

namespace WhowatchSitePlugin
{
    internal class ItemMessageMetadata2 : MessageMetadataBase2
    {
        public ItemMessageMetadata2(IWhowatchItem message, IWhowatchSiteOptions siteOptions, ICommentProvider cp)
            : base(siteOptions, cp)
        {
        }
    }
}
