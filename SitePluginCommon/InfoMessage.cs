using Common;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SitePluginCommon
{
    public interface IInfoMessage : ISiteMessage
    {
        InfoType Type { get; set; }
        string Text { get; }
        DateTime CreatedAt { get; }
    }
    public class InfoMessage : IInfoMessage
    {
        public InfoType Type { get; set; }
        public string Raw { get; }
        public SiteType SiteType { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; } = DateTime.Now;

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
    }
    public class InfoMessageMetadata2 : IMessageMetadata2
    {
        private readonly IInfoMessage _infoMessage;
        public bool IsNgUser => false;
        public bool IsSiteNgUser => false;
        public bool IsFirstComment => false;
        public bool IsInitialComment => false;
        public bool Is184 => false;
        public SitePluginId SiteContextGuid { get; set; }
        public string UserId { get; }
        public IEnumerable<IMessagePart> UserName { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public SiteType SiteType => SiteType.Unknown;//発生元のサイトにするべき？
        public InfoMessageMetadata2(IInfoMessage infoMessage)
        {
            _infoMessage = infoMessage;
        }
    }
    public class InfoMessageMethods : IMessageMethods
    {
        public Task AfterCommentAdded()
        {
            return Task.CompletedTask;
        }
    }
    public class InfoMessageContext2 : IMessageContext2
    {
        public SitePlugin.ISiteMessage Message { get; }

        public IMessageMetadata2 Metadata { get; }
        public InfoMessageContext2(IInfoMessage message, InfoMessageMetadata2 metadata)
        {
            Message = message;
            Metadata = metadata;
        }
        public static InfoMessageContext2 Create(InfoMessage message)
        {
            var metadata = new InfoMessageMetadata2(message);
            var context = new InfoMessageContext2(message, metadata);
            return context;

        }
    }
}
