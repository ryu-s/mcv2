using Common;
using SitePlugin;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SitePluginCommon;
using System.Collections.Generic;
using ryu_s.BrowserCookie;

namespace NicoSitePlugin
{
    abstract class CommentProviderInternalBase2 : INicoCommentProviderInternal2
    {
        protected readonly INicoSiteOptions _siteOptions;
        protected readonly IDataSource _dataSource;
        protected readonly ILogger _logger;
        protected readonly ConcurrentDictionary<string, int> _userCommentCountDict = new ConcurrentDictionary<string, int>();
        /// <summary>
        /// 現在の放送の部屋のThreadIdで一番小さいもの。
        /// 基本的にはアリーナがこれに該当するが、自分の部屋しか取れない場合もあるためそれを考慮してこういう形にした。
        /// </summary>
        protected string _mainRoomThreadId;
        protected void SendSystemInfo(string message, InfoType type)
        {
            var context = InfoMessageContext2.Create(new InfoMessage
            {
                Text = message,
                SiteType = SiteType.NicoLive,
                Type = type,
            });
            MessageReceived?.Invoke(this, context);
        }
        public async Task<NicoMessageContext2> CreateMessageContextAsync(IChat chat, string roomName, bool isInitialComment)
        {
            NicoMessageContext2 messageContext = null;
            INicoMessageMetadata2 metadata;

            var userId = chat.UserId;


            INicoMessage message;
            var messageType = Tools.GetMessageType(chat, _mainRoomThreadId);
            switch (messageType)
            {
                case NicoMessageType.Comment:
                    {
                        var comment = await Tools.CreateNicoComment2(chat, _siteOptions, roomName, async userid => await API.GetUserInfo(_dataSource, userid), _logger);

                        bool isFirstComment;
                        if (_userCommentCountDict.ContainsKey(userId))
                        {
                            _userCommentCountDict[userId]++;
                            isFirstComment = false;
                        }
                        else
                        {
                            _userCommentCountDict.AddOrUpdate(userId, 1, (s, n) => n);
                            isFirstComment = true;
                        }
                        message = comment;
                        metadata = new CommentMessageMetadata2(comment, _siteOptions, isFirstComment)
                        {
                            IsInitialComment = isInitialComment,
                            SiteContextGuid = SiteContextGuid,
                        };
                    }
                    break;
                case NicoMessageType.Info:
                    {
                        var info = Tools.CreateNicoInfo(chat, roomName, _siteOptions);
                        message = info;
                        metadata = new InfoMessageMetadata2(info, _siteOptions);
                    }
                    break;
                case NicoMessageType.Ad:
                    {
                        var ad = Tools.CreateNicoAd(chat, roomName, _siteOptions);
                        message = ad;
                        metadata = new AdMessageMetadata2(ad, _siteOptions);
                    }
                    break;
                case NicoMessageType.Item:
                    {
                        var item = Tools.CreateNicoItem(chat, roomName, _siteOptions);
                        message = item;
                        metadata = new ItemMessageMetadata2(item, _siteOptions);
                    }
                    break;
                default:
                    message = null;
                    metadata = null;
                    break;
            }
            if (message == null || metadata == null)
            {
                return null;
            }
            else
            {
                var methods = new NicoMessageMethods();
                messageContext = new NicoMessageContext2(message, metadata, methods);
                return messageContext;
            }
        }

        public abstract void BeforeConnect();

        public abstract void AfterDisconnected();

        public abstract Task ConnectAsync(string input, CookieContainer cc);

        public abstract void Disconnect();

        public abstract bool IsValidInput(string input);
        protected void RaiseMessageReceived(NicoMessageContext2 messageContext)
        {
            if (messageContext != null)
            {
                MessageReceived?.Invoke(this, messageContext);
            }
        }
        protected void RaiseConnected(ConnectedEventArgs args)
        {
            Connected?.Invoke(this, args);
        }
        protected void RaiseMetadataUpdated(IMetadata metadata)
        {
            MetadataUpdated?.Invoke(this, metadata);
        }

        public abstract Task PostCommentAsync(string comment, string mail);
        public abstract void SetMessage(string raw);

        public CommentProviderInternalBase2(INicoSiteOptions siteOptions, IDataSource dataSource, ILogger logger)
        {
            _siteOptions = siteOptions;
            _dataSource = dataSource;
            _logger = logger;
        }
        public SitePluginId SiteContextGuid { get; set; }

        public event EventHandler<IMessageContext2> MessageReceived;
        public event EventHandler<IMetadata> MetadataUpdated;
        public event EventHandler<ConnectedEventArgs> Connected;
    }
}
