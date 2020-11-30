using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
#nullable enable
    public interface IMainViewConnectionStatus : INotifyPropertyChanged
    {
        string Name { get; }
        /// <summary>
        /// Connection識別用ID
        /// </summary>
        string Guid { get; }
        //bool CanPostComment{get;}//接続中で且つ大抵のサイトではログイン済みである必要がある
        Color BackColor { get; }
        Color ForeColor { get; }
    }
    interface IConnectionViewModel : IMainViewConnectionStatus
    {
        ObservableCollection<BrowserViewModel> Browsers { get; }
        bool CanConnect { get; set; }
        bool CanDisconnect { get; set; }
        ICommand ConnectCommand { get; }
        ICommand DisconnectCommand { get; }
        ConnectionId Id { get; }
        string Input { get; set; }
        BrowserViewModel SelectedBrowser { get; set; }
        SiteViewModel SelectedSite { get; set; }
        ObservableCollection<SiteViewModel> Sites { get; }
        bool IsSelected { get; set; }
        bool NeedSave { get; set; }
        string LoggedInUsername { get; }
    }
    class ConnectionViewModel : ViewModelBase, IMainViewConnectionStatus, IConnectionViewModel
    {
        public SiteViewModel SelectedSite
        {
            get
            {
                return _selectedSite;
            }
            set
            {
                if (_selectedSite == value) return;
                _selectedSite = value;
                _host.SendRequest(new RequestChangeConnectionStatus(Id)
                {
                    Site = value?.SiteId,
                });
                RaisePropertyChanged();
            }
        }
        private readonly IModel _host;
        private readonly IConnectionModel _connHost;
        private bool _canConnect;
        private bool _canDisconnect;
        private string _loggedInUsername;
        private string _name;
        private string _input;
        private SiteViewModel _selectedSite;
        private BrowserViewModel _selectedBrowser;

        private bool _needSave;

        public BrowserViewModel SelectedBrowser
        {
            get
            {
                return _selectedBrowser;
            }
            set
            {
                if (_selectedBrowser == value) return;
                _selectedBrowser = value;
                _host.SendRequest(new RequestChangeConnectionStatus(Id)
                {
                    Browser = value?.BrowserId,
                });
                RaisePropertyChanged();
            }
        }
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public string Input
        {
            get
            {
                return _input;
            }
            set
            {
                //このコードだと他のプラグインでInputを変更した場合（_input != value）にもRequestChangeConnectionStatusをしてしまう
                if (_input == value) return;
                _input = value;
                _host.SendRequest(new RequestChangeConnectionStatus(Id)
                {
                    Input = value,
                });
                RaisePropertyChanged();
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value) return;
                _name = value;
                _host.SendRequest(new RequestChangeConnectionStatus(Id)
                {
                    Name = value,
                });
                RaisePropertyChanged();
            }
        }

        public ConnectionId Id { get; }
        public ObservableCollection<SiteViewModel> Sites { get; }
        public ObservableCollection<BrowserViewModel> Browsers { get; }

        public ConnectionViewModel(ConnectionId connectionId, IModel host, IConnectionModel connHost,
            ObservableCollection<SiteViewModel> sites, ObservableCollection<BrowserViewModel> browsers)
        {
            ConnectCommand = new RelayCommand(Connect);
            DisconnectCommand = new RelayCommand(Disconnect);
            Id = connectionId;
            _host = host;
            _connHost = connHost;
            Sites = sites;
            Browsers = browsers;
            CanConnect = true;
            CanDisconnect = false;
            _loggedInUsername = "";
        }

        private void Connect()
        {
            CanConnect = false;
            CanDisconnect = true;
            _host.SendRequest(new RequestChangeConnectionStatus(Id)
            {
                IsConnected = true,
            });
        }
        private void Disconnect()
        {
            _host.SendRequest(new RequestChangeConnectionStatus(Id)
            {
                IsConnected = false,
            });
        }
        public bool CanConnect
        {
            get
            {
                return _canConnect;
            }
            set
            {
                _canConnect = value;
                RaisePropertyChanged();
            }
        }
        public bool CanDisconnect
        {
            get
            {
                return _canDisconnect;
            }
            set
            {
                _canDisconnect = value;
                RaisePropertyChanged();
            }
        }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        string IMainViewConnectionStatus.Guid => Id.ToString();

        public bool IsSelected
        {
            get
            {
                return _connHost.GetIsSelected(Id);
            }
            set
            {
                _connHost.SetIsSelected(Id, value);
            }
        }
        public bool NeedSave
        {
            get
            {
                return _connHost.GetNeedSave(Id);
            }
            set
            {
                _connHost.SetNeedSave(Id, value);
            }
        }
        public string LoggedInUsername
        {
            get
            {
                return _loggedInUsername;
            }
        }
        public void UpdateLoggedInUsername(string newUsername)
        {
            _loggedInUsername = newUsername;
            RaisePropertyChanged(nameof(LoggedInUsername));
        }
    }
}
