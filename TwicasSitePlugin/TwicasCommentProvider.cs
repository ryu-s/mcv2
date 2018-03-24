﻿using System;
using Common;
using System.Windows.Threading;
using SitePlugin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace TwicasSitePlugin
{
    class TwicasCommentProvider : ICommentProvider
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

        public event EventHandler<List<ICommentViewModel>> InitialCommentsReceived;
        public event EventHandler<ICommentViewModel> CommentReceived;
        public event EventHandler<IMetadata> MetadataUpdated;
        public event EventHandler CanConnectChanged;
        public event EventHandler CanDisconnectChanged;

        private void SendInfo(string message, InfoType type)
        {
            CommentReceived?.Invoke(this, new InfoCommentViewModel(_options, message, type));
        }
        private CookieContainer _cc;
        MessageProvider _messageProvider;
        public async Task ConnectAsync(string input, global::ryu_s.BrowserCookie.IBrowserProfile browserProfile)
        {
            var broadcasterId = Tools.ExtractBroadcasterId(input);
            if (string.IsNullOrEmpty(broadcasterId))
            {
                //Info
                return;
            }
            _cc = new CookieContainer();
            try
            {
                var cookies = browserProfile.GetCookieCollection("twitcasting.tv");
                _cc.Add(cookies);
            }
            catch { }

            CanConnect = false;
            CanDisconnect = true;
            int cnum = -1;
            long liveId = -1;
            try
            {
                var context = await API.GetLiveContext(_server, broadcasterId, _cc);
                cnum = context.MovieCnum;
                liveId = context.MovieId;
                if (!string.IsNullOrEmpty(context.AudienceId))
                {
                    var audienceId = context.AudienceId;
                    SendInfo($"ログイン済みユーザID:{audienceId}", InfoType.Notice);
                }
                else
                {
                    SendInfo("未ログイン", InfoType.Notice);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogException(ex);
                string message;
                if (ex.InnerException != null)
                {
                    message = ex.InnerException.Message;
                }
                else
                {
                    message = ex.Message;
                }
                SendInfo(message, InfoType.Debug);
            }
            catch (InvalidBroadcasterIdException ex)
            {
                _logger.LogException(ex, "", $"input=\"{input}\"");
                SendInfo("入力されたURLまたはIDは存在しないか無効な値です", InfoType.Notice);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _logger.LogException(ex);
                SendInfo(ex.Message, InfoType.Debug);
            }
            if(cnum < 0 || liveId < 0)
            {
                AfterDisconnected();
                return;
            }
            try
            {
                _messageProvider = new MessageProvider(_server, _cc, _logger);
                _messageProvider.InitialCommentsReceived += _messageProvider_InitialCommentsReceived;
                _messageProvider.Received += MessageProvider_Received;
                _messageProvider.MetaReceived += MessageProvider_MetaReceived;
                _messageProvider.InfoOccured += MessageProvider_InfoOccured;

                await _messageProvider.ConnectAsync(broadcasterId, cnum, liveId);
            }
            catch (Exception ex)
            {
                SendInfo(ex.Message, InfoType.Error);
                _logger.LogException(ex);
            }
            finally
            {
                AfterDisconnected();
            }
        }
        private void AfterDisconnected()
        {
            _messageProvider = null;
            CanConnect = true;
            CanDisconnect = false;
        }
        private void MessageProvider_InfoOccured(object sender, InfoData e)
        {
            SendInfo(e.Message, e.Type);
        }

        private void _messageProvider_InitialCommentsReceived(object sender, IEnumerable<ICommentData> e)
        {
            var list = new List<ICommentViewModel>();
            foreach (var data in e)
            {
                try
                {
                    var cvm = CommentData2CommentViewModel(data);
                    list.Add(cvm);
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                    SendInfo(ex.Message, InfoType.Debug);
                }
            }
            InitialCommentsReceived?.Invoke(this, list);
        }
        private TwicasCommentViewModel CommentData2CommentViewModel(ICommentData data)
        {
            var userId = data.UserId;
            var user = _userStore.GetUser(userId);
            var cvm = new TwicasCommentViewModel(_options, data, user);
            return cvm;
        }
        private void MessageProvider_MetaReceived(object sender, IMetadata e)
        {
            MetadataUpdated?.Invoke(this, e);
        }

        private void MessageProvider_Received(object sender, IEnumerable<ICommentData> e)
        {
            foreach (var data in e)
            {
                try
                {
                    var cvm = CommentData2CommentViewModel(data);
                    CommentReceived?.Invoke(this, cvm);
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                    SendInfo(ex.Message, InfoType.Debug);
                }
            }
        }

        public void Disconnect()
        {
            if(_messageProvider != null)
            {
                _messageProvider.Disconnect();
            }
        }

        public IEnumerable<ICommentViewModel> GetUserComments(IUser user)
        {
            throw new NotImplementedException();
        }

        public Task PostCommentAsync(string text)
        {
            throw new NotImplementedException();
        }
        private readonly Dictionary<IUser, ObservableCollection<TwicasCommentViewModel>> _userCommentDict = new Dictionary<IUser, ObservableCollection<TwicasCommentViewModel>>();
        private readonly IDataServer _server;
        private readonly ILogger _logger;
        private readonly ICommentOptions _options;
        private readonly TwicasSiteOptions _siteOptions;
        private readonly IUserStore _userStore;
        public TwicasCommentProvider(IDataServer server, ILogger logger, ICommentOptions options, TwicasSiteOptions siteOptions, IUserStore userStore)
        {
            _server = server;
            _logger = logger;
            _options = options;
            _siteOptions = siteOptions;
            _userStore = userStore;

            CanConnect = true;
            CanDisconnect = false;
        }
    }
}
