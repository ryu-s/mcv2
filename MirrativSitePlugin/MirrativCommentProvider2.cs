using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Net;
using System.Threading.Tasks;
using SitePluginCommon.AutoReconnection;
namespace MirrativSitePlugin
{
    class MirrativCommentProvider33 : CommentProviderBase2
    {
        FirstCommentDetector _first = new FirstCommentDetector();
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        private readonly IMirrativSiteOptions _siteOptions;

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
        NewAutoReconnector _autoReconnector;
        /// <summary>
        /// 放送IDを取得する
        /// </summary>
        /// <param name="input"></param>
        /// <returns>ユーザページのURLを入力した場合に配信中で無ければnull</returns>
        private async Task<string> GetCurrentLiveIdAsync(string input)
        {
            string liveId = null;
            if (Tools.IsValidUserId(input))
            {
                var userId = Tools.ExtractUserId(input);
                var userProfile = await Api.GetUserProfileAsync(_server, userId);
                if (!string.IsNullOrEmpty(userProfile.OnLiveLiveId))
                {
                    liveId = userProfile.OnLiveLiveId;
                }
            }
            else if (Tools.IsValidLiveId(input))
            {
                liveId = Tools.ExtractLiveId(input);
            }
            return liveId;
        }
        MessageProvider2 _p1;
        MetadataProvider2 _p2;
        bool _isInitialized;
        public async Task InitAsync()
        {
            if (_isInitialized) return;
            var p1 = new MessageProvider2(new WebSocket("wss://online.mirrativ.com/"), _logger);
            p1.MessageReceived += P1_MessageReceived;
            p1.MetadataUpdated += P1_MetadataUpdated;
            _p1 = p1;
            var p2 = new MetadataProvider2(_server, _siteOptions);
            p2.MetadataUpdated += P2_MetadataUpdated;
            p2.Master = p1;
            _p2 = p2;
        }
        private async Task ConnectInternalAsync(string input, IBrowserProfile2 browserProfile)
        {
            await InitAsync();
            //var p1 = new MessageProvider2(new WebSocket("wss://online.mirrativ.com/"), _logger);
            //p1.MessageReceived += P1_MessageReceived;
            //p1.MetadataUpdated += P1_MetadataUpdated;
            //var p2 = new MetadataProvider2(_server, _siteOptions);
            //p2.MetadataUpdated += P2_MetadataUpdated;
            //p2.Master = p1;
            try
            {
                var dummy = new DummyImpl(_server, input, _logger, _siteOptions, _p1, _p2);
                var connectionManager = new ConnectionManager(_logger);
                _autoReconnector = new NewAutoReconnector(connectionManager, dummy, new MessageUntara(), _logger);

                //isInitialCommentを取得する
                var liveId = await GetCurrentLiveIdAsync(input);
                if (!string.IsNullOrEmpty(liveId))
                {
                    var initialComments = await Api.GetLiveComments(_server, liveId);
                    foreach (var c in initialComments)
                    {
                        var userId = c.UserId;
                        var isFirstComment = _first.IsFirstComment(userId);
                        var context = CreateMessageContext(new MirrativComment(c, ""), true);
                        RaiseMessageReceived(context);
                    }
                }

                //接続開始
                await _autoReconnector.AutoReconnectAsync();
            }
            finally
            {
                //p1.MessageReceived -= P1_MessageReceived;
                //p1.MetadataUpdated -= P1_MetadataUpdated;
                //p2.MetadataUpdated -= P2_MetadataUpdated;
            }
        }

        private void P2_MetadataUpdated(object sender, ILiveInfo e)
        {
            var liveInfo = e;
            RaiseMetadataUpdated(new Metadata
            {
                IsLive = liveInfo.IsLive,
                Title = liveInfo.Title,
                TotalViewers = liveInfo.TotalViewerNum.ToString(),
                CurrentViewers = liveInfo.OnlineUserNum.ToString(),
            });

        }

        private void P1_MetadataUpdated(object sender, IMetadata e)
        {
            var metadata = e;
            RaiseMetadataUpdated(metadata);
        }

