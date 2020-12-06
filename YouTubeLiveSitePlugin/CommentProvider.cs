using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using SitePlugin;
using ryu_s.BrowserCookie;
using Common;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Concurrent;
using System.Linq;
using SitePluginCommon;
using System.Net.Http;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using mcv2;

namespace YouTubeLiveSitePlugin
{
    class CommentProvider : ICommentProvider
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
        public event EventHandler? LoggedInStateChanged;
        public bool IsLoggedIn
        {
            get { return _connection != null ? _connection.IsLoggedIn : false; }
        }
        public event EventHandler<IMessageContext2>? MessageReceived;
        public event EventHandler<IMetadata>? MetadataUpdated;
        public event EventHandler? CanConnectChanged;
        public event EventHandler? CanDisconnectChanged;
        public event EventHandler<ConnectedEventArgs>? Connected;

        CookieContainer _cc;
        private readonly IYouTubeLiveSiteOptions _siteOptions;
        private readonly ILogger _logger;
        //ChatProvider _chatProvider;
        //IMetadataProvider _metaProvider;
        //IActiveCounter<string> _activeCounter;
        private void SendInfo(string comment, InfoType type)
        {
            var context = InfoMessageContext2.Create(new InfoMessage
            {
                Text = comment,
                SiteType = SiteType.YouTubeLive,
                Type = type,
            });
            MessageReceived?.Invoke(this, context);
        }
        private void BeforeConnect()
        {
            CanConnect = false;
            CanDisconnect = true;
            _receivedCommentIds = CreateReicedCommentIdsCollection();
            _disconnectedByUser = false;
            //_userCommentCountDict = new Dictionary<string, int>();
        }
        protected virtual SynchronizedCollection<string> CreateReicedCommentIdsCollection()
        {
            return new SynchronizedCollection<string>();
        }
        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        private void ActiveCounter_Updated(object sender, int e)
        {
            MetadataUpdated?.Invoke(this, new Metadata { Active = e.ToString() });
        }

        private void AfterConnect()
        {
            //_chatProvider = null;
            CanConnect = true;
            CanDisconnect = false;
            _connection = null;
            _elapsedTimer.Enabled = false;
            SendInfo("切断しました", InfoType.Notice);
        }



