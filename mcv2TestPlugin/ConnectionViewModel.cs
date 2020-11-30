using GalaSoft.MvvmLight;
using mcv2;
using SitePlugin;
using System;

namespace mcv2TestPlugin
{
    public class ConnectionViewModel : ViewModelBase
    {
        private string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
                RaisePropertyChanged();
            }
        }
        public ConnectionId ConnectionId { get; }
        public SitePluginId? SiteGuid { get; internal set; }

        public ConnectionViewModel(ConnectionId connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}
