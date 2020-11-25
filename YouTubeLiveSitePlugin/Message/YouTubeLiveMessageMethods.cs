using SitePlugin;
using System.Threading.Tasks;

namespace YouTubeLiveSitePlugin
{
    internal class YouTubeLiveMessageMethods : IMessageMethods
    {
        public Task AfterCommentAdded()
        {
            return Task.CompletedTask;
        }
    }
}
