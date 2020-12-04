using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using SitePlugin;
using Common;
using System.Linq;
using SitePluginCommon;
using ryu_s.BrowserCookie;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace YouTubeLiveSitePlugin
{
    /// <summary>
    /// 接続が切れた理由
    /// </summary>
    enum DisconnectReason
    {
        /// <summary>
        /// 原因不明
        /// </summary>
        Unknown,
        /// <summary>
        /// 配信終了
        /// </summary>
        Finished,
        /// <summary>
        /// ユーザによる切断
        /// </summary>
        ByUser,
        /// <summary>
        /// リロードが必要
        /// </summary>
        Reload,
        /// <summary>
        /// チャットが無効
        /// </summary>
        ChatUnavailable,
        /// <summary>
        /// YtInitialDataが無かった
        /// </summary>
        YtInitialDataNotFound,
        /// <summary>
        /// サーバ側でエラーが発生
        /// </summary>
        ServerError,
        InvalidInput,
    }
    class EachConnection
    {
        private readonly ILogger _logger;
        private readonly CookieContainer _cc;
        private readonly IYouTubeLiveServer _server;
        private readonly IYouTubeLiveSiteOptions _siteOptions;
        private readonly Dictionary<string, int> _userCommentCountDict;
        private readonly SynchronizedCollection<string> _receivedCommentIds;
        private readonly ICommentProvider _cp;
        ChatProvider? _chatProvider;
        DisconnectReason _disconnectReason;

        public event EventHandler<IMessageContext2>? MessageReceived;
        public event EventHandler<IMetadata>? MetadataUpdated;
        public event EventHandler Connected;
        public SitePluginId SiteContextGuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<DisconnectReason> ReceiveAsync(string vid, BrowserType browserType)
        {
            _disconnectReason = DisconnectReason.Unknown;
            string liveChatHtml;
            try
            {
                liveChatHtml = await GetLiveChatHtml(vid);
            }
            catch (InvalidInputException ex)
            {
                _logger.LogException(ex);
                return DisconnectReason.InvalidInput;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return DisconnectReason.ServerError;
            }
            string ytInitialData;
            try
            {
                ytInitialData = Tools.ExtractYtInitialDataFromLiveChatHtml(liveChatHtml);
            }
            catch (ParseException ex)
            {
                _logger.LogException(ex, "live_chatからのytInitialDataの抜き出しに失敗", liveChatHtml);
                return DisconnectReason.YtInitialDataNotFound;
            }
            IContinuation initialContinuation;
            ChatContinuation chatContinuation;
            List<CommentData> initialCommentData;
            try
            {
                (initialContinuation, chatContinuation, initialCommentData) = Tools.ParseYtInitialData(ytInitialData);
            }
            catch (YouTubeLiveServerErrorException ex)
            {
                _logger.LogException(ex, "サーバエラー", $"ytInitialData={ytInitialData},vid={vid}");
                return DisconnectReason.ServerError;
            }
            catch (ContinuationNotExistsException)
            {
                //放送終了
                return DisconnectReason.Finished;
            }
            catch (ChatUnavailableException)
            {
                //SendInfo("この配信ではチャットが無効になっているようです", InfoType.Error);
                return DisconnectReason.ChatUnavailable;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "未知の例外", $"ytInitialData={ytInitialData},vid={vid}");
                return DisconnectReason.Unknown;
            }

            Connected?.Invoke(this, EventArgs.Empty);

            //直近の過去コメントを送る。
            foreach (var data in initialCommentData)
            {
                if (_receivedCommentIds.Contains(data.Id))
                {
                    continue;
                }
                else
                {
                    _receivedCommentIds.Add(data.Id);
                }
                var messageContext = CreateMessageContext(data, true);
                MessageReceived?.Invoke(this, messageContext);
            }
            //コメント投稿に必要なものの準備
            PrepareForPostingComments(liveChatHtml, ytInitialData);

            var tasks = new List<Task>();
            Task? activeCounterTask = null;
            IActiveCounter<string>? activeCounter = null;
            //if (_options.IsActiveCountEnabled)
            //{
            //    activeCounter = new ActiveCounter<string>()
            //    {
            //        CountIntervalSec = _options.ActiveCountIntervalSec,
            //        MeasureSpanMin = _options.ActiveMeasureSpanMin,
            //    };
            //    activeCounter.Updated += (s, e) =>
            //    {
            //        MetadataUpdated?.Invoke(this, new Metadata { Active = e.ToString() });
            //    };
            //    activeCounterTask = activeCounter.Start();
            //    tasks.Add(activeCounterTask);
            //}

            var (metaTask, metaProvider) = CreateMetadataReceivingTask(browserType, vid, liveChatHtml);
            if (metaTask != null)
            {
                tasks.Add(metaTask);
            }

            _chatProvider = new ChatProvider(_server, _logger);
            _chatProvider.ActionsReceived += (s, e) =>
            {
                foreach (var action in e)
                {
                    if (_receivedCommentIds.Contains(action.Id))
                    {
                        continue;
                    }
                    else
                    {
                        activeCounter?.Add(action.Id);
                        _receivedCommentIds.Add(action.Id);
                    }

                    var messageContext = CreateMessageContext(action, false);
                    MessageReceived?.Invoke(this, messageContext);
                }
            };
            _chatProvider.InfoReceived += (s, e) =>
            {
                SendInfo(e.Comment, e.Type);
            };
            var continuation = _siteOptions.IsAllChat ? chatContinuation.AllChatContinuation : chatContinuation.JouiChatContinuation;
            var chatTask = _chatProvider.ReceiveAsync(continuation);
            tasks.Add(chatTask);

            _disconnectReason = DisconnectReason.Finished;
            while (tasks.Count > 0)
            {
                var t = await Task.WhenAny(tasks);
                if (t == metaTask)
                {
                    try
                    {
                        await metaTask;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex, "metaTaskが終了した原因");
                    }
                    //metaTask内でParseExceptionもしくはDisconnect()
                    //metaTaskは終わっても良い。
                    tasks.Remove(metaTask);
                    metaProvider = null;
                }
                else if (t == activeCounterTask)
                {
                    try
                    {
                        await activeCounterTask;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex, "activeCounterTaskが終了した原因");
                    }
                    tasks.Remove(activeCounterTask);
                    activeCounter = null;
                }
                else //chatTask
                {
                    tasks.Remove(chatTask);
                    try
                    {
                        await chatTask;
                    }
                    catch (ReloadException ex)
                    {
                        _logger.LogException(ex, "", $"vid={vid}");
                        _disconnectReason = DisconnectReason.Reload;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex);
                        _disconnectReason = DisconnectReason.Unknown;
                    }
                    _chatProvider = null;

                    //chatTaskが終わったらmetaTaskも終了させる
                    metaProvider?.Disconnect();
                    if (metaTask != null)
                    {
                        try
                        {
                            await metaTask;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException(ex, "metaTaskが終了した原因");
                        }
                        tasks.Remove(metaTask);
                    }
                    metaProvider = null;

                    activeCounter?.Stop();
                    if (activeCounterTask != null)
                    {
                        try
                        {
                            await activeCounterTask;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException(ex, "activeCounterTaskが終了した原因");
                        }
                        tasks.Remove(activeCounterTask);
                    }
                    activeCounter = null;
                }
            }
            return _disconnectReason;
        }

        public void Disconnect()
        {
            _chatProvider?.Disconnect();
            _disconnectReason = DisconnectReason.ByUser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="liveChatHtml"></param>
        /// <returns></returns>
        /// <exception cref="ReloadException"></exception>
        private async Task<string> GetLiveChatHtml(string vid)
        {
            var liveChatUrl = $"https://www.youtube.com/live_chat?v={vid}&is_popout=1";
            //live_chatを取得する。この中にこれから必要なytInitialDataとytcfgがある
            var res = await _server.GetNoThrowAsync(liveChatUrl, _cc);
            switch (res.StatusCode)
            {
                case HttpStatusCode.OK:
                    return res.Content;
                case HttpStatusCode.BadRequest:
                    throw new InvalidInputException("", $"url={liveChatUrl}");
                default:
                    {
                        var content = res.Content;
                        throw new ReloadException("", $"statuscode={res.StatusCode}");
                    }
            }
        }

        public event EventHandler? LoggedInStateChanged;
        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                if (_isLoggedIn == value) return;
                _isLoggedIn = value;
                LoggedInStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        bool CanPostComment => PostCommentContext != null;
        protected virtual PostCommentContext2? PostCommentContext { get; set; }
        public async Task<bool> PostCommentAsync(string text)
        {
            if (!CanPostComment) return false;
            var richMessage = "\"richMessage\":{\"textSegments\":[{\"text\":\"" + text + "\"}]}";
            var payload = Tools.AddRichTextToInnerTubeContext(PostCommentContext!.InnerTubeContext, richMessage);
            var url = "https://www.youtube.com/youtubei/v1/live_chat/send_message?key=" + PostCommentContext.InnerTubeApiKey;
            var res = await _server.PostJsonAsync(url, payload, _cc);
            return true;
        }
        private void SendInfo(string v, InfoType error)
        {
            var message = new InfoMessage
            {
                Type = error,
                Text = v,
                SiteType = SiteType.YouTubeLive,
            };
            var metadata = new InfoMessageMetadata2(message);
            var methods = new InfoMessageMethods();
            var context = new InfoMessageContext2(message, metadata);
            MessageReceived?.Invoke(this, context);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="browserType"></param>
        /// <param name="vid"></param>
        /// <param name="liveChatHtml"></param>
        /// <returns></returns>
        /// <exception cref="SpecChangedException">live_chatからのytcfgの抜き出しに失敗した場合</exception>
        protected virtual (Task?, IMetadataProvider) CreateMetadataReceivingTask(BrowserType browserType, string vid, string liveChatHtml)
        {
            IMetadataProvider metaProvider;
            string? ytCfg = null;
            try
            {
                ytCfg = Tools.ExtractYtcfg(liveChatHtml);
            }
            catch (ParseException ex)
            {
                throw new SpecChangedException(liveChatHtml, ex);
            }
            //"service_ajax?name=updatedMetadataEndpoint"はIEには対応していないらしく、400が返って来てしまう。
            //そこで、IEの場合のみ旧版の"youtubei"を使うようにした。
            if (browserType == BrowserType.IE)
            {
                metaProvider = new MetaDataYoutubeiProvider(_server, _logger);
            }
            else
            {
                metaProvider = new MetadataProvider(_server, _logger);
            }
            metaProvider.MetadataReceived += (s, e) =>
            {
                MetadataUpdated?.Invoke(this, e);
            };
            metaProvider.InfoReceived += (s, e) =>
            {
                SendInfo(e.Comment, e.Type);
            };
            var task = metaProvider.ReceiveAsync(ytCfg: ytCfg, vid: vid, cc: _cc);
            return (task, metaProvider);
        }
        private void PrepareForPostingComments(string liveChatHtml, string ytInitialData)
        {
            var innerTubeContext = Tools.GetInnerTubeContext(liveChatHtml);
            if (innerTubeContext == null)
            {
                throw new SpecChangedException(liveChatHtml);
            }

            var match = Regex.Match(liveChatHtml, "\"INNERTUBE_API_KEY\":\"([^\"]+)\"");
            var innerTubeApiKey = match.Groups[1].Value;
            PostCommentContext = new PostCommentContext2(innerTubeContext, innerTubeApiKey);
        }
        private YouTubeLiveMessageContext2 CreateMessageContext(CommentData commentData, bool isInitialComment)
        {
            IYouTubeLiveMessage message;
            IEnumerable<IMessagePart> commentItems;
            IEnumerable<IMessagePart> nameItems;

            if (commentData.IsPaidMessage)
            {
                var superchat = new YouTubeLiveSuperchat(commentData);
                message = superchat;
                nameItems = superchat.NameItems;
                commentItems = superchat.CommentItems;
            }
            else
            {
                var comment = new YouTubeLiveComment(commentData);
                message = comment;
                nameItems = comment.NameItems;
                commentItems = comment.CommentItems;
            }
            var metadata = CreateMetadata(message, isInitialComment);
            var methods = new YouTubeLiveMessageMethods();
            //if (_siteOptions.IsAutoSetNickname)
            //{
            //    //var user = metadata.User;
            //    var messageText = Common.MessagePartsTools.ToText(commentItems);
            //    var nick = SitePluginCommon.Utils.ExtractNickname(messageText);
            //    if (!string.IsNullOrEmpty(nick))
            //    {
            //        //user.Nickname = nick;
            //    }
            //}
            //metadata.User.Name = nameItems;
            return new YouTubeLiveMessageContext2(message, metadata, methods);
        }
        private IYouTubeLiveMessage CreateMessage(CommentData data)
        {
            IYouTubeLiveMessage message;
            if (data.IsPaidMessage)
            {
                message = new YouTubeLiveSuperchat(data);
            }
            else
            {
                message = new YouTubeLiveComment(data);
            }

            return message;
        }
        private static string? ExtractNickname(IEnumerable<IMessagePart> message, IYouTubeLiveSiteOptions siteOptions)
        {
            if (!siteOptions.IsAutoSetNickname) return null;
            foreach (var part in message.Where(m => m is IMessageText).Reverse().Cast<IMessageText>())
            {
                if (part == null) continue;
                return SitePluginCommon.Utils.ExtractNickname(part.Text);
            }
            return null;
        }
        private YouTubeLiveMessageMetadata2 CreateMetadata(IYouTubeLiveMessage message, bool isInitialComment)
        {
            string? userId;
            IEnumerable<IMessagePart>? name;
            string? nickname;
            if (message is IYouTubeLiveComment comment)
            {
                userId = comment.UserId;
                name = comment.NameItems;
                nickname = ExtractNickname(comment.CommentItems, _siteOptions);
            }
            else if (message is IYouTubeLiveSuperchat superchat)
            {
                userId = superchat.UserId;
                name = superchat.NameItems;
                nickname = ExtractNickname(superchat.CommentItems, _siteOptions);
            }
            else
            {
                userId = null;
                name = null;
                nickname = null;
            }
            bool isFirstComment;
            //IUser user;
            if (userId != null)
            {
                if (_userCommentCountDict.ContainsKey(userId))
                {
                    _userCommentCountDict[userId]++;
                    isFirstComment = false;
                }
                else
                {
                    _userCommentCountDict.Add(userId, 1);
                    isFirstComment = true;
                }
                //user = null;
            }
            else
            {
                isFirstComment = false;
                //user = null;
            }
            var metadata = new YouTubeLiveMessageMetadata2(message, _siteOptions, isFirstComment)
            {
                IsInitialComment = isInitialComment,
                SiteContextGuid = SiteContextGuid,
                UserId = userId,
                UserName = name,
                NewNickname = nickname,
            };
            return metadata;
        }
        public EachConnection(ILogger logger, CookieContainer cc, IYouTubeLiveServer server,
            IYouTubeLiveSiteOptions siteOptions, Dictionary<string, int> userCommentCountDict, SynchronizedCollection<string> receivedCommentIds,
            ICommentProvider cp)
        {
            _logger = logger;
            _cc = cc;
            _server = server;
            _siteOptions = siteOptions;
            _userCommentCountDict = userCommentCountDict;
            _receivedCommentIds = receivedCommentIds;
            _cp = cp;
        }
    }
}
