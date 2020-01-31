using ryu_s.BrowserCookie;
using System;

namespace Common
{
    public class BrowserViewModel
    {
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if(obj is BrowserViewModel vm)
            {
                return this.Id.Equals(vm.Id);
            }
            return false;
        }
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_profileName))
                {
                    return _browserName;
                }
                else
                {
                    return $"{_browserName}({_profileName})";
                }
            }
        }
        public Guid Id;
        private readonly string _browserName;
        private readonly string _profileName;

        public BrowserViewModel(Guid profileGuid,string browserName,string profileName)
        {
            Id = profileGuid;
            _browserName = browserName;
            _profileName = profileName;
        }
    }
}
