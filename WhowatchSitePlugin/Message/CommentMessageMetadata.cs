using SitePlugin;
using System;
using System.Windows;
using System.Windows.Media;

namespace WhowatchSitePlugin
{
    internal class CommentMessageMetadata2 : MessageMetadataBase2
    {
        public CommentMessageMetadata2(IWhowatchMessage message, IWhowatchSiteOptions siteOptions, ICommentProvider2 cp, bool isFirstComment)
            : base(siteOptions, cp)
        {
            IsFirstComment = isFirstComment;
        }
    }
}
