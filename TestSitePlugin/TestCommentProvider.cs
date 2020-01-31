using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestSitePlugin
{
    public class TestCommentProvider2 : ICommentProvider2
    {
        #region CanConnect
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
        #endregion //CanConnect

        #region CanDisconnect
        private bool _canDisconnect;
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
        #endregion //CanDisconnect

        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<IMessageContext2> MessageReceived;
        public event EventHandler<IMetadata> MetadataUpdated;
        public event EventHandler CanConnectChanged;
        public event EventHandler CanDisconnectChanged;
        CancellationTokenSource _cts;

        public async Task ConnectAsync(string input, IBrowserProfile2 browserProfile)
        {
            if (_cts != null)
            {
                throw new InvalidOperationException("_cts is not null!");
            }
            _cts = new CancellationTokenSource();
            CanConnect = false;
            CanDisconnect = true;
            Connected?.Invoke(this, new ConnectedEventArgs
            {
                IsInputStoringNeeded = false,
            });
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, _cts.Token);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (TaskCanceledException) { break; }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            CanConnect = true;
            CanDisconnect = false;
            _cts = null;
        }

        public void Disconnect()
        {
            _cts?.Cancel();
        }
        class CurrentUserInfo : ICurrentUserInfo
        {
            public string Username { get; set; }
            public bool IsLoggedIn { get; set; }
        }
        public Task<ICurrentUserInfo> GetCurrentUserInfo(IBrowserProfile2 browserProfile)
        {
            ICurrentUserInfo info = new CurrentUserInfo
            {
                Username = "test",
                IsLoggedIn = true,
            };
            return Task.FromResult(info);
        }

        public Task PostCommentAsync(string text)
        {
            var arr = text.Split(',');
            if (arr.Length == 2)
            {
                var userId = arr[0];
                var content = arr[1];
                var comment = new TestComment
                {
                    UserId = userId,
                    Text = content,
                };
                var metadata = new TestMetadata2(userId)
                {
                    SiteContextGuid = SiteContextGuid,
                };
                var methods = new TestMethods();
                var context = new MessageContext2(comment, metadata, methods);
                MessageReceived?.Invoke(this, context);
            }
            else if (arr.Length == 3)
            {
                var userId = arr[0];
                var name = arr[1];
                var content = arr[2];
                var comment = new TestComment
                {
                    UserId = userId,
                    UserName = name,
                    Text = content,
                };
                var metadata = new TestMetadata2(userId)
                {
                    SiteContextGuid = SiteContextGuid,
                };
                var methods = new TestMethods();
                var context = new MessageContext2(comment, metadata, methods);
                MessageReceived?.Invoke(this, context);
            }
            else
            {
                SendSystemInfo(text, InfoType.Notice);
            }
            return Task.CompletedTask;
        }
        private void SendSystemInfo(string message, InfoType type)
        {
            var context = InfoMessageContext2.Create(new InfoMessage
            {
                Text = message,
                SiteType = SiteType.Unknown,
                Type = type,
            });
            MessageReceived?.Invoke(this, context);
        }

        public void SetMessage(string raw)
        {
            throw new NotImplementedException();
        }

        public SitePluginId SiteContextGuid { get; set; }
        private readonly ILogger _logger;
        public TestCommentProvider2(ILogger logger)
        {
            _logger = logger;
            CanConnect = true;
            CanDisconnect = false;
        }
    }
}
