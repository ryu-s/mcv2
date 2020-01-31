using SitePlugin;
using System;
using System.Collections.Generic;

namespace mcv2.Model
{
    public interface IUserStore
    {
        void AddUser(SitePluginId siteContextGuid, McvUser user);
        bool Exists(SitePluginId siteContextGuid, string userId);
        McvUser GetUser(SitePluginId siteGuid, string userId);
        /// <summary>
        /// ユーザーを取得する。存在しなければ追加もする。
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        McvUser GetOrCreateUser(SitePluginId siteGuid, string userId);
        event EventHandler<UserAddedEventArgs> UserAdded;
    }
    public class UserAddedEventArgs : EventArgs
    {
        public UserAddedEventArgs(SitePluginId siteGuid, string userId)
        {
            SiteGuid = siteGuid;
            UserId = userId;
        }

        public SitePluginId SiteGuid { get; }
        public string UserId { get; }
    }
    class TestUserStore : IUserStore
    {
        public event EventHandler<UserAddedEventArgs> UserAdded;
        private Dictionary<string, McvUser> GetSiteDict(SitePluginId siteContextGuid)
        {
            if (!_tempUserStore.TryGetValue(siteContextGuid, out var dict))
            {
                dict = new Dictionary<string, McvUser>();
                _tempUserStore.Add(siteContextGuid, dict);
            }
            return dict;
        }
        public bool Exists(SitePluginId siteContextGuid, string userId)
        {
            var dict = GetSiteDict(siteContextGuid);
            return dict.ContainsKey(userId);
        }
        public void AddUser(SitePluginId siteContextGuid, McvUser user)
        {
            var dict = GetSiteDict(siteContextGuid);
            dict.Add(user.Id, user);
            UserAdded?.Invoke(this, new UserAddedEventArgs(siteContextGuid, user.Id));
        }

        public McvUser GetUser(SitePluginId siteGuid, string userId)
        {
            if (!Exists(siteGuid, userId))
            {
                throw new ArgumentException($"This user does not exist: SiteGuid:{siteGuid}, UserId:{userId}");
            }
            var dict = _tempUserStore[siteGuid];
            return dict[userId];
        }

        public McvUser GetOrCreateUser(SitePluginId siteGuid, string userId)
        {
            McvUser user;
            if (Exists(siteGuid, userId))
            {
                user = new McvUser(userId);
                AddUser(siteGuid, user);
            }
            else
            {
                user = GetUser(siteGuid, userId);
            }
            return user;
        }

        readonly Dictionary<SitePluginId, Dictionary<string, McvUser>> _tempUserStore = new Dictionary<SitePluginId, Dictionary<string, McvUser>>();
    }
}
