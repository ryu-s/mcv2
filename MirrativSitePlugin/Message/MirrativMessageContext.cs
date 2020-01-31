using SitePlugin;
using System;

namespace MirrativSitePlugin
{
    internal class MirrativMessageContext2 : IMessageContext2
    {
        public SitePlugin.ISiteMessage Message { get; }

        public IMessageMetadata2 Metadata { get; }

        public IMessageMethods Methods { get; }
        public MirrativMessageContext2(IMirrativMessage message, IMirrativMessageMetadata2 metadata, IMessageMethods methods)
        {
            Message = message;
            Metadata = metadata;
            Methods = methods;
        }
    }
}
