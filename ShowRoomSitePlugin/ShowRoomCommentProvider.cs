using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using SitePluginCommon;
using SitePluginCommon.AutoReconnector;

namespace ShowRoomSitePlugin
{

    class PeriscopeConnector : IConnector
    {
        System.Timers.Timer _pingTimer = new System.Timers.Timer();
        private readonly IDataServer _server;
        private readonly string _broadcastId;
        private readonly IMessageProvider _messageProvider;
        private readonly ILogger _logger;
        DisconnectReason _disconnectReason = DisconnectReason.Unknown;
        public async Task<DisconnectReason> ConnectAsync()
        {
            _disconnectReason = DisconnectReason.Unknown;
            var liveInfo = await Api.GetLiveInfo(_server, _broadcastId);
            _pingTimer.Enabled = true;
            try
            {
                await _messageProvider.ReceiveAsync(liveInfo.BcsvrHost, liveInfo.BcsvrKey);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            _pingTimer.Enabled = false;
            return _disconnectReason;
        }
        public void Disconnect()
        {
            _messageProvider.Disconnect();
            _disconnectReason = DisconnectReason.User;
        }

        public async Task<bool> IsLivingAsync()
        {
            var liveInfo = await Api.GetLiveInfo(_server, _broadcastId);
            return IsBroadcastRunning(liveInfo);
        }
        private bool IsBroadcastRunning(LiveInfo liveInfo)
        {
            Debug.WriteLine($"SHOWROOM LiveInfo.LiveStatus: {liveInfo.LiveStatus}");
            return liveInfo.LiveStatus == 2;
        }
        public PeriscopeConnector(IDataServer server, string broadcastId, IMessageProvider messageProvider, ILogger logger)
        {
            _server = server;
            _broadcastId = broadcastId;
            _messageProvider = messageProvider;
            _logger = logger;
            _pingTimer.Interval = 1 * 60 * 1000;
            _pingTimer.Elapsed += PingTimer_Elapsed;
        }
        private async void PingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                await _messageProvider.SendAsync("PING\tshowroom");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }
    }

    internal class ShowRoomCommentProvider2 : CommentProviderBase2
    {
        private MessageMetadata2 CreateMessageMetadata(IShowRoomComment message, bool isFirstComment, string? nickname = null)
        {
            return new MessageMetadata2(message, _siteOptions, isFirstComment)
            {
                SiteContextGuid = SiteContextGuid,
                UserId = message.UserId,
                UserName = Common.MessagePartFactory.CreateMessageItems(message.UserName),
                NewNickname = nickname,
            };
        }
        private static string? ExtractNickname(bool isAutoSetNickname, string message)
        {
            if (isAutoSetNickname)
            {
                return SitePluginCommon.Utils.ExtractNickname(message);
            }
            else
            {
                return null;
            }
        }
        private void MessageProvider_Received(object sender, IInternalMessage e)
        {
            switch (e)
            {
                case T1 t1:
                    {
                        if (_siteOptions.IsIgnore50Counts && int.TryParse(t1.Cm, out int count))
                        {
                            if (count >= 1 && count <= 50)
                            {
                                //1-50の数字のみのコメントは無視する
                                return;
                            }
                        }
                        var message = new ShowRoomComment(t1);
                        var userId = message.UserId;
                        var isFirstComment = _first.IsFirstComment(userId);
                        //var user = GetUser(userId);
                        //user.Name = Common.MessagePartFactory.CreateMessageItems(message.Text);
                        var nickname = ExtractNickname(_siteOptions.IsAutoSetNickname, t1.Cm);
                        var metadata = CreateMessageMetadata(message, isFirstComment, nickname);
                        var methods = new MessageMethods();
                        RaiseMessageReceived(new MessageContext2(message, metadata, methods));
                    }
                    break;
            }
        }
        protected override void BeforeConnect()
        {
            base.BeforeConnect();
        }
        protected override void AfterDisconnected()
        {
            base.AfterDisconnected();
            _autoReconnector = null;
        }
        AutoReconnector _autoReconnector;
        private async Task ConnectInternalAsync(string input, IBrowserProfile2 browserProfile)
        {
            var roomName = GetRoomNameFromInput(input);
            if (string.IsNullOrEmpty(roomName))
            {
                throw new Exception("invalid input");
            }
            var livePageUrl = "https://www.showroom-live.com/" + roomName;
            var livePageHtml = await _server.GetAsync(livePageUrl);
            var match = Regex.Match(livePageHtml, "room_id=(\\d+)");
            if (!match.Success)
            {
                throw new Exception("room_idが無い");
            }
            var room_id = match.Groups[1].Value;

            var messageProvider = new MessageProvider(new Websocket(), _logger);
            messageProvider.Received += MessageProvider_Received;
            var connector = new PeriscopeConnector(_server, room_id, messageProvider, _logger);
            var me = new MessageUntara();
            me.SystemInfoReiceved += Me_SystemInfoReiceved;
            _autoReconnector = new AutoReconnector(connector, me);
            await _autoReconnector.AutoConnect();
            me.SystemInfoReiceved -= Me_SystemInfoReiceved;
            messageProvider.Received -= MessageProvider_Received;
            //var liveInfo = await Api.GetLiveInfo(_server, room_id);
            //if(liveInfo.LiveStatus == 1)
            //{
            //    //放送終了？
            //    return;
            //}
            //_pingTimer.Enabled = true;
            //try
            //{
            //    await _messageProvider.ReceiveAsync(liveInfo.BcsvrHost, liveInfo.BcsvrKey);
            //}
            //finally
            //{
            //    _pingTimer.Enabled = false;
            //}
            //return;
        }
        private void Me_SystemInfoReiceved(object sender, SystemInfoEventArgs e)
        {
            SendSystemInfo(e.Message, e.Type);
        }
        private string GetRoomNameFromInput(string input)
        {
            var match = Regex.Match(input, "showroom-live.com/([^/?#]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }
        public override async Task ConnectAsync(string input, IBrowserProfile2 browserProfile)
        {
            BeforeConnect();
            try
            {
                await ConnectInternalAsync(input, browserProfile);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "", $"input={input}");
            }
            finally
            {
                AfterDisconnected();
            }
        }
        FirstCommentDetector _first = new FirstCommentDetector();
        public override void Disconnect()
        {
            _autoReconnector?.Disconnect();
        }

        public override async Task PostCommentAsync(string text)
        {
            await Task.FromResult<object>(null);
        }

        public override async Task<ICurrentUserInfo> GetCurrentUserInfo(IBrowserProfile2 browserProfile)
        {
            var userInfo = new CurrentUserInfo
            {

            };
            return await Task.FromResult(userInfo);
        }

        public override void SetMessage(string raw)
        {
            throw new NotImplementedException();
        }

        private readonly IDataServer _server;
        private readonly ILogger _logger;
        private readonly IShowRoomSiteOptions _siteOptions;
        public ShowRoomCommentProvider2(IDataServer server, ILogger logger, IShowRoomSiteOptions siteOptions)
            : base(logger)
        {
            _server = server;
            _logger = logger;
            _siteOptions = siteOptions;
        }
    }
    class CurrentUserInfo : ICurrentUserInfo
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
