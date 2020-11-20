﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using mcv2;
using SitePlugin;

namespace mcv2YoyakuPlugin
{
    public class UserStore
    {
        readonly Dictionary<SitePluginId, Dictionary<string, YoyakuUser>> _dict = new Dictionary<SitePluginId, Dictionary<string, YoyakuUser>>();
        public void AddUser(YoyakuUser user)
        {
            var siteContextGuid = user.SitePluginGuid;
            var dict = GetOrCreateDict(siteContextGuid);
            dict.Add(user.Id, user);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <param name="user"></param>
        internal void RemoveUser(YoyakuUser user)
        {
            var siteContextGuid = user.SitePluginGuid;
            var dict = GetOrCreateDict(siteContextGuid);
            dict.Remove(user.Id);
        }

        private Dictionary<string, YoyakuUser> GetOrCreateDict(SitePluginId siteGuid)
        {
            if (!_dict.TryGetValue(siteGuid, out var dict))
            {
                dict = new Dictionary<string, YoyakuUser>();
                _dict.Add(siteGuid, dict);
            }
            return dict;
        }
        internal YoyakuUser GetUser(SitePluginId siteGuid, string userId)
        {
            var dict = GetOrCreateDict(siteGuid);
            if (dict.TryGetValue(userId, out var user))
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
    public class Model : INotifyPropertyChanged
    {
        private readonly DynamicOptions _options;
        private readonly IPluginHost _host;
        private YoyakuUser _currentUser;
        public string ReserveCommandPattern
        {
            get { return _options.ReserveCommandPattern; }
            set { _options.ReserveCommandPattern = value; }
        }
        public string DeleteCommandPattern
        {
            get { return _options.DeleteCommandPattern; }
            set { _options.DeleteCommandPattern = value; }
        }
        public string Reserved_Se
        {
            get { return _options.Reserved_Se; }
            set { _options.Reserved_Se = value; }
        }
        public string Reserved_Message
        {
            get { return _options.Reserved_Message; }
            set { _options.Reserved_Message = value; }
        }
        public string Delete_Se
        {
            get { return _options.Delete_Se; }
            set { _options.Delete_Se = value; }
        }
        public string Delete_Message
        {
            get { return _options.Delete_Message; }
            set { _options.Delete_Message = value; }
        }
        public string Call_Se
        {
            get { return _options.Call_Se; }
            set { _options.Call_Se = value; }
        }
        public string Call_Message
        {
            get { return _options.Call_Message; }
            set { _options.Call_Message = value; }
        }
        public string AlreadyReserved_Se
        {
            get { return _options.AlreadyReserved_Se; }
            set { _options.AlreadyReserved_Se = value; }
        }
        public string AlreadyReserved_Message
        {
            get { return _options.AlreadyReserved_Message; }
            set { _options.AlreadyReserved_Message = value; }
        }
        public string HandleNameNotSubscribed_Se
        {
            get { return _options.HandleNameNotSubscribed_Se; }
            set { _options.HandleNameNotSubscribed_Se = value; }
        }
        public string HandleNameNotSubscribed_Message
        {
            get { return _options.HandleNameNotSubscribed_Message; }
            set { _options.HandleNameNotSubscribed_Message = value; }
        }
        public string NotReserved_Se
        {
            get { return _options.NotReserved_Se; }
            set { _options.NotReserved_Se = value; }
        }
        public string NotReserved_Message
        {
            get { return _options.NotReserved_Message; }
            set { _options.NotReserved_Message = value; }
        }
        public string DeleteByOther_Se
        {
            get { return _options.DeleteByOther_Se; }
            set { _options.DeleteByOther_Se = value; }
        }
        public string DeleteByOther_Message
        {
            get { return _options.DeleteByOther_Message; }
            set { _options.DeleteByOther_Message = value; }
        }
        public string Explain_Se
        {
            get { return _options.Explain_Se; }
            set { _options.Explain_Se = value; }
        }
        public string Explain_Message
        {
            get { return _options.Explain_Message; }
            set { _options.Explain_Message = value; }
        }
        public double DateWidth
        {
            get => _options.DateWidth;
            set => _options.DateWidth = value;
        }
        public double IdWidth
        {
            get => _options.IdWidth;
            set => _options.IdWidth = value;
        }
        public double NameWidth
        {
            get => _options.NameWidth;
            set => _options.NameWidth = value;
        }
        public double CalledWidth
        {
            get => _options.CalledWidth;
            set => _options.CalledWidth = value;
        }
        public Model(DynamicOptions options, IPluginHost host)
        {
            _options = options;
            _host = host;
            options.PropertyChanged += Options_PropertyChanged;

            TestComment = "abc";
            TestPattern = "abc";
            SetReserveCommandPattern(options.ReserveCommandPattern);
            SetDeleteCommandPattern(options.DeleteCommandPattern);
        }
        private Regex _reserveCommandRegex;

        internal void ChangeUserStatus(SitePluginId siteGuid, string userId, string name, string nickname, bool? isNgUser, bool? isSiteNgUser)
        {
            var user = _userStore.GetUser(siteGuid, userId);
            if (user == null)
            {
                //このプラグインに登録されたユーザーじゃないからスルー。
                return;
            }
            if (name != null)
            {
                user.Name = name;
            }
            if (!string.IsNullOrEmpty(nickname))
            {
                user.Nickname = nickname;
            }
        }

        private Regex _deleteCommandRegex;
        private void SetReserveCommandPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return;

            Regex regex;
            try
            {
                regex = CreateCommandRegex(pattern);
            }
            catch (Exception)
            {
                regex = CreateCommandRegex("/yoyaku");
            }
            _reserveCommandRegex = regex;
        }
        private Regex CreateCommandRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) throw new ArgumentNullException(nameof(pattern));
            return new Regex(EscapePattern(pattern), RegexOptions.Singleline | RegexOptions.Compiled);
        }
        private void SetDeleteCommandPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return;

