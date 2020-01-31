using SitePlugin;
using System;

namespace BigoSitePlugin
{
    internal class BigoMessageContext2 : IMessageContext2
    {
        public SitePlugin.ISiteMessage Message { get; }

        public IMessageMetadata2 Metadata { get; }

        public IMessageMethods Methods { get; }
        public BigoMessageContext2(IBigoMessage message, BigoMessageMetadata2 metadata, IMessageMethods methods)
        {
            Message = message;
            Metadata = metadata;
            Methods = methods;
        }
    }
}
