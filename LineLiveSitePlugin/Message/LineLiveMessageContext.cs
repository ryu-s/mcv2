using SitePlugin;
using System;

namespace LineLiveSitePlugin
{
    internal class LineLiveMessageContext2 : IMessageContext2
    {
        public SitePlugin.ISiteMessage Message { get; }

        public IMessageMetadata2 Metadata { get; }

        public IMessageMethods Methods { get; }
        public LineLiveMessageContext2(ILineLiveMessage message, MessageMetadata2 metadata, IMessageMethods methods)
        {
            Message = message;
            Metadata = metadata;
            Methods = methods;
        }
    }
}
