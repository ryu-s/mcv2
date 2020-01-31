using SitePlugin;
using System;

namespace OpenrecSitePlugin
{
    internal class OpenrecMessageContext2 : IMessageContext2
    {
        public SitePlugin.ISiteMessage Message { get; }

        public IMessageMetadata2 Metadata { get; }

        public IMessageMethods Methods { get; }
        public OpenrecMessageContext2(IOpenrecMessage message, MessageMetadata2 metadata, IMessageMethods methods)
        {
            Message = message;
            Metadata = metadata;
            Methods = methods;
        }
    }
}