        protected virtual CookieContainer CreateCookieContainer(IBrowserProfile2? browserProfile)
        {
            var cc = new CookieContainer();//まずCookieContainerのインスタンスを作っておく。仮にCookieの取得で失敗しても/live_chatで"YSC"と"VISITOR_INFO1_LIVE"が取得できる。これらは/service_ajaxでメタデータを取得する際に必須。
            if (browserProfile == null)
            {
                return cc;
            }
            try
            {
                var cookies = browserProfile.GetCookieCollection("youtube.com");
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
        EachConnection? _connection;
        DateTime? _startedAt;
        System.Timers.Timer _elapsedTimer = new System.Timers.Timer();
        public async Task ConnectAsync(string input, IBrowserProfile2? browserProfile)
        {
            if (string.IsNullOrEmpty(input))
            {
                SendInfo("チャンネルもしくは配信ページのURLを入力してください", InfoType.Error);
                //例示も必要だと思う。何を入力すれば良いのか分からない人が結構いる。
                return;
            }
            BeforeConnect();
            MetadataUpdated?.Invoke(this, new Metadata
            {
                Active = "-",
                CurrentViewers = "-",
                Elapsed = "-",
                Title = "-",
                TotalViewers = "-",
            });
            string? vid = null;
            bool isInputStoringNeeded = false;
            var resolver = new VidResolver();
            try
            {
                var result = await resolver.GetVid(_server, input).ConfigureAwait(false);
                if (result is MultiVidsResult multi)
                {
                    SendInfo("このチャンネルでは複数のライブが配信中です。", InfoType.Notice);
                    foreach (var v in multi.Vids)
                    {
                        SendInfo(v, InfoType.Notice);//titleも欲しい
                    }
                }
                else if (result is VidResult vidResult)
                {
                    vid = vidResult.Vid;
                }
                else if (result is NoVidResult no)
                {
                    SendInfo("このチャンネルでは生放送をしていないようです", InfoType.Error);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            catch (HttpRequestException ex)
            {
                SendInfo("Http error", InfoType.Error);
                _logger.LogException(ex, "Http error", "input=" + input);
                AfterConnect();
                return;
            }
            catch (YtInitialDataNotFoundException ex)
            {
                SendInfo("ytInitialDataが存在しません", InfoType.Error);
                _logger.LogException(ex);
                AfterConnect();
                return;
            }
            catch (Exception ex)
            {
                SendInfo("入力されたURLは存在しないか無効な値です", InfoType.Error);
                _logger.LogException(ex, "Invalid input", "input=" + input);
                AfterConnect();
                return;
            }
            if (string.IsNullOrEmpty(vid))
            {
                AfterConnect();
                return;
            }
            if (resolver.IsChannel(input) || resolver.IsCustomChannel(input) || resolver.IsUser(input))
            {
                isInputStoringNeeded = true;
            }
            var html = await _server.GetAsync($"https://www.youtube.com/watch?v={vid}").ConfigureAwait(false);
            var liveBroadcastDetails = Tools.ExtractLiveBroadcastDetailsFromLivePage(html);
            if (liveBroadcastDetails != null)
            {
                dynamic? d = Newtonsoft.Json.JsonConvert.DeserializeObject(liveBroadcastDetails);
                if (d != null && d!.ContainsKey("startTimestamp"))
                {
                    var startedStr = (string)d!.startTimestamp;
                    _startedAt = DateTime.Parse(startedStr);
                    _elapsedTimer.Interval = 500;
                    _elapsedTimer.Elapsed += (s, e) =>
                    {
                        if (!_startedAt.HasValue) return;
                        var elapsed = DateTime.Now - _startedAt.Value;
                        MetadataUpdated?.Invoke(this, new Metadata
                        {
                            Elapsed = Tools.ToElapsedString(elapsed),
                        });
                    };
                    _elapsedTimer.Enabled = true;
                }
            }
            _cc = CreateCookieContainer(browserProfile);
            var userCommentCountDict = CreateUserCommentCountDict();
            _connection = CreateConnection(_logger, _cc, _server, _siteOptions, userCommentCountDict, _receivedCommentIds, this, SiteContextGuid);
            _connection.Connected += (s, e) =>
            {
                Connected?.Invoke(this, new ConnectedEventArgs { IsInputStoringNeeded = isInputStoringNeeded });
            };
            _connection.MessageReceived += (s, e) => MessageReceived?.Invoke(this, e);
            _connection.MetadataUpdated += (s, e) => MetadataUpdated?.Invoke(this, e);
            _connection.LoggedInStateChanged += (s, e) => LoggedInStateChanged?.Invoke(this, e);
            var reloadManager = new ReloadManager()
            {
                CountLimit = 5,
                CountCheckTimeRangeMin = 1,
            };
        reload:
            if (_disconnectedByUser)
            {
                AfterConnect();
                return;
            }
            if (!reloadManager.CanReload())
            {
                SendInfo($"{reloadManager.CountCheckTimeRangeMin}分以内に{reloadManager.CountLimit}回再接続を試みました。サーバーに余計な負荷を掛けるのを防ぐため自動再接続を中断します", InfoType.Error);
                AfterConnect();
                return;
            }
            reloadManager.SetTime();

            try
            {
                var browserType = browserProfile == null ? BrowserType.Unknown : browserProfile.Type;
                var disconnectReason = await ReceiveConnectionAsync(_connection, browserType, vid).ConfigureAwait(false);
                switch (disconnectReason)
                {
                    case DisconnectReason.Reload:
                        SendInfo("エラーが発生したためサーバーとの接続が切断されましたが、自動的に再接続します", InfoType.Error);
                        goto reload;
                    case DisconnectReason.ByUser:
                        SendInfo("ユーザーが切断ボタンを押したため切断しました", InfoType.Debug);
                        break;
                    case DisconnectReason.Finished:
                        SendInfo("配信が終了しました", InfoType.Notice);
                        break;
                    case DisconnectReason.ChatUnavailable:
                        SendInfo("この配信ではチャットが無効になっているようです", InfoType.Error);
                        break;
                    case DisconnectReason.YtInitialDataNotFound:
                        SendInfo("ytInitialDataの取得に失敗しました", InfoType.Error);
                        break;
                    case DisconnectReason.ServerError:
                        SendInfo("サーバでエラーが発生したため接続できませんでした", InfoType.Error);
                        break;
                    case DisconnectReason.Unknown:
                        SendInfo("原因不明のエラーが発生したため切断されましたが、自動的に再接続します", InfoType.Error);
                        goto reload;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "", $"input={input}");
                SendInfo("回復不能なエラーが発生しました", InfoType.Error);
            }
            AfterConnect();
        }

        protected virtual Task<DisconnectReason> ReceiveConnectionAsync(EachConnection connection, BrowserType browserType, string vid)
        {
            return connection.ReceiveAsync(vid, browserType);
        }

        protected virtual Dictionary<string, int> CreateUserCommentCountDict()
        {
            return new Dictionary<string, int>();
        }

        protected virtual EachConnection CreateConnection(ILogger logger, CookieContainer cc, IYouTubeLiveServer server,
            IYouTubeLiveSiteOptions siteOptions, Dictionary<string, int> userCommentCountDict, SynchronizedCollection<string> receivedCommentIds,
            ICommentProvider cp, SitePluginId siteContextGuid)
        {
            return new EachConnection(logger, cc, server, siteOptions, userCommentCountDict, receivedCommentIds, cp)
            {
                SiteContextGuid = siteContextGuid,
            };
        }

        protected virtual DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }


        SynchronizedCollection<string> _receivedCommentIds;
        bool _disconnectedByUser;
        public void Disconnect()
        {
            _connection?.Disconnect();
            _disconnectedByUser = true;
        }

        async Task ICommentProvider.PostCommentAsync(string text)
        {
            var b = await PostCommentAsync(text);
        }
        public async Task<bool> PostCommentAsync(string text)
        {
            if (_connection == null) return false;
            return await _connection.PostCommentAsync(text);
        }
        public async Task PostCommentAsync(ICommentDataToPost dataToPost)
        {
            if (!(dataToPost is IYouTubeLiveCommentDataToPost ytCommentDataToPost))
            {
                return;
            }
            var _ = await PostCommentAsync(ytCommentDataToPost.Message);
        }
        public async Task<ICurrentUserInfo> GetCurrentUserInfo(IBrowserProfile2? browserProfile)
        {
            var currentUserInfo = new CurrentUserInfo();
            var cc = CreateCookieContainer(browserProfile);
            var url = "https://www.youtube.com/embed";
            var html = await _server.GetAsync(url, cc);
            //"user_display_name":"Ryu"
            var match = Regex.Match(html, "\"user_display_name\":\"([^\"]+)\"");
            if (match.Success)
            {
                var name = match.Groups[1].Value;
                currentUserInfo.Username = name;
                currentUserInfo.IsLoggedIn = true;
            }
            else
            {
                currentUserInfo.IsLoggedIn = false;
                currentUserInfo.Username = null;
            }
            return currentUserInfo;
        }

        public void SetMessage(string raw)
        {

        }

        public SitePluginId SiteContextGuid { get; set; }
        IYouTubeLiveServer _server;
        public CommentProvider(IYouTubeLiveServer server, IYouTubeLiveSiteOptions siteOptions, ILogger logger)
        {
            _siteOptions = siteOptions;
            _logger = logger;
            _server = server;

            CanConnect = true;
            CanDisconnect = false;
        }
    }
    class PostCommentContext2
    {
        public string InnerTubeContext { get; }
        public string InnerTubeApiKey { get; }

        public PostCommentContext2(string innerTubeContext, string innerTubeApiKey)
        {
            InnerTubeContext = innerTubeContext;
            InnerTubeApiKey = innerTubeApiKey;
        }
    }
    class PostCommentContext
    {
        public string SessionToken { get; set; }
        public string Sej { get; set; }
        public string ClientIdPrefix { get; set; }
    }
    class CurrentUserInfo : ICurrentUserInfo
    {
        public string? Username { get; set; }
        public string? UserId { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
