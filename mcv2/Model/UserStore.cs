using Common;
using Microsoft.Data.Sqlite;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace mcv2.Model
{
    public interface IUserStore
    {
        Task InitAsync();
        Task SaveAsync();
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
    class SqliteUserStore : IUserStore
    {
        public event EventHandler<UserAddedEventArgs> UserAdded;
        public string FilePath { get; }
        public void AddUser(SitePluginId siteContextGuid, McvUser user)
        {
            _internal.AddUser(siteContextGuid, user);
        }

        public bool Exists(SitePluginId siteContextGuid, string userId)
        {
            return _internal.Exists(siteContextGuid, userId);
        }

        public McvUser GetOrCreateUser(SitePluginId siteGuid, string userId)
        {
            return _internal.GetOrCreateUser(siteGuid, userId);
        }

        public McvUser GetUser(SitePluginId siteGuid, string userId)
        {
            return _internal.GetUser(siteGuid, userId);
        }

        public async Task InitAsync()
        {
            if (!System.IO.File.Exists(FilePath)) return;
            var list = new List<(SitePluginId, McvUser)>();
            const string tableName = "users";
            var connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = FilePath,
                Mode = SqliteOpenMode.ReadWrite,
            }.ToString();
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    if (!ExistTable(connection, tableName))
                    {
                        CreateTable(connection, tableName);
                    }
                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT id,json FROM {tableName}";

                    using (var reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            var id = reader.GetString(0);
                            var siteIdRaw = reader.GetString(1);
                            var siteGuid = Guid.Parse(siteIdRaw);
                            var siteId = new SitePluginId(siteGuid);
                            var json = reader.GetString(2);
                            var user = McvUser.Deserialize(json);
                            if (user != null)
                            {
                                list.Add((siteId, user));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            _internal.AddRangeUser(list);
        }

        private void CreateTable(SqliteConnection connection, string tableName)
        {
            //SELECT name FROM sqlite_master WHERE type='table' AND name='beispiel';
            var command = connection.CreateCommand();
            command.CommandText = $"CREATE TABLE {tableName} (id TEXT,siteid Text, json TEXT)";
            command.ExecuteNonQuery();
        }

        private bool ExistTable(SqliteConnection connection, string tableName)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    return true;
                }
            }
            return false;
        }

        public async Task SaveAsync()
        {
            var users = _internal.GetAllUsers();
            const string tableName = "users";
            var connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = FilePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
            }.ToString();
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    if (!ExistTable(connection, tableName))
                    {
                        CreateTable(connection, tableName);
                    }
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (var (siteId, user) in users)
                        {
                            if (!McvUser.HasChanged(user))
                            {
                                continue;
                            }
                            var command = connection.CreateCommand();
                            command.CommandText = $"INSERT INTO {tableName} (id,siteid,json) VALUES ($id,$siteid,$json)";
                            command.Parameters.AddWithValue("$id", user.Id);
                            command.Parameters.AddWithValue("$siteid", siteId.ToString());
                            command.Parameters.AddWithValue("$json", McvUser.Serialize(user));
                            await command.ExecuteNonQueryAsync();
                        }
                        await transaction.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }
        TestUserStore _internal;
        private readonly ILogger _logger;

        public SqliteUserStore(string filePath, ILogger logger)
        {
            _internal = new TestUserStore();
            _internal.UserAdded += _internal_UserAdded;
            FilePath = filePath;
            _logger = logger;
            SQLitePCL.Batteries.Init();
        }

        private void _internal_UserAdded(object? sender, UserAddedEventArgs e)
        {
            UserAdded?.Invoke(this, e);
        }
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
        public List<(SitePluginId, McvUser)> GetAllUsers()
        {
            var list = new List<(SitePluginId, McvUser)>();
            foreach (var (siteId, dict) in _tempUserStore)
            {
                foreach (var (_, user) in dict)
                {
                    list.Add((siteId, user));
                }
            }
            return list;
        }
        public void SetUsers(IEnumerable<(SitePluginId, McvUser)> users)
        {
            foreach (var (siteId, user) in users)
            {
                var siteDict = GetSiteDict(siteId);
                siteDict.Add(user.Id, user);
            }
        }

        public Task InitAsync()
        {
            return Task.CompletedTask;
        }

        public Task SaveAsync()
        {
            return Task.CompletedTask;
        }

        public void AddRangeUser(IEnumerable<(SitePluginId, McvUser)> users)
        {
            foreach (var (siteId, user) in users)
            {
                var dict = GetSiteDict(siteId);
                dict.Add(user.Id, user);
            }
        }

        readonly Dictionary<SitePluginId, Dictionary<string, McvUser>> _tempUserStore = new Dictionary<SitePluginId, Dictionary<string, McvUser>>();
    }
}
