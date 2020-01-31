using Common.Wpf;
using GalaSoft.MvvmLight.CommandWpf;
using SitePlugin;
using SitePluginCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    internal partial class MainViewModel : CommentDataGridViewModelBase
    {
        private readonly IModel _host;
        private readonly IConnectionModel _connHost;
        private readonly ObservableCollection<IMcvCommentViewModel> _comments = new ObservableCollection<IMcvCommentViewModel>();
        public ICommand ShowOptionsWindowCommand { get; }
        public ICommand MainViewClosingCommand { get; }
        public ICommand MainViewClosedCommand { get; }
        public ICommand AddNewConnectionCommand { get; }
        public ICommand ShowUserInfoCommand { get; }
        public ICommand CheckUpdateCommand { get; }
        public ICommand ShowDeveloppersTwitterCommand { get; }
        public ICommand RemoveSelectedConnectionCommand { get; }
        public ICommand ClearAllCommentsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ShowUserListCommand { get; }
        public ICommand ShowWebSiteCommand { get; }

        public ObservableCollection<MetadataViewModel> MetaCollection { get; } = new ObservableCollection<MetadataViewModel>();
        public ObservableCollection<ConnectionViewModel> Connections { get; } = new ObservableCollection<ConnectionViewModel>();
        public ObservableCollection<PluginMenuItemViewModel> PluginMenuItemCollection { get; } = new ObservableCollection<PluginMenuItemViewModel>();
        public MainViewModel()
            : base(new DynamicOptionsTest())
        {
        }
        public MainViewModel(IModel host, IConnectionModel connModel, DynamicOptionsTest options)
            : base(options)
        {
            _host = host;
            _connHost = connModel;
            Comments = CollectionViewSource.GetDefaultView(_comments);
            //_userStore = new UserStore(options);
            AddNewConnectionCommand = new RelayCommand(AddConnection);
            ShowOptionsWindowCommand = new RelayCommand(ShowOptionsWindow);
            MainViewClosingCommand = new RelayCommand<CancelEventArgs>(Closing);
            MainViewClosedCommand = new RelayCommand(Closed);
            ShowUserInfoCommand = new RelayCommand(ShowUserInfo);
            //ShowUserListCommand = new RelayCommand(ShowUserList);
            CheckUpdateCommand = new RelayCommand(CheckUpdate);
            YouTubeCommentPostPanelViewModel = new YouTubeCommentPostPanelViewModel(host);

            ShowDeveloppersTwitterCommand = new RelayCommand(ShowDeveloppersTwitter);
            RemoveSelectedConnectionCommand = new RelayCommand(RemoveSelectedConnection);
            ClearAllCommentsCommand = new RelayCommand(ClearAllComments);
            ExitCommand = new RelayCommand(Exit);
            ShowUserListCommand = new RelayCommand(ShowUserList);
            ShowWebSiteCommand = new RelayCommand(ShowWebSite);

            options.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(options.MainViewLeft):
                        RaisePropertyChanged(nameof(MainViewLeft));
                        break;
                    case nameof(options.MainViewTop):
                        RaisePropertyChanged(nameof(MainViewTop));
                        break;
                    case nameof(options.MainViewHeight):
                        RaisePropertyChanged(nameof(MainViewHeight));
                        break;
                    case nameof(options.MainViewWidth):
                        RaisePropertyChanged(nameof(MainViewWidth));
                        break;
                    case nameof(options.IsShowThumbnail):
                        RaisePropertyChanged(nameof(IsShowThumbnail));
                        break;
                    case nameof(options.IsShowUsername):
                        RaisePropertyChanged(nameof(IsShowUsername));
                        break;
                    case nameof(options.IsShowConnectionName):
                        RaisePropertyChanged(nameof(IsShowConnectionName));
                        break;
                    case nameof(options.IsShowCommentId):
                        RaisePropertyChanged(nameof(IsShowCommentId));
                        break;
                    case nameof(options.IsShowMessage):
                        RaisePropertyChanged(nameof(IsShowMessage));
                        break;
                    case nameof(options.IsShowPostTime):
                        RaisePropertyChanged(nameof(IsShowPostTime));
                        break;
                    case nameof(options.IsShowInfo):
                        RaisePropertyChanged(nameof(IsShowInfo));
                        break;

                    case nameof(options.TitleBackColor):
                        RaisePropertyChanged(nameof(TitleBackground));
                        break;
                    case nameof(options.TitleForeColor):
                        RaisePropertyChanged(nameof(TitleForeground));
                        break;
                    case nameof(options.ViewBackColor):
                        RaisePropertyChanged(nameof(ViewBackground));
                        break;
                    case nameof(options.WindowBorderColor):
                        RaisePropertyChanged(nameof(WindowBorderBrush));
                        break;
                    case nameof(options.SystemButtonBackColor):
                        RaisePropertyChanged(nameof(SystemButtonBackground));
                        break;
                    case nameof(options.SystemButtonForeColor):
                        RaisePropertyChanged(nameof(SystemButtonForeground));
                        break;
                    case nameof(options.SystemButtonBorderColor):
                        RaisePropertyChanged(nameof(SystemButtonBorderBrush));
                        break;
                    case nameof(options.SystemButtonMouseOverBackColor):
                        RaisePropertyChanged(nameof(SystemButtonMouseOverBackground));
                        break;
                    case nameof(options.SystemButtonMouseOverForeColor):
                        RaisePropertyChanged(nameof(SystemButtonMouseOverForeground));
                        break;
                    case nameof(options.SystemButtonMouseOverBorderColor):
                        RaisePropertyChanged(nameof(SystemButtonMouseOverBorderBrush));
                        break;

                    case nameof(options.MenuBackColor):
                        RaisePropertyChanged(nameof(MenuBackground));
                        RaisePropertyChanged(nameof(ContextMenuBackground));
                        break;
                    case nameof(options.MenuForeColor):
                        RaisePropertyChanged(nameof(MenuForeground));
                        RaisePropertyChanged(nameof(ContextMenuForeground));
                        break;
                    case nameof(options.MenuPopupBorderColor):
                        RaisePropertyChanged(nameof(MenuPopupBorderBrush));
                        break;
                    case nameof(options.MenuSeparatorBackColor):
                        RaisePropertyChanged(nameof(MenuSeparatorBackground));
                        break;
                    case nameof(options.MenuItemCheckMarkColor):
                        RaisePropertyChanged(nameof(MenuItemCheckMarkBrush));
                        break;
                    case nameof(options.MenuItemMouseOverBackColor):
                        RaisePropertyChanged(nameof(MenuItemMouseOverBackground));
                        break;
                    case nameof(options.MenuItemMouseOverForeColor):
                        RaisePropertyChanged(nameof(MenuItemMouseOverForeground));
                        break;
                    case nameof(options.MenuItemMouseOverBorderColor):
                        RaisePropertyChanged(nameof(MenuItemMouseOverBorderBrush));
                        break;
                    case nameof(options.MenuItemMouseOverCheckMarkColor):
                        RaisePropertyChanged(nameof(MenuItemMouseOverCheckMarkBrush));
                        break;


                    case nameof(options.ButtonBackColor):
                        RaisePropertyChanged(nameof(ButtonBackground));
                        break;
                    case nameof(options.ButtonForeColor):
                        RaisePropertyChanged(nameof(ButtonForeground));
                        break;
                    case nameof(options.ButtonBorderColor):
                        RaisePropertyChanged(nameof(ButtonBorderBrush));
                        break;
                    case nameof(options.CommentListBackColor):
                        RaisePropertyChanged(nameof(CommentListBackground));
                        RaisePropertyChanged(nameof(ConnectionListBackground));
                        break;
                    case nameof(options.CommentListHeaderBackColor):
                        RaisePropertyChanged(nameof(CommentListHeaderBackground));
                        RaisePropertyChanged(nameof(ConnectionListHeaderBackground));
                        break;
                    case nameof(options.CommentListHeaderForeColor):
                        RaisePropertyChanged(nameof(CommentListHeaderForeground));
                        RaisePropertyChanged(nameof(ConnectionListHeaderForeground));
                        break;
                    case nameof(options.CommentListHeaderBorderColor):
                        RaisePropertyChanged(nameof(CommentListHeaderBorderBrush));
                        RaisePropertyChanged(nameof(ConnectionListHeaderBorderBrush));
                        break;
                    case nameof(options.CommentListBorderColor):
                        RaisePropertyChanged(nameof(CommentListBorderBrush));
                        RaisePropertyChanged(nameof(ConnectionListBorderBrush));
                        break;
                    case nameof(options.CommentListSeparatorColor):
                        RaisePropertyChanged(nameof(CommentListSeparatorBrush));
                        RaisePropertyChanged(nameof(ConnectionListSeparatorBrush));
                        break;
                    //case nameof(options.ConnectionListBackColor):
                    //    RaisePropertyChanged(nameof(ConnectionListBackground));
                    //    break;
                    //case nameof(options.ConnectionListHeaderBackColor):
                    //    RaisePropertyChanged(nameof(ConnectionListHeaderBackground));
                    //    break;
                    //case nameof(options.ConnectionListHeaderForeColor):
                    //    RaisePropertyChanged(nameof(ConnectionListHeaderForeground));
                    //    break;
                    case nameof(options.ConnectionListRowBackColor):
                        RaisePropertyChanged(nameof(ConnectionListRowBackground));
                        break;

                    case nameof(options.ScrollBarBackColor):
                        RaisePropertyChanged(nameof(ScrollBarBackground));
                        break;
                    case nameof(options.ScrollBarBorderColor):
                        RaisePropertyChanged(nameof(ScrollBarBorderBrush));
                        break;
                    case nameof(options.ScrollBarThumbBackColor):
                        RaisePropertyChanged(nameof(ScrollBarThumbBackground));
                        break;
                    case nameof(options.ScrollBarThumbMouseOverBackColor):
                        RaisePropertyChanged(nameof(ScrollBarThumbMouseOverBackground));
                        break;
                    case nameof(options.ScrollBarThumbPressedBackColor):
                        RaisePropertyChanged(nameof(ScrollBarThumbPressedBackground));
                        break;


                    case nameof(options.ScrollBarButtonBackColor):
                        RaisePropertyChanged(nameof(ScrollBarButtonBackground));
                        break;
                    case nameof(options.ScrollBarButtonForeColor):
                        RaisePropertyChanged(nameof(ScrollBarButtonForeground));
                        break;
                    case nameof(options.ScrollBarButtonBorderColor):
                        RaisePropertyChanged(nameof(ScrollBarButtonBorderBrush));
                        break;


                    case nameof(options.ScrollBarButtonDisabledBackColor):
                        RaisePropertyChanged(nameof(ScrollBarButtonDisabledBackground));
                        break;
                    case nameof(options.ScrollBarButtonDisabledForeColor):
                        RaisePropertyChanged(nameof(ScrollBarButtonDisabledForeground));
                        break;
                    case nameof(options.ScrollBarButtonDisabledBorderColor):
                        RaisePropertyChanged(nameof(ScrollBarButtonDisabledBorderBrush));
                        break;

                    case nameof(options.ScrollBarButtonMouseOverBackColor):
                        RaisePropertyChanged(nameof(ScrollBarButtonMouseOverBackground));
                        break;
                    case nameof(options.ScrollBarButtonPressedBackColor):
                        RaisePropertyChanged(nameof(ScrollBarButtonPressedBackground));
                        break;
                    case nameof(options.ScrollBarButtonPressedBorderColor):
                        RaisePropertyChanged(nameof(ScrollBarButtonPressedBorderBrush));
                        break;

                    case nameof(options.IsEnabledSiteConnectionColor):
                    case nameof(options.SiteConnectionColorType):
                        RaisePropertyChanged(nameof(ConnectionColorColumnWidth));
                        //RaisePropertyChanged(nameof(IsShowConnectionsViewConnectionBackground));
                        //RaisePropertyChanged(nameof(IsShowConnectionsViewConnectionForeground));
                        break;
                    case nameof(options.IsTopmost):
                        //_pluginManager.OnTopmostChanged(options.IsTopmost);
                        RaisePropertyChanged(nameof(Topmost));
                        break;

                    case nameof(options.IsShowHorizontalGridLine):
                        break;
                    case nameof(options.HorizontalGridLineColor):
                        RaisePropertyChanged(nameof(HorizontalGridLineBrush));
                        break;
                    case nameof(options.IsShowVerticalGridLine):
                        break;
                    case nameof(options.VerticalGridLineColor):
                        RaisePropertyChanged(nameof(VerticalGridLineBrush));
                        break;

                    case nameof(options.IsShowMetaConnectionName):
                        RaisePropertyChanged(nameof(IsShowMetaConnectionName));
                        break;
                    case nameof(options.IsShowMetaTitle):
                        RaisePropertyChanged(nameof(IsShowMetaTitle));
                        break;
                    case nameof(options.IsShowMetaElapse):
                        RaisePropertyChanged(nameof(IsShowMetaElapse));
                        break;
                    case nameof(options.IsShowMetaCurrentViewers):
                        RaisePropertyChanged(nameof(IsShowMetaCurrentViewers));
                        break;
                    case nameof(options.IsShowMetaTotalViewers):
                        RaisePropertyChanged(nameof(IsShowMetaTotalViewers));
                        break;
                    case nameof(options.IsShowMetaActive):
                        RaisePropertyChanged(nameof(IsShowMetaActive));
                        break;
                    case nameof(options.IsShowMetaOthers):
                        RaisePropertyChanged(nameof(IsShowMetaOthers));
                        break;
                }
            };
            RaisePropertyChanged(nameof(Topmost));
        }



        private void ShowDeveloppersTwitter()
        {
            var url = "https://twitter.com/kv510k";
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                _host.SendError(ex, "", "");
            }
        }
        private void RemoveSelectedConnection()
        {
            _host.RemoveSelectedConnection();
        }
        private void ClearAllComments()
        {

        }
        private void Exit()
        {

        }
        private void ShowUserList()
        {

        }
        private void ShowWebSite()
        {
            try
            {
                System.Diagnostics.Process.Start("https://ryu-s.github.io/app/multicommentviewer");
            }
            catch (Exception ex)
            {
                _host.SendError(ex, "", "");
            }
        }
        public double ConnectionColorColumnWidth
        {
            get
            {
                if (_options.IsEnabledSiteConnectionColor && _options.SiteConnectionColorType == SiteConnectionColorType.Connection)
                {
                    return 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool IsShowMetaConnectionName
        {
            get { return _options.IsShowMetaConnectionName; }
            set { _options.IsShowMetaConnectionName = value; }
        }

        public bool IsShowMetaTitle
        {
            get { return _options.IsShowMetaTitle; }
            set { _options.IsShowMetaTitle = value; }
        }

        public bool IsShowMetaElapse
        {
            get { return _options.IsShowMetaElapse; }
            set { _options.IsShowMetaElapse = value; }
        }

        public bool IsShowMetaCurrentViewers
        {
            get { return _options.IsShowMetaCurrentViewers; }
            set { _options.IsShowMetaCurrentViewers = value; }
        }
        #region Metadata
        public int MetadataViewConnectionNameDisplayIndex
        {
            get { return _options.MetadataViewConnectionNameDisplayIndex; }
            set { _options.MetadataViewConnectionNameDisplayIndex = value; }
        }
        public int MetadataViewTitleDisplayIndex
        {
            get { return _options.MetadataViewTitleDisplayIndex; }
            set { _options.MetadataViewTitleDisplayIndex = value; }
        }
        public int MetadataViewElapsedDisplayIndex
        {
            get { return _options.MetadataViewElapsedDisplayIndex; }
            set { _options.MetadataViewElapsedDisplayIndex = value; }
        }
        public int MetadataViewCurrentViewersDisplayIndex
        {
            get { return _options.MetadataViewCurrentViewersDisplayIndex; }
            set { _options.MetadataViewCurrentViewersDisplayIndex = value; }
        }
        public int MetadataViewTotalViewersDisplayIndex
        {
            get { return _options.MetadataViewTotalViewersDisplayIndex; }
            set { _options.MetadataViewTotalViewersDisplayIndex = value; }
        }
        public int MetadataViewActiveDisplayIndex
        {
            get { return _options.MetadataViewActiveDisplayIndex; }
            set { _options.MetadataViewActiveDisplayIndex = value; }
        }
        public int MetadataViewOthersDisplayIndex
        {
            get { return _options.MetadataViewOthersDisplayIndex; }
            set { _options.MetadataViewOthersDisplayIndex = value; }
        }
        #endregion

        public bool IsShowMetaTotalViewers
        {
            get { return _options.IsShowMetaTotalViewers; }
            set { _options.IsShowMetaTotalViewers = value; }
        }

        public bool IsShowMetaActive
        {
            get { return _options.IsShowMetaActive; }
            set { _options.IsShowMetaActive = value; }
        }

        public bool IsShowMetaOthers
        {
            get { return _options.IsShowMetaOthers; }
            set { _options.IsShowMetaOthers = value; }
        }

        public YouTubeCommentPostPanelViewModel YouTubeCommentPostPanelViewModel { get; }
        public ConnectionViewModel SelectedConnection
        {
            get
            {
                var connectionId = _host.GetCurrentConnection();
                if (connectionId == null)
                {
                    //UnselectedConnectionViewModel的なものを返したい。
                    return null;
                }
                if (!_connDict.TryGetValue(connectionId, out var connVm))
                {
                    Debug.Assert(false);
                    return null;
                }
                return connVm;
            }
            set
            {
                var next = value;
                var currentId = _host.GetCurrentConnection();
                if (currentId == next.Id) return;
                _host.SetCurrentConnection(next.Id);

                IsShowYouTubeCommentPostPanel = false;

                var siteType = _host.GetSiteType(next.SelectedSite.SiteId);
                switch (siteType)
                {
                    case SiteType.YouTubeLive:
                        IsShowYouTubeCommentPostPanel = true;
                        break;
                    default:
                        break;
                }
            }
        }
        public bool IsShowYouTubeCommentPostPanel
        {
            get
            {
                return _isShowYouTubeCommentPostPanel;
            }
            set
            {
                _isShowYouTubeCommentPostPanel = value;
                RaisePropertyChanged();
            }
        }


        private void CheckUpdate()
        {
            MessengerInstance.Send(new ShowUpdateViewMessage(new mcv2MainViewPlugin.Update.UpdateViewModel(_host)));
        }
        private void Closing(CancelEventArgs e)
        {

        }
        private void Closed()
        {
            SendRequest(new RequestAppClose());
        }
        private void AddConnection()
        {
            var req = new RequestAddConnection();
            SendRequest(req);
        }

        internal void AddMessage(ConnectionId connectionId, ISiteMessage message, IMessageMetadata2 metadata, UserViewModel? user)
        {
            var connVm = _connDict[connectionId];

            IMcvCommentViewModel? vm = null;
            if (message is SitePluginCommon.InfoMessage info)
            {
                vm = new McvInfoCommentViewModel(info, connVm, _options, metadata.SiteContextGuid);
            }
            else if (message is YouTubeLiveSitePlugin.IYouTubeLiveComment comment)
            {
                vm = new McvYouTubeLiveCommentViewModel2(comment, connVm, user, _options, _youtubeOptions, metadata.SiteContextGuid);
            }
            else if (message is YouTubeLiveSitePlugin.IYouTubeLiveSuperchat ytSuperChat)
            {
                vm = new McvYouTubeLiveSuperChatViewModel2(ytSuperChat, connVm, user, _options, _youtubeOptions, metadata.SiteContextGuid);
            }
            else if (message is TwitchSitePlugin.ITwitchComment twComment)
            {
                vm = new McvTwitchCommentViewModel(twComment, connVm, user, _options, _twitchOptions, metadata.SiteContextGuid);
            }
            else if (message is WhowatchSitePlugin.IWhowatchComment wwComment)
            {
                vm = new McvWhowatchCommentViewModel(wwComment, connVm, user, _options, _whowatchOptions, metadata.SiteContextGuid);
            }
            else if (message is TwicasSitePlugin.ITwicasComment casComment)
            {
                vm = new McvTwicasCommentViewModel(casComment, connVm, user, _options, _twicasOptions, metadata.SiteContextGuid);
            }
            if (vm != null)
            {
                _comments.Add(vm);
            }
        }
        /// <summary>
        /// メニューにプラグインを登録する
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="pluginName"></param>
        internal void AddPlugin(PluginId pluginId, string pluginName)
        {
            Debug.WriteLine($"MainViewPlugin::MainViewModel::AddPlugin() {pluginName}");

            if (PluginMenuItemCollection.Select(x => x.PluginId).Contains(pluginId))
            {
                Debug.WriteLine($"MainViewPlugin::MainViewModel::AddPlugin() already added: ID={pluginId} Name={pluginName}");
                return;
            }
            var vm = new PluginMenuItemViewModel(_host, pluginId, pluginName);
            PluginMenuItemCollection.Add(vm);
        }
        private void SendRequest(IRequest req)
        {
            _host.SendRequest(req);
        }
        private SiteViewModel? GetSiteViewModel(SitePluginId? siteGuid)
        {
            SiteViewModel? site;
            if (siteGuid != null)
            {
                site = _siteDict[siteGuid];
            }
            else
            {
                //selectedSiteがnullの時は_sitesもnullであるはず。
                site = null;
            }
            return site;
        }
        private BrowserViewModel GetBrowserViewModel(Guid? browserGuid)
        {
            BrowserViewModel browser;
            if (browserGuid.HasValue)
            {
                browser = _browserDict[browserGuid.Value];
            }
            else
            {
                //selectedSiteがnullの時は_sitesもnullであるはず。
                browser = null;
            }
            return browser;
        }
        internal void AddNewConnection(ConnectionData conn)
        {
            var selectedSite = GetSiteViewModel(conn.SelectedSite);
            var selectedBrowser = GetBrowserViewModel(conn.SelectedBrowser);
            var adaptor = new Adaptor(_host, _siteDict, _browserDict);
            var connVm = new ConnectionViewModel(conn.Id, _host, _connHost, _sites, _browsers, adaptor);
            //_connDict.Add()する前にconnVmのプロパティを弄ってはいけない。_connDictに登録されておらず、例外が投げられてしまう。
            Connections.Add(connVm);
            _connDict.Add(conn.Id, connVm);
            connVm.UpdateInput(conn.Input);
            connVm.UpdateName(conn.Name);
            connVm.UpdateSelectedSite(selectedSite);
            connVm.UpdateSelectedBrowser(selectedBrowser);
            connVm.UpdateLoggedInUsername(conn.LoggedInUserName);

            var metaVm = new MetadataViewModel()
            {
                ConnectionName = conn.Name,
            };
            MetaCollection.Add(metaVm);
            _metaDict.Add(conn.Id, metaVm);
        }

        internal void RemoveConnection(ConnectionId connId)
        {
            var conn = _connDict[connId];
            Connections.Remove(conn);
            _connDict.Remove(connId);

            var meta = _metaDict[connId];
            MetaCollection.Remove(meta);
            _metaDict.Remove(connId);
        }

        internal void UpdateConnectionStatus(IConnectionStatusDiff newStatus)
        {
            var connection = _connDict[newStatus.ConnectionId];
            if (newStatus.Name != null)
            {
                connection.UpdateName(newStatus.Name);
            }
            if (newStatus.Input != null)
            {
                connection.UpdateInput(newStatus.Input);
            }
            if (newStatus.Site != null)
            {
                var newSite = _siteDict[newStatus.Site];
                connection.UpdateSelectedSite(newSite);
            }
            if (newStatus.Browser.HasValue)
            {
                var newBrowser = _browserDict[newStatus.Browser.Value];
                connection.UpdateSelectedBrowser(newBrowser);
            }
            if (newStatus.IsConnected.HasValue)
            {
                connection.CanConnect = !newStatus.IsConnected.Value;
                connection.CanDisconnect = newStatus.IsConnected.Value;
            }
            if (newStatus.LoggedInUserName != null)
            {
                connection.UpdateLoggedInUsername(newStatus.LoggedInUserName);
            }
        }

        //internal void SetConnectionStatus(Guid connectionId, string name, string input, Guid? site, Guid? browser)
        //{
        //    var connection = _connDict[connectionId];
        //    connection.Name = name;
        //    connection.Input = input;
        //    if (site.HasValue)
        //    {
        //        var siteVm = _siteDict[site.Value];
        //        connection.SelectedSite = siteVm;
        //    }
        //    if (browser.HasValue)
        //    {
        //        var browserVm = _browserDict[browser.Value];
        //        connection.SelectedBrowser = browserVm;
        //    }


        //}
        YouTubeLiveSiteOptions _youtubeOptions = new YouTubeLiveSiteOptions();
        TwitchSiteOptions _twitchOptions = new TwitchSiteOptions();
        WhowatchSiteOptions _whowatchOptions = new WhowatchSiteOptions();
        TwicasSiteOptions _twicasOptions = new TwicasSiteOptions();
        LineLiveSiteOptions _lineLiveSiteOptions = new LineLiveSiteOptions();
        BigoSiteOptions _bigoLiveSiteOptions = new BigoSiteOptions();
        MildomSiteOptions _mildomSiteOptions = new MildomSiteOptions();
        MirrativSiteOptions _mirrativSiteOptions = new MirrativSiteOptions();
        NicoSiteOptions _nicoSiteOptions = new NicoSiteOptions();
        OpenrecSiteOptions _openrecSiteOptions = new OpenrecSiteOptions();
        ShowRoomSiteOptions _showroomSiteOptions = new ShowRoomSiteOptions();
        PeriscopeSiteOptions _periscopeSiteOptions = new PeriscopeSiteOptions();
        private void ShowOptionsWindow()
        {
            try
            {
                var mainOptionsCopy = new MainOptionsCopy();
                mainOptionsCopy.Set(_options);

                var youTubeOptinonsCopy = new YouTubeLiveSiteOptionsCopy();
                youTubeOptinonsCopy.Set(_youtubeOptions);

                var twitchOptionsCopy = new TwitchSiteOptionsCopy();
                twitchOptionsCopy.Set(_twitchOptions);

                var whowatchOptionsCopy = new WhowatchSiteOptionsCopy();
                whowatchOptionsCopy.Set(_whowatchOptions);

                var twicasOptionsCopy = new TwicasSiteOptionsCopy();
                twicasOptionsCopy.Set(_twicasOptions);

                var lineliveOptionsCopy = new LineLiveSiteOptionsCopy();
                lineliveOptionsCopy.Set(_lineLiveSiteOptions);

                var bigoliveOptionsCopy = new BigoLiveSiteOptionsCopy();
                bigoliveOptionsCopy.Set(_bigoLiveSiteOptions);

                var mildomOptionsCopy = new MildomSiteOptionsCopy();
                mildomOptionsCopy.Set(_mildomSiteOptions);

                var mirrativOptionsCopy = new MirrativSiteOptionsCopy();
                mirrativOptionsCopy.Set(_mirrativSiteOptions);

                var nicoOptionsCopy = new NicoSiteOptionsCopy();
                nicoOptionsCopy.Set(_nicoSiteOptions);

                var openrecOptionsCopy = new OpenrecSiteOptionsCopy();
                openrecOptionsCopy.Set(_openrecSiteOptions);

                var showRoomOptionsCopy = new ShowRoomSiteOptionsCopy();
                showRoomOptionsCopy.Set(_showroomSiteOptions);

                var periscopeOptionsCopy = new PeriscopeSiteOptionsCopy();
                periscopeOptionsCopy.Set(_periscopeSiteOptions);

                var vm = new OptionsViewModel(mainOptionsCopy, youTubeOptinonsCopy, twitchOptionsCopy, whowatchOptionsCopy, twicasOptionsCopy, lineliveOptionsCopy, bigoliveOptionsCopy, mildomOptionsCopy, mirrativOptionsCopy, nicoOptionsCopy, openrecOptionsCopy, showRoomOptionsCopy, periscopeOptionsCopy);
                MessengerInstance.Send(new ShowOptionsViewMessage(vm));
                if (vm.Result == OptionsViewResult.Ok)
                {
                    _options.Set(mainOptionsCopy);
                    _youtubeOptions.Set(youTubeOptinonsCopy);
                    _twitchOptions.Set(twitchOptionsCopy);
                    _whowatchOptions.Set(whowatchOptionsCopy);
                    _twicasOptions.Set(twicasOptionsCopy);
                    _lineLiveSiteOptions.Set(lineliveOptionsCopy);
                    _bigoLiveSiteOptions.Set(bigoliveOptionsCopy);
                    _mildomSiteOptions.Set(mildomOptionsCopy);
                    _mirrativSiteOptions.Set(mirrativOptionsCopy);
                    _nicoSiteOptions.Set(nicoOptionsCopy);
                    _openrecSiteOptions.Set(openrecOptionsCopy);
                    _showroomSiteOptions.Set(showRoomOptionsCopy);
                    _periscopeSiteOptions.Set(periscopeOptionsCopy);
                }
            }
            catch (Exception ex)
            {
                _host.SendError(ex, "", "");
            }
        }
        private void ShowUserInfo()
        {
            var current = SelectedComment;
            try
            {
                Debug.Assert(current != null);
                Debug.Assert(current is IMcvCommentViewModel);

                var userId = current.UserId;
                if (string.IsNullOrEmpty(userId))
                {
                    Debug.WriteLine("UserIdがnull");
                    return;
                }
                ShowUserInfo(current.SiteContextGuid, userId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public void ShowUserInfo(SitePluginId siteContextGuid, string userId)
        {
            var uvm = _host.GetUser(siteContextGuid, userId);
            if (uvm == null)
            {
                Debug.WriteLine($"userId={userId}が存在しない");
                return;
            }
            var view = new CollectionViewSource { Source = _comments }.View;
            view.Filter = obj =>
            {
                if (!(obj is IMcvCommentViewModel cvm))
                {
                    return false;
                }
                return cvm.UserId == userId;
            };
            uvm.Comments = view;
            MessengerInstance.Send(new ShowUserViewMessage(uvm));
        }
        Dictionary<ConnectionId, ConnectionViewModel> _connDict = new Dictionary<ConnectionId, ConnectionViewModel>();
        Dictionary<ConnectionId, MetadataViewModel> _metaDict = new Dictionary<ConnectionId, MetadataViewModel>();
        Dictionary<SitePluginId, SiteViewModel> _siteDict = new Dictionary<SitePluginId, SiteViewModel>();
        ObservableCollection<SiteViewModel> _sites = new ObservableCollection<SiteViewModel>();
        Dictionary<Guid, BrowserViewModel> _browserDict = new Dictionary<Guid, BrowserViewModel>();
        ObservableCollection<BrowserViewModel> _browsers = new ObservableCollection<BrowserViewModel>();

        private bool _isShowYouTubeCommentPostPanel;

        internal void AddSite(SitePluginId siteId, string siteName)
        {
            var site = new SiteViewModel(siteName, siteId);
            _sites.Add(site);
            _siteDict.Add(siteId, site);
        }
        internal void AddBrowser(Guid browserId, string browserName, string profileName)
        {
            var browser = new BrowserViewModel(browserName, profileName, browserId);
            _browsers.Add(browser);
            _browserDict.Add(browserId, browser);
        }
        //internal void AddUser(Guid siteContextGuid, string userId)
        //{
        //    _userViewModelStore.Set(siteContextGuid, userId, new UserViewModel(userId, _options));
        //}
        internal void UpdateMetadata(ConnectionId connectionId, string? name, IMetadata metadata)
        {
            var vm = _metaDict[connectionId];
            //2020/08/05 Emptyは許容すべきでは？nullは未設定、その他は値として処理したい。
            if (name != null)
            {
                vm.ConnectionName = name;
            }
            if (!string.IsNullOrEmpty(metadata.Active))
                vm.Active = metadata.Active;
            if (!string.IsNullOrEmpty(metadata.Elapsed))
                vm.Elapsed = metadata.Elapsed;
            if (!string.IsNullOrEmpty(metadata.Others))
                vm.Others = metadata.Others;
            if (!string.IsNullOrEmpty(metadata.Title))
                vm.Title = metadata.Title;
            if (!string.IsNullOrEmpty(metadata.TotalViewers))
                vm.TotalViewers = metadata.TotalViewers;
            if (!string.IsNullOrEmpty(metadata.CurrentViewers))
                vm.CurrentViewers = metadata.CurrentViewers;
        }
        public string Title
        {
            get
            {
                var version = _host.GetCoreVersion();
                var buildType = _host.GetCoreBuildType();

#if DEBUG
                //mcvのPost-buildイベントでmcv本体にコピーされるようにしているが、
                //なぜかうまく渡されていない場合があるから更新時刻を確認しやすいようにした
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                if (codeBase == null) return "";
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                var fileInfo = new FileInfo(path);
                var diff = DateTime.Now - fileInfo.LastWriteTime;
                var createdMinutesAgo = $"created: {(int)diff.TotalMinutes} min ago";
                return $"{version} ({buildType}) {createdMinutesAgo}";
#else
                return $"{version} ({buildType})";
#endif
            }
        }
        public bool Topmost
        {
            get { return _options.IsTopmost; }
            set { _options.IsTopmost = value; }
        }
        public double MainViewHeight
        {
            get { return _options.MainViewHeight; }
            set { _options.MainViewHeight = value; }
        }
        public double MainViewWidth
        {
            get { return _options.MainViewWidth; }
            set { _options.MainViewWidth = value; }
        }
        public double MainViewLeft
        {
            get { return _options.MainViewLeft; }
            set { _options.MainViewLeft = value; }
        }
        public double MainViewTop
        {
            get { return _options.MainViewTop; }
            set { _options.MainViewTop = value; }
        }
        public double ConnectionViewHeight
        {
            get { return _options.ConnectionViewHeight; }
            set { _options.ConnectionViewHeight = value; }
        }
        public double MetadataViewHeight
        {
            get { return _options.MetadataViewHeight; }
            set { _options.MetadataViewHeight = value; }
        }
        public double MetadataViewConnectionNameColumnWidth
        {
            get { return _options.MetadataViewConnectionNameColumnWidth; }
            set { _options.MetadataViewConnectionNameColumnWidth = value; }
        }
        public double MetadataViewTitleColumnWidth
        {
            get { return _options.MetadataViewTitleColumnWidth; }
            set { _options.MetadataViewTitleColumnWidth = value; }
        }
        public double MetadataViewElapsedColumnWidth
        {
            get { return _options.MetadataViewElapsedColumnWidth; }
            set { _options.MetadataViewElapsedColumnWidth = value; }
        }
        public double MetadataViewCurrentViewersColumnWidth
        {
            get { return _options.MetadataViewCurrentViewersColumnWidth; }
            set { _options.MetadataViewCurrentViewersColumnWidth = value; }
        }
        public double MetadataViewTotalViewersColumnWidth
        {
            get { return _options.MetadataViewTotalViewersColumnWidth; }
            set { _options.MetadataViewTotalViewersColumnWidth = value; }
        }
        public double MetadataViewActiveColumnWidth
        {
            get { return _options.MetadataViewActiveColumnWidth; }
            set { _options.MetadataViewActiveColumnWidth = value; }
        }
        public double MetadataViewOthersColumnWidth
        {
            get { return _options.MetadataViewOthersColumnWidth; }
            set { _options.MetadataViewOthersColumnWidth = value; }
        }
        public Brush TitleForeground => new SolidColorBrush(_options.TitleForeColor);
        public Brush TitleBackground => new SolidColorBrush(_options.TitleBackColor);
        public Brush ViewBackground => new SolidColorBrush(_options.ViewBackColor);
        public Brush WindowBorderBrush => new SolidColorBrush(_options.WindowBorderColor);
        public Brush SystemButtonForeground => new SolidColorBrush(_options.SystemButtonForeColor);
        public Brush SystemButtonBackground => new SolidColorBrush(_options.SystemButtonBackColor);
        public Brush SystemButtonBorderBrush => new SolidColorBrush(_options.SystemButtonBorderColor);
        public Brush SystemButtonMouseOverBackground => new SolidColorBrush(_options.SystemButtonMouseOverBackColor);
        public Brush SystemButtonMouseOverForeground => new SolidColorBrush(_options.SystemButtonMouseOverForeColor);
        public Brush SystemButtonMouseOverBorderBrush => new SolidColorBrush(_options.SystemButtonMouseOverBorderColor);
        public Brush MenuBackground => new SolidColorBrush(_options.MenuBackColor);
        public Brush MenuForeground => new SolidColorBrush(_options.MenuForeColor);
        public Brush MenuItemCheckMarkBrush => new SolidColorBrush(_options.MenuItemCheckMarkColor);
        public Brush MenuItemMouseOverBackground => new SolidColorBrush(_options.MenuItemMouseOverBackColor);
        public Brush MenuItemMouseOverForeground => new SolidColorBrush(_options.MenuItemMouseOverForeColor);
        public Brush MenuItemMouseOverBorderBrush => new SolidColorBrush(_options.MenuItemMouseOverBorderColor);
        public Brush MenuItemMouseOverCheckMarkBrush => new SolidColorBrush(_options.MenuItemMouseOverCheckMarkColor);
        public Brush MenuSeparatorBackground => new SolidColorBrush(_options.MenuSeparatorBackColor);
        public Brush MenuPopupBorderBrush => new SolidColorBrush(_options.MenuPopupBorderColor);
        public Brush ButtonBackground => new SolidColorBrush(_options.ButtonBackColor);
        public Brush ButtonForeground => new SolidColorBrush(_options.ButtonForeColor);
        public Brush ButtonBorderBrush => new SolidColorBrush(_options.ButtonBorderColor);
        public Brush CommentListBackground => new SolidColorBrush(_options.CommentListBackColor);
        public Brush CommentListBorderBrush => new SolidColorBrush(_options.CommentListBorderColor);
        public Brush CommentListHeaderBackground => new SolidColorBrush(_options.CommentListHeaderBackColor);
        public Brush CommentListHeaderForeground => new SolidColorBrush(_options.CommentListHeaderForeColor);
        public Brush CommentListHeaderBorderBrush => new SolidColorBrush(_options.CommentListHeaderBorderColor);
        public Brush CommentListSeparatorBrush => new SolidColorBrush(_options.CommentListSeparatorColor);
        public Brush ConnectionListBackground => new SolidColorBrush(_options.CommentListBackColor);
        public Brush ConnectionListHeaderBackground => new SolidColorBrush(_options.CommentListHeaderBackColor);
        public Brush ConnectionListHeaderForeground => new SolidColorBrush(_options.CommentListHeaderForeColor);
        public Brush ConnectionListHeaderBorderBrush => new SolidColorBrush(_options.CommentListHeaderBorderColor);
        public Brush ConnectionListBorderBrush => new SolidColorBrush(_options.CommentListBorderColor);
        public Brush ConnectionListSeparatorBrush => new SolidColorBrush(Colors.Yellow);
        public Brush ConnectionListRowBackground => new SolidColorBrush(_options.ConnectionListRowBackColor);
        public Brush ContextMenuBackground => new SolidColorBrush(_options.MenuBackColor);
        public Brush ContextMenuForeground => new SolidColorBrush(_options.MenuForeColor);
        public Brush ContextMenuBorderBrush => new SolidColorBrush(_options.MenuPopupBorderColor);
        public Brush ScrollBarBorderBrush => new SolidColorBrush(_options.ScrollBarBorderColor);
        public Brush ScrollBarThumbBackground => new SolidColorBrush(_options.ScrollBarThumbBackColor);
        public Brush ScrollBarThumbMouseOverBackground => new SolidColorBrush(_options.ScrollBarThumbMouseOverBackColor);
        public Brush ScrollBarThumbPressedBackground => new SolidColorBrush(_options.ScrollBarThumbPressedBackColor);
        public Brush ScrollBarBackground => new SolidColorBrush(_options.ScrollBarBackColor);
        public Brush ScrollBarButtonBackground => new SolidColorBrush(_options.ScrollBarButtonBackColor);
        public Brush ScrollBarButtonForeground => new SolidColorBrush(_options.ScrollBarButtonForeColor);
        public Brush ScrollBarButtonBorderBrush => new SolidColorBrush(_options.ScrollBarButtonBorderColor);
        public Brush ScrollBarButtonDisabledBackground => new SolidColorBrush(_options.ScrollBarButtonDisabledBackColor);
        public Brush ScrollBarButtonDisabledForeground => new SolidColorBrush(_options.ScrollBarButtonDisabledForeColor);
        public Brush ScrollBarButtonDisabledBorderBrush => new SolidColorBrush(_options.ScrollBarButtonDisabledBorderColor);
        public Brush ScrollBarButtonMouseOverBackground => new SolidColorBrush(_options.ScrollBarButtonMouseOverBackColor);
        public Brush ScrollBarButtonMouseOverForeground => new SolidColorBrush(_options.ScrollBarButtonMouseOverForeColor);
        public Brush ScrollBarButtonMouseOverBorderBrush => new SolidColorBrush(_options.ScrollBarButtonMouseOverBorderColor);
        public Brush ScrollBarButtonPressedBackground => new SolidColorBrush(_options.ScrollBarButtonPressedBackColor);
        public Brush ScrollBarButtonPressedForeground => new SolidColorBrush(_options.ScrollBarButtonPressedForeColor);
        public Brush ScrollBarButtonPressedBorderBrush => new SolidColorBrush(_options.ScrollBarButtonPressedBorderColor);
    }
}
