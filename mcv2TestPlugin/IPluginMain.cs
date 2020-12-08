using mcv2;
using SitePlugin;
using System;

namespace mcv2TestPlugin
{
    interface IPluginMain
    {
        void PostComment(ConnectionId connectionId, ISiteMessage message, IMessageMetadata2 metadata);
        void SetRequest(IRequest req);
        IResponse GetData(IRequest req);
    }
}
