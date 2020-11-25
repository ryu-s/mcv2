using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SitePluginCommon
{
    public abstract class CommentProviderBase2 : ICommentProvider
    {
        private bool _canConnect;
        public bool CanConnect
        {
            get { return _canConnect; }
            set
            {
                if (_canConnect == value)
                    return;
                _canConnect = value;
                CanConnectChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _canDisconnect;
        private readonly ILogger _logger;

        public bool CanDisconnect
        {
            get { return _canDisconnect; }
            set
            {
                if (_canDisconnect == value)
                    return;
                _canDisconnect = value;
                CanDisconnectChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public SitePluginId SiteContextGuid { get; set; }
        protected virtual CookieContainer GetCookieContainer(IBrowserProfile2 browserProfile, string domain)
        {
            var cc = new CookieContainer();
            try
            {
                var cookies = browserProfile.GetCookieCollection(domain);
                foreach (var cookie in cookies)
                {
                    cc.Add(cookie);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            return cc;
        }
        protected void SendSystemInfo(string message, InfoType type)
        {
            var context = InfoMessageContext2.Create(new InfoMessage
            {
                Text = message,
                SiteType = SiteType.Periscope,
                Type = type,
            });
            MessageReceived?.Invoke(this, context);
        }
        public event EventHandler<IMetadata> MetadataUpdated;
        public event EventHandler CanConnectChanged;
        public event EventHandler CanDisconnectChanged;
        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<IMessageContext2> MessageReceived;
        protected void RaiseMessageReceived(IMessageContext2 context)
        {
            MessageReceived?.Invoke(this, context);
        }
        protected void RaiseMetadataUpdated(IMetadata metadata)
        {
            MetadataUpdated?.Invoke(this, metadata);
        }
        public abstract Task ConnectAsync(string input, IBrowserProfile2 browserProfile);

        public abstract void Disconnect();

        public abstract Task PostCommentAsync(string text);

        public abstract Task<ICurrentUserInfo> GetCurrentUserInfo(IBrowserProfile2 browserProfile);
        protected virtual void BeforeConnect()
        {
            CanConnect = false;
            CanDisconnect = true;
        }
        protected virtual void AfterDisconnected()
        {
            CanConnect = true;
            CanDisconnect = false;
        }

        public abstract void SetMessage(string raw);

        public CommentProviderBase2(ILogger logger)
        {
            _logger = logger;

            CanConnect = true;
            CanDisconnect = false;
        }
    }
}
