using SitePlugin;
using System;

namespace WhowatchSitePlugin
{
    internal class WhowatchMessageContext2 : IMessageContext2
    {
        public SitePlugin.ISiteMessage Message { get; }
        public IMessageMetadata2 Metadata { get; }
        public WhowatchMessageContext2(IWhowatchMessage message, IMessageMetadata2 metadata)
        {
            Message = message;
            Metadata = metadata;
        }
    }
}
