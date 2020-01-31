using SitePlugin;
using System;

namespace NicoSitePlugin
{
    internal class NicoMessageContext2 : IMessageContext2
    {
        public SitePlugin.ISiteMessage Message { get; }

        public IMessageMetadata2 Metadata { get; }

        public IMessageMethods Methods { get; }
        public NicoMessageContext2(INicoMessage message, INicoMessageMetadata2 metadata, IMessageMethods methods)
        {
            Message = message;
            Metadata = metadata;
            Methods = methods;
        }
    }
}