        private void P1_MessageReceived(object sender, IMirrativMessage e)
        {
            var message = e;
            var messageContext = CreateMessageContext(message, false);
            if (messageContext != null)
            {
                RaiseMessageReceived(messageContext);
            }
        }
        private MirrativMessageContext2 CreateMessageContext(IMirrativMessage message, bool isInitialComment)
        {
            if (message is IMirrativComment comment)
            {
                var userId = comment.UserId;
                var isFirst = _first.IsFirstComment(userId);
                var metadata = new CommentMessageMetadata2(comment, _siteOptions, isFirst)
                {
                    IsInitialComment = isInitialComment,
                    SiteContextGuid = SiteContextGuid,
                    UserId = userId,
                    UserName = MessagePartFactory.CreateMessageItems(comment.UserName),
                };
                var methods = new MirrativMessageMethods();
                if (_siteOptions.NeedAutoSubNickname)
                {
                    var messageText = comment.Text;
                    var nick = SitePluginCommon.Utils.ExtractNickname(messageText);
                    if (!string.IsNullOrEmpty(nick))
                    {
                        //user.Nickname = nick;
                    }
                }
                return new MirrativMessageContext2(comment, metadata, methods);
            }
            else if (message is IMirrativJoinRoom join && _siteOptions.IsShowJoinMessage)
            {
                var userId = join.UserId;
                var metadata = new JoinMessageMetadata2(join, _siteOptions)
                {
                    IsInitialComment = isInitialComment,
                    SiteContextGuid = SiteContextGuid,
                    UserId = userId,
                    UserName = MessagePartFactory.CreateMessageItems(join.UserName),
                };
                var methods = new MirrativMessageMethods();
                return new MirrativMessageContext2(join, metadata, methods);
            }
            else if (message is IMirrativItem item)
            {
                var userId = item.UserId;
                var isFirst = _first.IsFirstComment(userId);
                var metadata = new ItemMessageMetadata2(item, _siteOptions)
                {
                    IsInitialComment = isInitialComment,
                    SiteContextGuid = SiteContextGuid,
                    UserId = userId,
                    UserName = MessagePartFactory.CreateMessageItems(item.UserName),
                };
                var methods = new MirrativMessageMethods();
                if (_siteOptions.NeedAutoSubNickname)
                {
                    var messageText = item.Text;
                    var nick = SitePluginCommon.Utils.ExtractNickname(messageText);
                    if (!string.IsNullOrEmpty(nick))
                    {
                        //user.Nickname = nick;
                    }
                }
                return new MirrativMessageContext2(item, metadata, methods);
            }
            else if (message is IMirrativConnected connected)
            {
                var metadata = new ConnectedMessageMetadata2(connected, _siteOptions)
                {
                    IsInitialComment = isInitialComment,
                    SiteContextGuid = SiteContextGuid,
                };
                var methods = new MirrativMessageMethods();
                return new MirrativMessageContext2(connected, metadata, methods);
            }
            else if (message is IMirrativDisconnected disconnected)
            {
                var metadata = new DisconnectedMessageMetadata2(disconnected, _siteOptions)
                {
                    IsInitialComment = isInitialComment,
                    SiteContextGuid = SiteContextGuid,
                };
                var methods = new MirrativMessageMethods();
                return new MirrativMessageContext2(disconnected, metadata, methods);
            }
            else
            {
                return null;
            }
        }
        public override void Disconnect()
        {
            _autoReconnector?.Disconnect();
        }
        protected virtual CookieContainer GetCookieContainer(IBrowserProfile2 browserProfile)
        {
            var cc = new CookieContainer();

            try
            {
                var cookies = browserProfile.GetCookieCollection("mirrativ.com");
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
        public override async Task<ICurrentUserInfo> GetCurrentUserInfo(IBrowserProfile2 browserProfile)
        {
            var cc = GetCookieContainer(browserProfile);
            var currentUser = await Api.GetCurrentUserAsync(_server, cc);

            return new CurrentUserInfo
            {
                IsLoggedIn = currentUser.IsLoggedIn,
                UserId = currentUser.UserId,
                Username = currentUser.Name,
            };
        }

        public override Task PostCommentAsync(string text)
        {
            throw new NotImplementedException();
        }

        public override async void SetMessage(string raw)
        {
            await InitAsync();
            _p1.SetMessage(raw);
        }

        public MirrativCommentProvider33(IDataServer server, ILogger logger, IMirrativSiteOptions siteOptions)
            : base(logger)
        {
            _server = server;
            _logger = logger;
            _siteOptions = siteOptions;
        }
    }
}
