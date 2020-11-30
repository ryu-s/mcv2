using SitePlugin;

namespace TestSitePlugin
{
    class MessageContext2 : IMessageContext2
    {
        public ISiteMessage Message { get; }
        public IMessageMetadata2 Metadata { get; }
        public IMessageMethods Methods { get; }
        public MessageContext2(ISiteMessage message, IMessageMetadata2 metadata, IMessageMethods methods)
        {
            Message = message;
            Metadata = metadata;
            Methods = methods;
        }
    }
}
