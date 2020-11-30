using SitePlugin;
using System;
using System.Threading.Tasks;

namespace YouTubeLiveSitePlugin
{
    public class SitePluginMain : IMcvSitePlugin
    {
        public IMcvCommentProvider CreateCommentProvider()
        {
            return new McvCommentProvider();
        }
    }
    public class McvCommentProvider : IMcvCommentProvider
    {
        public CommentProviderId Id { get; } = new CommentProviderId();

        public Task ConnectAsync(IConnectOptions connectOptions)
        {
            throw new NotImplementedException();
        }

        public Task PostCommentAsync(ICommentDataToPost commentData)
        {
            throw new NotImplementedException();
        }

        public void SetOptions(ISiteOptions siteOptions)
        {
            throw new NotImplementedException();
        }
    }
}
namespace YouTubeLiveSitePlugin
{
    internal class YouTubeLiveMessageContext2 : IMessageContext2
    {
        public SitePlugin.ISiteMessage Message { get; }

        public IMessageMetadata2 Metadata { get; }

        public IMessageMethods Methods { get; }
        public YouTubeLiveMessageContext2(IYouTubeLiveMessage message, YouTubeLiveMessageMetadata2 metadata, IMessageMethods methods)
        {
            Message = message;
            Metadata = metadata;
            Methods = methods;
        }
    }
}
