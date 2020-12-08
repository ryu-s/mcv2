using SitePlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace mcv2.MainViewPlugin
{
    class UserStore
    {
        readonly Dictionary<SitePluginId, Dictionary<string, UserViewModel>> _dict = new Dictionary<SitePluginId, Dictionary<string, UserViewModel>>();
        private readonly IOptions _options;
        private readonly IUserHost _host;

        public UserViewModel GetOrCreateUser(SitePluginId siteContextGuid, string userId)
        {
            Debug.Assert(userId != null);
            if (!_dict.TryGetValue(siteContextGuid, out var dict))
            {
                dict = new Dictionary<string, UserViewModel>();
                _dict.Add(siteContextGuid, dict);
            }
            if (!dict.TryGetValue(userId, out var user))
            {
                user = new UserViewModel(siteContextGuid, userId, _options, _host);
                dict.Add(userId, user);
            }
            return user;
        }
        public UserStore(IOptions options, IUserHost host)
        {
            _options = options;
            _host = host;
        }
    }
}
