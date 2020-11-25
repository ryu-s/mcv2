using System;
using SitePlugin;
using Common;
using System.Windows.Controls;

namespace SitePluginCommon
{
    public abstract class SiteContextBase : ISiteContext
    {
        private readonly ILogger _logger;

        public abstract SiteType SiteType { get; }
        public abstract SitePluginId Guid { get; }
        public abstract string DisplayName { get; }
        public abstract ICommentProvider2 CreateCommentProvider();

        public virtual void Init()
        {
        }

        public abstract bool IsValidInput(string input);

        public abstract void LoadOptions(string path, IIo io);

        public virtual void Save()
        {
        }

        public abstract void SaveOptions(string path, IIo io);
        public SiteContextBase(ILogger logger)
        {
            _logger = logger;
        }
    }
}
