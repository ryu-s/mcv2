using System;
using System.Collections.Generic;

namespace mcv2.MainViewPlugin
{
    internal partial class MainViewModel
    {
        class UserViewModelStore
        {
            private readonly Dictionary<Guid, Dictionary<string, UserViewModel>> _userViewModelDict = new Dictionary<Guid, Dictionary<string, UserViewModel>>();
            public UserViewModel? Get(Guid siteContextGuid, string userId)
            {
                var dict = GetDict(siteContextGuid);
                if (dict.TryGetValue(userId, out var uvm))
                {
                    return uvm;
                }
                else
                {
                    return null;
                }
            }
            private Dictionary<string, UserViewModel> GetDict(Guid siteContextGuid)
            {
                if (!_userViewModelDict.TryGetValue(siteContextGuid, out var dict))
                {
                    dict = new Dictionary<string, UserViewModel>();
                    _userViewModelDict.Add(siteContextGuid, dict);
                }
                return dict;
            }
            public void Set(Guid siteContextGuid, string userId, UserViewModel vm)
            {
                var dict = GetDict(siteContextGuid);
                dict.Add(userId, vm);
            }
        }
    }
}