            Regex regex;
            try
            {
                regex = CreateCommandRegex(pattern);
            }
            catch (Exception)
            {
                regex = CreateCommandRegex("/torikeshi");
            }
            _deleteCommandRegex = regex;
        }
        private void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_options.ReserveCommandPattern):
                    SetReserveCommandPattern(_options.ReserveCommandPattern);
                    break;
                case nameof(_options.DeleteCommandPattern):
                    SetDeleteCommandPattern(_options.DeleteCommandPattern);
                    break;
            }
        }

        /// <summary>
        /// 登録済みのN人を呼び出す
        /// </summary>
        /// <param name="n"></param>
        public void CallNPeople(int n)
        {
            var toCall = new List<YoyakuUser>();

            //N人をピックアップ
            lock (RegisteredUsers)
            {
                foreach (var user in RegisteredUsers)
                {
                    if (toCall.Count >= n)
                        break;
                    if (!user.HadCalled)
                        toCall.Add(user);
                }
            }

            //呼び出しコメント
            if (toCall.Count > 0)
            {
                var names = string.Join("、", toCall.Select(user => user.Name + "さん"));
                var s = Call_Message.Replace("$name", names);
                WriteComment(s);
            }

            //呼出済にする
            foreach (var called in toCall)
            {
                called.HadCalled = true;
            }
        }
        public List<YoyakuUser> RegisteredUsers { get; } = new List<YoyakuUser>();

        public void ChangeRegisteredUsers(System.Collections.Specialized.NotifyCollectionChangedAction action, int oldIndex, int newIndex)
        {
            switch (action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    var item = RegisteredUsers[oldIndex];
                    RegisteredUsers.Remove(item);
                    RegisteredUsers.Insert(newIndex, item);
                    break;
            }
        }
        UserStore _userStore = new UserStore();
        public void AddUser(YoyakuUser user)
        {
            lock (RegisteredUsers)
            {
                RegisteredUsers.Add(user);
                _userStore.AddUser(user);
            }
            RaiseUsersListChanged();
        }
        private void RemoveUser(YoyakuUser user)
        {
            lock (RegisteredUsers)
            {
                RegisteredUsers.Remove(user);
                _userStore.RemoveUser(user);
            }
            RaiseUsersListChanged();
        }
        private YoyakuUser FindUser(string userId)
        {
            lock (RegisteredUsers)
            {
                foreach (var user in RegisteredUsers)
                {
                    if (user.Id == userId)
                        return user;
                }
            }
            return null;
        }
        protected virtual DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
        private bool IsYoyakuCommand(string comment)
        {
            return _reserveCommandRegex.IsMatch(comment);
        }
        private bool IsTorikeshiCommand(string comment)
        {
            return _deleteCommandRegex.IsMatch(comment);
        }
        private bool IsKakuninCommand(string comment)
        {
            return comment.StartsWith("/kakunin") || comment.StartsWith("/確認");
        }
        public void SetComment(string userId, string name, string comment, IMcvUser user, SitePluginId siteContextGuid)
        {
            //"/yoyaku"
            //"/torikeshi"
            //"/kakunin"

            if (string.IsNullOrEmpty(userId)) return;

            string defaultName;
            if (user != null && !string.IsNullOrEmpty(user.Nickname))
            {
                defaultName = user.Nickname;
            }
            else
            {
                defaultName = name;
            }

            if (IsYoyakuCommand(comment))
            {
                if (FindUser(userId) == null)
                {
                    //未登録なら登録する
                    var pluginUser = new YoyakuUser { Date = GetCurrentDateTime(), Id = userId, Name = defaultName, HadCalled = false, SitePluginGuid = siteContextGuid };
                    AddUser(pluginUser);
                    WriteComment(Reserved_Message.Replace("$name", defaultName));
                }
            }
            else if (IsTorikeshiCommand(comment))
            {
                var pluginUser = FindUser(userId);
                if (pluginUser != null)
                {
                    //登録済みなら取り消す
                    RemoveUser(pluginUser);
                    WriteComment(Delete_Message.Replace("$name", defaultName));
                }
            }
            else if (IsKakuninCommand(comment))
            {
                //未登録の場合は何も表示されない。
                lock (RegisteredUsers)
                {
                    var i = 0;
                    foreach (var pluginUser in RegisteredUsers)
                    {
                        if (pluginUser.Id == userId && !pluginUser.HadCalled)
                        {
                            WriteComment($"ただいまの予約人数は{RegisteredUsers.Count}名です。{defaultName}さんは、{i + 1}番目です");
                            break;
                        }
                        if (!pluginUser.HadCalled)
                            i++;
                    }
                }
            }
        }
        /// <summary>
        /// 呼び出し済みのユーザを削除する
        /// </summary>
        public void RemoveCalledUsers()
        {
            lock (RegisteredUsers)
            {
                var toRemove = new List<YoyakuUser>();
                foreach (var user in RegisteredUsers)
                {
                    if (user.HadCalled)
                        toRemove.Add(user);
                }
                foreach (var removeUser in toRemove)
                {
                    RemoveUser(removeUser);
                }
            }
            RaiseUsersListChanged();
        }
        public void RemoveAllUsers()
        {
            RegisteredUsers.Clear();
            RaiseUsersListChanged();
        }
        public void RemoveCurrentUser()
        {
            if (CurrentUser != null)
            {
                RemoveUser(CurrentUser);
                CurrentUser = null;
            }
        }
        public void WriteExplain()
        {
            WriteComment(Explain_Message);
        }
        private void RaiseUsersListChanged()
        {
            UsersListChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler UsersListChanged;
        public YoyakuUser CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                RaisePropertyChanged();
            }
        }
        public bool IsEnabled
        {
            get => _options.IsEnabled;
            set => _options.IsEnabled = value;
        }
        public string TestPattern
        {
            get
            {
                return _testPattern;
            }
            set
            {
                _testPattern = value;
                Test();
            }
        }
        /// <summary>
        /// 予約管理プラグインの正規表現に不要な正規表現を制限する
        /// </summary>
        /// <param name="raw">ユーザが入力したパターンの生の値</param>
        /// <returns>不要な正規表現を無効化したパターン</returns>
        private string EscapePattern(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return raw;
            var s = raw;
            return s;
        }
        public string TestResult
        {
            get
            {
                return _testResult;
            }
            set
            {
                _testResult = value;
                RaisePropertyChanged();
            }
        }
        public string TestComment
        {
            get
            {
                return _testComment;
            }
            set
            {
                _testComment = value;
                Test();
            }
        }
        private void Test()
        {
            string result;
            if (string.IsNullOrEmpty(TestPattern))
            {
                result = $"{nameof(TestPattern)}が空欄です";
            }
            else if (string.IsNullOrEmpty(TestComment))
            {
                result = $"{nameof(TestComment)}が空欄です";
            }
            else
            {
                try
                {
                    var regex = CreateCommandRegex(TestPattern);
                    result = regex.IsMatch(TestComment).ToString();
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            TestResult = result;
        }
        private void WriteComment(string comment)
        {
            //TODO:コメントを投稿できるようにする
            //_host.PostCommentToAll(comment);
        }
        private string _testPattern;
        private string _testResult;
        private string _testComment;

        #region INotifyPropertyChanged
        [NonSerialized]
        private System.ComponentModel.PropertyChangedEventHandler _propertyChanged;

        /// <summary>
        /// 
        /// </summary>
        public virtual event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            _propertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
