﻿using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using SitePluginCommon;
using SitePluginCommon.AutoReconnection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace TwicasSitePlugin
{
    class TwicasCommentProvider2 : CommentProviderBase2
    {
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        private readonly TwicasSiteOptions _siteOptions;
        private readonly FirstCommentDetector _first = new FirstCommentDetector();
        private readonly MessageUntara _messenger = new MessageUntara();
        TwicasAutoReconnector _autoReconnector;
        /// <summary>
        /// 放送ID
        /// 配信外ではnull
        /// </summary>
        private long? _liveId;
        /// <summary>
        /// 最後に取得したコメントのID
        /// コメント投稿に必要
        /// </summary>
        long _lastCommentId;
        public event EventHandler LoggedInStateChanged;
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
        private async Task<List<TwicasMessageContext2>> GetInitialComments(long liveId)
        {
            var list = new List<TwicasMessageContext2>();
            var (initialComments, initialRaw) = await API.GetListAll(_server, _broadcasterId, liveId, _lastCommentId, 0, 20, _cc);
            if (initialComments.Length > 0)
            {
                foreach (var lowComment in initialComments)
                {
                    if (lowComment.Type == "filtered_comment") continue;
                    var internalComment = Tools.Parse(lowComment);
                    var context = CreateMessageContext(internalComment, true);
                    list.Add(context);
                }
                //var initialDataList = LowComment2Data(initialComments, initialRaw);
                //if (initialDataList.Count > 0)
                //{
                //    InitialCommentsReceived?.Invoke(this, initialDataList);
                //}
                var lastComment = initialComments[initialComments.Length - 1];
                _lastCommentId = lastComment.Id;
            }
            return list;
        }
        CookieContainer _cc;
        string _broadcasterId;
        public override async Task ConnectAsync(string input, IBrowserProfile2 browserProfile)
        {
            BeforeConnect();
            _first.Reset();
            _cc = GetCookieContainer(browserProfile, "twitcasting.tv");
            _broadcasterId = Tools.ExtractBroadcasterId(input);
            _lastCommentId = 0;
            var (context, contextRaw) = await API.GetLiveContext(_server, _broadcasterId, _cc);
            if (!string.IsNullOrEmpty(context.AudienceId))
            {
                SendSystemInfo($"ログイン済みユーザID:{context.AudienceId}", InfoType.Notice);
                IsLoggedIn = true;
            }
            else
            {
                SendSystemInfo("未ログイン", InfoType.Notice);
                IsLoggedIn = false;
            }
            //配信歴がある場合は初期コメントを取得する
            if (context.MovieId > 0)
            {
                var initialComments = await GetInitialComments(context.MovieId);
                foreach (var initialComment in initialComments)
                {
                    RaiseMessageReceived(initialComment);
                }
            }

            var p1 = new WebsocketMessageProvider(new Websocket(), _server);
            p1.MessageReceived += P1_MessageReceived;
            var p2 = new MetadataProvider(_logger, _server, _broadcasterId, _messenger);
            p2.ItemReceived += P2_ItemReceived;
            p2.LiveIdReceived += P2_LiveIdReceived;
            p2.MetadataReceived += P2_MetadataReceived;
            _autoReconnector = new TwicasAutoReconnector(_broadcasterId, _cc, _server, _logger, p1, p2);
            try
            {
                await _autoReconnector.AutoReconnectAsync();
            }
            finally
            {
                _autoReconnector = null;
                AfterDisconnected();
            }
        }

        private void P2_MetadataReceived(object sender, Metadata e)
        {
            var metadata = e;
            RaiseMetadataUpdated(metadata);
        }

        private void P2_LiveIdReceived(object sender, long? e)
        {
            var liveId = e;
            _liveId = liveId;
        }

        private void P2_ItemReceived(object sender, InternalItem e)
        {
            var item = e;
            var messageContext = CreateMessageContext(item, false);
            RaiseMessageReceived(messageContext);
        }
        private TwicasMessageContext2 CreateMessageContext(InternalItem item, bool isInitialComment)
        {
            //var user = GetUser(item.UserId);
            var _ = _first.IsFirstComment(item.UserId);//アイテムを最初に投げる場合もありえる。
            var isFirstComment = false;//常にfalseにしておく。現状こうしないとアイテムの文字色背景色が適用されない。そっちのコードを直したとしてもアイテムには不要だろう。
            var commentItems = new List<IMessagePart>();
            if (!string.IsNullOrEmpty(item.Message))
            {
                commentItems.Add(Common.MessagePartFactory.CreateMessageText(item.Message + Environment.NewLine));
            }
            commentItems.Add(new MessageImage
            {
                Url = item.ImageUrl,
                Alt = item.Name,
                Height = 40,
                Width = 40,
            });
            var itemMessage = new TwicasItem(item.Raw)
            {
                CommentItems = commentItems,
                UserIcon = new MessageImage
                {
                    Url = item.UserImageUrl,
                    Alt = "",
                    Height = 40,
                    Width = 40,
                },
                ItemId = item.Id,
                ItemName = item.Name,
                UserId = item.UserId,
                UserName = item.ScreenName,
            };
            var metadata = new MessageMetadata2(itemMessage, _siteOptions, isFirstComment)
            {
                IsInitialComment = isInitialComment,
                SiteContextGuid = SiteContextGuid,
            };
            var methods = new TwicasMessageMethods();
            var context = new TwicasMessageContext2(itemMessage, metadata, methods);
            return context;
        }
        private void P1_MessageReceived(object sender, IInternalMessage e)
        {
            var internalMessage = e;
            switch (internalMessage)
            {
                case InternalComment comment:
                    {
                        _lastCommentId = comment.Id;
                        var context = CreateMessageContext(comment, false);
                        RaiseMessageReceived(context);
                    }
                    break;
            }
        }
        private string? ExtractNickname(string message)
        {
            if (_siteOptions.IsAutoSetNickname)
            {
                return SitePluginCommon.Utils.ExtractNickname(message);
            }
            else
            {
                return null;
            }
        }
        private TwicasMessageContext2 CreateMessageContext(InternalComment comment, bool isInitialComment)
        {
            //var user = GetUser(comment.UserId);
            var isFirstComment = _first.IsFirstComment(comment.UserId);
            var message = new TwicasComment(comment.Raw)
            {
                CommentItems = Tools.ParseMessage(comment.Message),
                Id = comment.Id.ToString(),
                UserName = comment.ScreenName,
                PostTime = comment.CreatedAt.ToString("HH:mm:ss"),
                UserId = comment.UserId,
                UserIcon = new MessageImage
                {
                    Url = comment.ProfileImageUrl,
                    Alt = null,
                    Height = 40,// commentData.ThumbnailHeight,
                    Width = 40,//commentData.ThumbnailWidth,
                },
            };
            var nickname = ExtractNickname(comment.Message);
            var metadata = new MessageMetadata2(message, _siteOptions, isFirstComment)
            {
                IsInitialComment = isInitialComment,
                SiteContextGuid = SiteContextGuid,
                NewNickname = nickname,
            };
            var methods = new TwicasMessageMethods();
            var messageContext = new TwicasMessageContext2(message, metadata, methods);
            return messageContext;
        }
        public override void Disconnect()
        {
            _autoReconnector?.Disconnect();
        }

        public override Task<ICurrentUserInfo> GetCurrentUserInfo(IBrowserProfile2 browserProfile)
        {
            var cc = GetCookieContainer(browserProfile, "twitcasting.tv");
            string name = null;
            foreach (var cookie in Tools.ExtractCookies(cc))
            {
                switch (cookie.Name)
                {
                    case "tc_id":
                        name = cookie.Value;
                        break;
                }
            }
            var info = new CurrentUserInfo
            {
                IsLoggedIn = !string.IsNullOrEmpty(name),
                Username = name,
            };
            return Task.FromResult<ICurrentUserInfo>(info);
        }

        public override async Task PostCommentAsync(string text)
        {
            var (res, raw) = await API.PostCommentAsync(_server, _broadcasterId, _liveId.Value, _lastCommentId, text, _cc);
            if (res.Error != null)
            {
                //error
            }
        }
        public TwicasCommentProvider2(IDataServer server, ILogger logger, TwicasSiteOptions siteOptions)
            : base(logger)
        {
            _server = server;
            _logger = logger;
            _siteOptions = siteOptions;
            _messenger.SystemInfoReiceved += _messenger_SystemInfoReiceved;
        }

        private void _messenger_SystemInfoReiceved(object sender, SitePluginCommon.AutoReconnector.SystemInfoEventArgs e)
        {

        }

        public override void SetMessage(string raw)
        {
            throw new NotImplementedException();
        }
    }
}
