using SitePlugin;
using System;

namespace TwitchSitePlugin
{
    internal class TwitchMessageContext2 : IMessageContext2
    {
        public SitePlugin.ISiteMessage Message { get; }

        public IMessageMetadata2 Metadata { get; }
        public TwitchMessageContext2(ITwitchMessage message, MessageMetadata2 metadata)
        {
            Message = message;
            Metadata = metadata;
        }
    }
}
