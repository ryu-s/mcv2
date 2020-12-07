using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace mcv2.Model
{
    class UserDiff : IMcvUserDiff
    {
        public SitePluginId SiteId { get; }
        public string UserId { get; }
        public IEnumerable<IMessagePart>? Name { get; set; }
        public string? Nickname { get; set; }
        public bool? IsNgUser { get; set; }
        public bool? IsSiteNgUser { get; set; }

        public UserDiff(SitePluginId sitePluginId, string userId)
        {
            SiteId = sitePluginId;
            UserId = userId;
        }
    }
    class EmptyBrowserProfile : IBrowserProfile2
    {
        public Guid Id { get; } = new Guid("0F63097E-0226-447E-B009-AF1DBB40C98C");
        public string Path { get; } = "";
        public string ProfileName { get; } = "";
        public BrowserType Type { get; }

        public Cookie GetCookie(string domain, string name)
        {
            return new Cookie();
        }

        public List<Cookie> GetCookieCollection(string domain)
        {
            return new List<Cookie>();
        }
    }
    public interface IBrowserLoader
    {
        IEnumerable<IBrowserProfile2> LoadBrowsers();
    }
    public class BrowserLoader : IBrowserLoader
    {
        private readonly ILogger _logger;

        public BrowserLoader(ILogger logger)
        {
            _logger = logger;
        }
        public IEnumerable<IBrowserProfile2> LoadBrowsers()
        {
            var list = new List<IBrowserProfile2>();
            var managers = new List<IBrowserManager>
            {
                new ChromeManager(),
                new ChromeBetaManager(),
                new FirefoxManager(),
                new EdgeManager(),
                new OperaManager(),
                new OperaGxManager(),
            };
            foreach (var manager in managers)
            {
                try
                {
                    list.AddRange(manager.GetProfiles());
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
            }
            return list;
        }
    }
    class Metadata : IMetadata
    {
        public string? Title { get; set; }
        public string? Elapsed { get; set; }
        public string? CurrentViewers { get; set; }
        public string? Active { get; set; }
        public string? TotalViewers { get; set; }
        public bool? IsLive { get; set; }
        public string? Others { get; set; }
    }
    class Model : IPluginHost
    {
        public string LoadOptions(PluginId pluginId, string filename)
        {
            var settingsDirPath = GetSettingsDirPath(pluginId);
            var filePath = System.IO.Path.Combine(settingsDirPath, filename);
            var s = _io.ReadFile(filePath);
            return s;
        }
        public void SaveOptions(PluginId pluginId, string filename, string optionsStr)
        {
            var settingsDirPath = GetSettingsDirPath(pluginId);
            var filePath = System.IO.Path.Combine(settingsDirPath, filename);
            _io.WriteFile(filePath, optionsStr);
        }
        private string GetSettingsDirPath(PluginId pluginId)
        {
            var pluginInfo = _pluginManager.GetPluginInfo(pluginId);
            var dirPath = System.IO.Path.Combine(SettingsDirName, pluginInfo.Name);
            return dirPath;
        }

        internal void SetNotify(INotify notify)
        {
            _pluginManager.SetNotify(notify);
        }

        [Obsolete("LoadOptions(PluginId pluginId, string filename) を使用すること")]
        public string LoadOptions(string path)
        {
            var s = _io.ReadFile(path);
            return s;
        }
        [Obsolete]
        public void SaveOptions(string path, string optionsStr)
        {
            _io.WriteFile(path, optionsStr);
        }
        private readonly PluginManager _pluginManager;
        private readonly ISitePluginManager _sitePluginManager;
        private readonly IUserStore _userStore;
        private readonly Dictionary<ConnectionId, Metadata> _metaDict = new Dictionary<ConnectionId, Metadata>();
        readonly Dictionary<Guid, IBrowserProfile2> _browserDict = new Dictionary<Guid, IBrowserProfile2>();
        private readonly ILogger _logger;
        private readonly IIo _io;
        private readonly ICoreOptions _coreOptions;
        readonly Dictionary<ConnectionId, Connection2> _connectionDict = new Dictionary<ConnectionId, Connection2>();
        readonly CancellationTokenSource _cts = new CancellationTokenSource();
        const string SettingsDirName = "settings";
        private readonly SitePluginId EmptySiteId = new SitePluginId(Guid.NewGuid());
        private readonly Guid EmptyBrowserId;
        public void SetRequest(PluginId pluginId, IRequest req)
        {
            Debug.WriteLine($"Model::SetRequest req={req}");
            switch (req)
            {
                case RequestAddConnection addConn:
                    {
                        //サイト一覧とブラウザ一覧はConnectionの管轄外とする。
                        //Connectionは状態保持クラス。
                        var conn = new Connection2(new ConnectionHost(this, _sitePluginManager), EmptySiteId, EmptyBrowserId);
                        _connectionDict.Add(conn.ConnectionId, conn);
                        _metaDict.Add(conn.ConnectionId, new Metadata());

                        //SelectedSiteの初期値を設定
                        if (_sitePluginManager.Sites().Count > 0)
                        {
                            var (siteId, displayName) = _sitePluginManager.Sites()[0];
                            conn.Set(new ConnectionStatusDiff(conn.ConnectionId)
                            {
                                Site = siteId,
                            });
                        }
                        //SelectedBrowserの初期値を設定
                        if (_browserDict.Count > 0)
                        {
                            conn.Set(new ConnectionStatusDiff(conn.ConnectionId)
                            {
                                Browser = _browserDict.Keys.First(),
                            });
                        }
                        _pluginManager.SetResponse(pluginId, new ResponseAddConnection(addConn.Id, conn.ConnectionId, conn.Input, conn.Name, conn.Site, conn.Browser, conn.IsConnected));
                        _pluginManager.SetNotify(new NotifyAddConnection(conn.ConnectionId, conn.Input, conn.Name, conn.Site, conn.Browser, conn.IsConnected, conn.LoggedInUserName));
                    }
                    break;
                case RequestRemoveConnection removeConn:
                    var connId = removeConn.ConnectionId;
                    if (CanRemoveConnection(connId))
                    {
                        _connectionDict.Remove(connId);
                        _pluginManager.SetNotify(new NotifyRemoveConnection(connId));
                    }
                    break;
                case RequestChangeConnectionStatus changeConnStatus:
                    {
                        var current = GetConnection(changeConnStatus.ConnectionId);
                        if (current == null)
                        {
                            Debug.WriteLine("Model::SetRequest() RequestChangeConnectionStatus the connectionId is not exists");
                            return;
                        }
                        var diff = current.Set(changeConnStatus);

                        //まずリクエストを送ってきたプラグインに対して結果を返す
                        _pluginManager.SetResponse(pluginId, new ResponseConnectionStatusChanged(changeConnStatus.Id, diff));
                        if (diff.HasChanged())
                        {
                            //次に他のプラグインに対しても変更があったことを通知する
                            _pluginManager.SetNotify(new NotifyConnectionStatusChanged(diff));
                            if (diff.Name != null)
                            {
                                _pluginManager.SetNotify(new NotifyMetadataUpdated(diff.ConnectionId, diff.Name, new Metadata()));
                            }
                        }
                    }
                    break;
                case RequestAddComment reqAddComment:
                    OnMessageReceived(reqAddComment.ConnectionId, reqAddComment.Message, reqAddComment.Metadata);
                    break;
                case RequestUpdateMetadata reqUpdateMetadata:
                    OnMetadataUpdated(reqUpdateMetadata.ConnectionId, reqUpdateMetadata.Name, reqUpdateMetadata.Metadata);
                    break;
                case RequestChangeUserStatus reqChangeUserStatus:
                    {
                        var user = GetUser(reqChangeUserStatus.SiteId, reqChangeUserStatus.UserId);
                        UpdateUser(user, reqChangeUserStatus);
                    }
                    break;
                case RequestAppClose close:
                    ClosingProcess();

                    _cts.Cancel();
                    break;
                default:
                    Debug.WriteLine($"Unknown request: {req}");
                    break;
            }
        }

        private bool CanRemoveConnection(ConnectionId connectionId)
        {
            var conn = GetConnection(connectionId);
            if (conn == null) return false;
            return (conn.IsConnected == false);
        }

        private void ClosingProcess()
        {
            //TODO:全プラグインに終了通知を出し、終了待ち
            _pluginManager.SetNotify(new NotifyCloseApp());
            //TODO:10秒程度経っても終了しないプラグインがあったら強制終了したい
            _pluginManager.UnloadPlugins();
            //McvUsersを保存する
        }

        public async Task SetRequestAsync(PluginId pluginId, IRequestAsync req)
        {
            Debug.WriteLine($"Model::SetRequestAsync req={req}");
            switch (req)
            {
                case RequestPostCommentAsync reqPostComment:
                    await _sitePluginManager.PostCommentAsync(reqPostComment.ConnectionId, reqPostComment.DataToPost);
                    break;
                case RequestUpdateCoreAsync reqUpdateCore:
                    {
                        ClosingProcess();

                        var exeFile = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                        var baseDir = System.IO.Path.GetDirectoryName(exeFile)!;
                        var zipFilename = "latest.zip";
                        var zipFilePath = System.IO.Path.Combine(baseDir, zipFilename);
                        System.IO.File.Delete(zipFilePath);

                        var wc = new WebClient();
                        wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                        try
                        {
                            await wc.DownloadFileTaskAsync(reqUpdateCore.LatestVersionUrl, zipFilePath);
                        }
                        catch (Exception ex)
                        {
                            //req = (error report)
                            //break;
                        }

                        //list.txtに記載されているファイル全てに.oldを付加
                        var listFilename = "list.txt";
                        if (!File.Exists(listFilename))
                        {

                        }
                        try
                        {
                            var list = new List<string>();
                            using (var sr = new System.IO.StreamReader(System.IO.Path.Combine(baseDir, "list.txt")))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var filename = sr.ReadLine();
                                    if (!string.IsNullOrEmpty(filename))
                                        list.Add(filename);
                                }
                            }
                            foreach (var filename in list)
                            {
                                var srcPath = System.IO.Path.Combine(baseDir, filename);
                                var dstPath = System.IO.Path.Combine(baseDir, filename + ".old");
                                try
                                {
                                    if (System.IO.File.Exists(srcPath))
                                    {
                                        System.IO.File.Delete(dstPath);//If the file to be deleted does not exist, no exception is thrown.
                                        System.IO.File.Move(srcPath, dstPath);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogException(ex, "", $"src={srcPath}, dst={dstPath}");
                                }
                            }
                        }
                        catch (System.IO.FileNotFoundException ex)
                        {
                            _logger.LogException(ex);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException(ex);
                        }
                        try
                        {
                            using (var archive = ZipFile.OpenRead(zipFilePath))
                            {
                                foreach (var entry in archive.Entries)
                                {
                                    var entryPath = System.IO.Path.Combine(baseDir, entry.FullName);
                                    var entryDir = System.IO.Path.GetDirectoryName(entryPath);
                                    if (!System.IO.Directory.Exists(entryDir))
                                    {
                                        System.IO.Directory.CreateDirectory(entryDir);
                                    }

                                    var entryFn = System.IO.Path.GetFileName(entryPath);
                                    if (!string.IsNullOrEmpty(entryFn))
                                    {
                                        try
                                        {
                                            entry.ExtractToFile(entryPath, true);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException(ex);
                        }
                        try
                        {
                            var pid = Process.GetCurrentProcess().Id;
                            System.Diagnostics.Process.Start(exeFile, "/updated " + pid);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException(ex);
                            return;
                        }
                        try
                        {
                            Process.GetCurrentProcess().Kill();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException(ex);
                        }
                    }
                    break;
                default:
                    Debug.WriteLine($"Unknown request: {req}");
                    break;
            }
        }
        private void Wc_DownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
        {
            _pluginManager.SetNotify(new NotifyDownloadCoreProgress(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage));
        }
        /// <summary>
        /// 指定したユーザーIDを持つユーザーを取得する
        /// 別々の配信サイト同士でユーザーIDが被る可能性があるから配信サイトも指定する必要がある。
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private McvUser? GetUser(SitePluginId siteGuid, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            McvUser user;
            if (!_userStore.Exists(siteGuid, userId))
            {
                user = new McvUser(siteGuid, userId);
                _userStore.AddUser(siteGuid, user);
                //_pluginManager.SetNotify(new NotifyUserAdded(siteGuid, userId));
            }
            else
            {
                user = _userStore.GetUser(siteGuid, userId);
            }
            return user;
        }
        /// <summary>
        /// ユーザーの現在の状態と変更要求を比較して、本当に変化する部分を取得する
        /// 現在の状態と同値の要求を弾くのが目的
        /// </summary>
        /// <param name="user">ユーザーの現在の状態</param>
        /// <param name="changeRequest">変更要求</param>
        /// <returns></returns>
        public static IMcvUserDiff GetUserDiff(IMcvUser user, IMcvUserDiff changeRequest)
        {
            //TODO:IMcvUserの項目に変更があった場合に対応し忘れる可能性があるからリフレクションを使って実装したい。
            var ret = new UserDiff(user.SiteId, user.Id);
            if (changeRequest.Nickname != null && user.Nickname != changeRequest.Nickname)
            {
                ret.Nickname = changeRequest.Nickname;
            }
            if (changeRequest.Name != null && user.Name != changeRequest.Name)
            {
                ret.Name = changeRequest.Name;
            }
            if (changeRequest.IsNgUser != null && user.IsNgUser != changeRequest.IsNgUser)
            {
                ret.IsNgUser = changeRequest.IsNgUser;
            }
            if (changeRequest.IsSiteNgUser != null && user.IsSiteNgUser != changeRequest.IsSiteNgUser)
            {
                ret.IsSiteNgUser = changeRequest.IsSiteNgUser;
            }
            return ret;
        }
        /// <summary>
        /// ユーザー情報を更新し、変更をプラグインに通知する
        /// </summary>
        /// <param name="user">対象のユーザー</param>
        /// <param name="changeRequest">変更要求</param>
        private void UpdateUser(IMcvUser user, IMcvUserDiff changeRequest)
        {
            var diff = GetUserDiff(user, changeRequest);
            user.Update(diff);
            _pluginManager.SetNotify(new NotifyUserChanged(diff));
        }
        private void OnMessageReceived(ConnectionId connectionId, ISiteMessage message, IMessageMetadata2 metadata)
        {
            var siteGuid = metadata.SiteContextGuid;
            var userId = metadata.UserId;

            var user = GetUser(siteGuid, userId);
            if (user != null)
            {
                UpdateUser(user, new UserDiff(siteGuid, userId)
                {
                    Name = metadata.UserName,
                    Nickname = metadata.NewNickname,
                });
            }
            _pluginManager.SetNotify(new NotifyMessageReceived(connectionId, message, metadata, user));
        }
        private void OnMetadataUpdated(ConnectionId connectionId, string? name, IMetadata? metadata)
        {
            var current = _metaDict[connectionId];
            _pluginManager.SetNotify(new NotifyMetadataUpdated(connectionId, name, metadata));
        }

        public async Task Run()
        {
            //適切に実行できる環境であるかを確認する
            //・zipファイルが展開されているか確認したい
            //・C:\Program Filesのような制限が掛かっているような場所に置かれていないか確認
            //・主要なdllの存在を確認

            //設定ファイルを置く場所を作成する
            if (!System.IO.Directory.Exists(SettingsDirName))
            {
                System.IO.Directory.CreateDirectory(SettingsDirName);
            }

            //プラグインを読み込む
            _pluginManager.LoadPluginFromDir(this, "plugins");

            //サイトプラグインを読み込む
            _sitePluginManager.Load();

            var browserLoader = new BrowserLoader(_logger);
            var browsers = browserLoader.LoadBrowsers().ToList();
            foreach (var browser in browsers)
            {
                AddBrowser(browser);
            }
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(500, _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
        public void AddSite(SitePluginId siteId, string siteDisplayName)
        {
            _pluginManager.SetNotify(new NotifySiteAdded
            {
                SiteId = siteId,
                SiteName = siteDisplayName,
            });
        }
        public void AddBrowser(IBrowserProfile2 browser)
        {
            _browserDict.Add(browser.Id, browser);
            _pluginManager.SetNotify(new NotifyBrowserAdded
            {
                BrowserId = browser.Id,
                BrowserName = browser.Type.ToString(),
                ProfileName = browser.ProfileName,
            });
        }
        /// <summary>
        /// 同期的に情報を取得する
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public IResponse GetData(IRequest req)
        {
            IResponse? res;
            switch (req)
            {
                case RequestConnectionIds getConnIds:
                    {
                        var ids = GetConnectionIds();
                        res = new ResponseConnectionIds(getConnIds.Id, ids);
                    }
                    break;
                case RequestConnectionStatus connStatus:
                    var connection = GetConnection(connStatus.ConnectionId);
                    if (connection != null)
                    {
                        res = new ResponseConnectionStatusAdded(connStatus.Id, connection);
                    }
                    else
                    {
                        res = new Error(connStatus.Id);//Errorを返したい
                    }

                    break;
                case RequestCoreVersion version:
                    {
                        var asm = System.Reflection.Assembly.GetExecutingAssembly();
                        var ver = asm.GetName().Version;
                        if (ver == null)
                        {
                            res = new Error(version.Id);
                            break;
                        }
                        var s = $"v{ver.Major}.{ver.Minor}.{ver.Build}";
                        res = new ResponseCoreVersion(version.Id, s);
                    }
                    break;
                case RequestCoreBuildType reqBuildType:
                    {
                        string s;
#if ALPHA
                        s = "アルファ版";
#elif BETA
                        s = "ベータ版";
#elif DEBUG
                        s = "DEBUG";
#else
                        s = "安定版";
#endif
                        res = new ResponseCoreBuildType(reqBuildType.Id, s);
                    }
                    break;
                case RequestLoadedPlugins reqLoadedPlugins:
                    var loadedPlugins = _pluginManager.GetLoadedPlugins();
                    res = new ResponseLoadedPlugins(reqLoadedPlugins.Id, loadedPlugins);
                    break;
                case RequestShowPluginSettingView reqShowPluginSettingView:
                    _pluginManager.ShowPluginSettingView(reqShowPluginSettingView.TargetPluginId);
                    res = new ResponseShowPluginSettingView(reqShowPluginSettingView.Id);
                    break;
                case RequestMainViewPosition reqMainViewPos:
                    var x = _coreOptions.MainViewPositionX;
                    var y = _coreOptions.MainViewPositionY;
                    res = new ResponseMainViewPosition(reqMainViewPos.Id, x, y);
                    break;
                case RequestUser reqUser:
                    var user = GetUser(reqUser.SiteGuid, reqUser.UserId);
                    res = new ResponseUser(reqUser.Id, user);
                    break;
                case GetSiteType reqSiteType:
                    var siteType = _sitePluginManager.GetSiteType(reqSiteType.SiteId);
                    res = new ResponseSiteType(reqSiteType.Id, siteType);
                    break;
                case RequestBrowser reqBrowser:
                    IBrowserProfile2 browserProfile;
                    if (_browserDict.ContainsKey(reqBrowser.BrowserId))
                    {
                        browserProfile = _browserDict[reqBrowser.BrowserId];
                        res = new ResponseBrowser(reqBrowser.Id, browserProfile);
                    }
                    else
                    {
                        res = new Error(reqBrowser.Id);
                    }
                    break;
                case RequestBrowsers reqBrowsers://読み込み済みのブラウザを全て取得する
                    res = new ResponseBrowsers(reqBrowsers.Id, _browserDict.Values);
                    break;
                case RequestSites reqSites:
                    res = new ResponseSites(reqSites.Id, _sitePluginManager.Sites());
                    break;
                default:
                    Debug.WriteLine($"Model::GetData() 未対応Reqest:{req}");
                    res = new Error(req.Id);
                    break;
            }
            return res;
        }
        public async Task<IResponse> GetDataAsync(IRequestAsync req)
        {
            IResponse res;
            switch (req)
            {
                case RequestPluginList reqPluginList:
                    {
                        var pluginList = new List<DownloadablePluginInfo>
                        {
                             new DownloadablePluginInfo
                             {
                                 Name = "プラグイン1",
                                 Type = PluginType.Dll,
                                 Url = "https://test.com/plugin1.zip",
                                 Size = 5_000_000,
                                 InstalledVersion = "1.6.8",
                                 LatestVersion = "1.7.2",
                             },
                            new DownloadablePluginInfo
                             {
                                 Name = "プラグイン2",
                                 Type = PluginType.Dll,
                                 Url = "https://test.com/plugin2.zip",
                                 Size = 78_000,
                                 InstalledVersion = "2.1.1",
                                 LatestVersion = "2.1.1",
                             },
                        };
                        res = new ResponsePluginList(reqPluginList.Id, pluginList);
                    }
                    break;
                case RequestLatestVersion reqLatestVersion:
                    {
#if ALPHA
                    var name = "multicommentviewer_alpha";
#elif BETA
                    var name="multicommentviewer_beta";
#elif DEBUG
                        //                    var name = "multicommentviewer_debug";
                        var name = "multicommentviewer_beta";
#else
                    var name = "multicommentviewer";
#endif
                        var url1 = @"https://ryu-s.github.io/" + name + "_latest";
                        using var client = new HttpClient();
                        var a = await client.GetAsync(url1);
                        var url2 = await a.Content.ReadAsStringAsync();

                        var a2 = await client.GetAsync(url2);
                        var json = await a2.Content.ReadAsStringAsync();
                        var latestVersionInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Common.AutoUpdate.LatestVersionInfo>(json);
                        res = new ResponseLatestVersion(req.Id, latestVersionInfo.Version.ToString(), latestVersionInfo.Url);
                    }
                    break;
                default:
                    Debug.WriteLine($"Model::GetDataAsync() 未対応Reqest:{req}");
                    res = new ResponseUnknownRequest(req.Id);
                    break;
            }
            return res;
        }

        private Connection2? GetConnection(ConnectionId connectionId)
        {
            if (_connectionDict.ContainsKey(connectionId))
            {
                return _connectionDict[connectionId];
            }
            else
            {
                return null;
            }
        }
        private ConnectionId[] GetConnectionIds()
        {
            return _connectionDict.Keys.ToArray();
        }

        public string SettingsDirPath => _coreOptions.SettingsDirPath;

        public Model(PluginManager pluginManager, ISitePluginManager sitePluginManager, IUserStore userStore, ILogger logger, IIo io, ICoreOptions coreOptions)
        {
            _pluginManager = pluginManager;
            pluginManager.PluginAdded += PluginManager_PluginAdded;

            _sitePluginManager = sitePluginManager;
            sitePluginManager.SiteAdded += SitePluginManager_SiteAdded;
            sitePluginManager.MetadataUpdated += SitePluginManager_MetadataUpdated;
            sitePluginManager.MessageReceived += SitePluginManager_MessageReceived;

            _userStore = userStore;

            _logger = logger;
            _io = io;
            _coreOptions = coreOptions;
            userStore.UserAdded += UserStore_UserAdded;

            var emptyBrowser = new EmptyBrowserProfile();
            _browserDict.Add(emptyBrowser.Id, emptyBrowser);
            EmptyBrowserId = emptyBrowser.Id;
        }

        private void SitePluginManager_MessageReceived(object? sender, RequestAddComment e)
        {
            OnMessageReceived(e.ConnectionId, e.Message, e.Metadata);
        }

        private void SitePluginManager_MetadataUpdated(object? sender, RequestUpdateMetadata e)
        {
            OnMetadataUpdated(e.ConnectionId, e.Name, e.Metadata);
        }

        private void SitePluginManager_SiteAdded(object? sender, SiteAddedEventArgs e)
        {
            AddSite(e.SiteId, e.SiteDisplayName);
        }

        private void UserStore_UserAdded(object? sender, UserAddedEventArgs e)
        {
            _pluginManager.SetNotify(new NotifyUserAdded(e.SiteGuid, e.UserId));
        }

        private void PluginManager_PluginAdded(object? sender, IMcvPluginV1 e)
        {
            var plugin = e;
            Debug.WriteLine($"Model::PluginManager_PluginAdded() Name:{plugin.Name} Id:{plugin.Id}");
            _pluginManager.SetNotify(new NotifyPluginAdded
            {
                PluginId = plugin.Id,
                PluginName = plugin.Name,
            });
        }

        public void SetError(Exception ex)
        {

        }

        public void SetError(string msg, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingFileLineNumber = 0)
        {
            throw new NotImplementedException();
        }
    }
}
