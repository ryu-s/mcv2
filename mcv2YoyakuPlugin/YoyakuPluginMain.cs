using Common.Wpf;
using mcv2;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace mcv2YoyakuPlugin
{
    [Export(typeof(IPlugin2))]
    public class YoyakuPluginMain : IPlugin2
    {
        public PluginId Id { get; } = new PluginId();
        public IPluginHost Host { get; set; }
        public string Name => "予約管理プラグイン";
        private readonly DynamicOptions _options = new DynamicOptions();
        SettingsViewModel _vm;
        private Dispatcher _dispatcher;
        Model _model;
        public void OnClosing()
        {
        }
        private void LoadOptions()
        {
            try
            {
                var s = Host.LoadOptions(GetSettingsFilePath());
                _options.Deserialize(s);
            }
            catch (System.IO.FileNotFoundException) { }
        }
        protected virtual Model CreateModel()
        {
            return new Model(_options, Host);
        }
        public void OnLoaded()
        {
            System.Diagnostics.Debug.WriteLine($"mcv2YoyakuPlugin::PluginMain::OnLoaded()");

            _dispatcher = Dispatcher.CurrentDispatcher;
            LoadOptions();
            _model = CreateModel();
            _model.UsersListChanged += Model_UsersListChanged;
            _vm = CreateSettingsViewModel();
            LoadRegisteredUsers();
        }
        private string GetPath()
        {
            var dir = Host.SettingsDirPath;
            var path = Path.Combine(dir, $"{Name}_users.txt");
            return path;
        }
        /// <summary>
        /// ファイルから読み込んだユーザー情報
        /// </summary>
        class LoadedUserData
        {
            public string UserId { get; internal set; }
            public string Name { get; internal set; }
            public string Nickname { get; internal set; }
            public DateTime Date { get; internal set; }
            public bool HasCalled { get; internal set; }
            public SitePluginId SiteGuid { get; internal set; }

            public override bool Equals(object? obj)
            {
                return obj is LoadedUserData data &&
                       UserId == data.UserId &&
                       SiteGuid.Equals(data.SiteGuid);
            }

            public override int GetHashCode()
            {
                int hashCode = 62662510;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserId);
                hashCode = hashCode * -1521134295 + SiteGuid.GetHashCode();
                return hashCode;
            }
        }
        private List<LoadedUserData> ParseUserData(string raw)
        {
            var list = new List<LoadedUserData>();
            if (string.IsNullOrEmpty(raw))
            {
                return list;
            }
            var lines = raw.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var arr = line.Split('\t');
                if (arr.Length != 6) continue;
                try
                {
                    var userId = arr[0];
                    var name = arr[1];
                    var nick = arr[2];
                    var date = DateTime.Parse(arr[3]);
                    var hasCalled = arr[4] == "True";
                    var siteGuid = new SitePluginId(new Guid(arr[5]));
                    //var siteType =
                    list.Add(new LoadedUserData
                    {
                        UserId = userId,
                        Name = name,
                        Nickname = nick,
                        Date = date,
                        HasCalled = hasCalled,
                        SiteGuid = siteGuid,
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return list;
        }
        Dictionary<(SitePluginId, string), LoadedUserData> _loadedUserDict = new Dictionary<(SitePluginId, string), LoadedUserData>();
        private void LoadRegisteredUsers()
        {
            var path = GetPath();
            var s = Host.LoadOptions(path);
            var loadedUsers = ParseUserData(s);
            foreach (var loadedUser in loadedUsers)
            {
                _loadedUserDict.Add((loadedUser.SiteGuid, loadedUser.UserId), loadedUser);
                ////ちょっとトリッキーだけど、本体にユーザーを追加する。
                ////また、ここでNotifyUserAddedが飛んでくるから、
                var _ = GetUser(loadedUser.SiteGuid, loadedUser.UserId);
                //_model.AddUser(new YoyakuUser
                //{
                //    Date = loadedUser.Date,
                //    HadCalled = loadedUser.HasCalled,
                //    Id = loadedUser.UserId,
                //    Name = loadedUser.Name,
                //    SitePluginGuid = loadedUser.SiteGuid,
                //});
            }
        }
        private IMcvUser GetUser(SitePluginId siteGuid, string userId)
        {
            var res = Host.GetData(new RequestUser(siteGuid, userId)) as ResponseUser;
            return res.User;
        }
        private void Model_UsersListChanged(object sender, EventArgs e)
        {
            var users = _model.RegisteredUsers.ToArray();
            var s = "";
            foreach (var user in users)
            {
                s += $"{user.Id}\t{user.Name}\t{user.Nickname}\t{user.Date}\t{user.HadCalled}\t{user.SitePluginGuid}" + Environment.NewLine;
            }
            var path = GetPath();
            Host.SaveOptions(path, s);
        }
        protected virtual SettingsViewModel CreateSettingsViewModel()
        {
            return new SettingsViewModel(_model, _dispatcher);
        }
        public string GetSettingsFilePath()
        {
            var dir = Host.SettingsDirPath;
            return System.IO.Path.Combine(dir, $"{Name}.xml");
        }
        public void SetNotify(INotify notify)
        {
            switch (notify)
            {
                case NotifyMessageReceived msgReceived:
                    OnMessageReceived(msgReceived.Message, msgReceived.Metadata, msgReceived.User);
                    break;
                case NotifyUserAdded userAdded:
                    {
                        var siteGuid = userAdded.SiteContextGuid;
                        var userId = userAdded.UserId;

                        if (_loadedUserDict.TryGetValue((siteGuid, userId), out var loadedUserData))
                        {
                            //"予約管理プラグイン_users.txt"にこのユーザーに関する情報が保存されていた場合
                            _model.AddUser(new YoyakuUser()
                            {
                                SitePluginGuid = siteGuid,
                                Id = userId,
                                Date = loadedUserData.Date,
                                Name = loadedUserData.Name,
                                HadCalled = loadedUserData.HasCalled,
                                Nickname = loadedUserData.Nickname,
                            });
                        }
                    }
                    break;
                case NotifyUserChanged userChanged:
                    {
                        var siteGuid = userChanged.SiteContextGuid;
                        var userId = userChanged.UserId;
                        var name = userChanged.Name.ToText();
                        var nickname = userChanged.Nickname;
                        var isNgUser = userChanged.IsNgUser;
                        var isSiteNgUser = userChanged.IsSiteNgUser;
                        _model.ChangeUserStatus(siteGuid, userId, name, nickname, isNgUser, isSiteNgUser);
                    }
                    break;
            }
        }
        public void OnMessageReceived(ISiteMessage message, IMessageMetadata2 messageMetadata, IMcvUser user)
        {
            if (!_options.IsEnabled || messageMetadata.IsNgUser || messageMetadata.IsInitialComment)
                return;

            var (name, text) = PluginCommon.Tools.GetData(message);
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
            {
                _model.SetComment(messageMetadata.UserId, name, text, user, messageMetadata.SiteContextGuid);
            }
        }
        public void SetResponse(IResponse res)
        {
        }

        public void ShowSettingView()
        {
            if (Host.GetData(new RequestMainViewPosition()) is ResponseMainViewPosition resPos)
            {
                var left = resPos.X;
                var top = resPos.Y;
                var view = new SettingsView
                {
                    Left = left,
                    Top = top,
                    DataContext = _vm
                };
                view.Show();
            }
            else
            {
                //バグ報告
            }
        }
    }
}
