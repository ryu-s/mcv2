using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using mcv2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mcv2PluginInstallerPlugin
{
    class PluginViewModel : ViewModelBase
    {
        public ICommand InstallCommand { get; }
        private string _status;
        private readonly Model _model;

        public string Name { get; set; }
        public string Url { get; set; }
        public string LatestVersion { get; set; }
        public string InstalledVersion { get; set; }
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }
        public PluginViewModel()
        {

        }

        public PluginViewModel(DownloadablePluginInfo plugin, Model model)
        {
            Name = plugin.Name;
            Url = plugin.Url;
            LatestVersion = plugin.LatestVersion;
            InstalledVersion = plugin.InstalledVersion;

            InstallCommand = new RelayCommand(Install);
            _model = model;
        }
        private void Install()
        {
            var filename = GetFilename(Url);
            _model.InstallPlugin(filename);
        }
        private string GetFilename(string url)
        {
            Uri uri = new Uri(url);
            var fileName = Path.GetFileName(uri.LocalPath);
            return fileName;
        }
    }
    class MainViewModel : ViewModelBase
    {
        private readonly Model _model;

        public ICommand UpdateCommand { get; }
        public ObservableCollection<PluginViewModel> Plugins { get; } = new ObservableCollection<PluginViewModel>();
        [PreferredConstructor]
        public MainViewModel(Model model)
        {
            UpdateCommand = new RelayCommand(Update);
            _model = model;
            model.Updated += Model_Updated;
        }
        public MainViewModel()
        {
            Plugins.Add(new PluginViewModel
            {
                Name = "plugin1",
                InstalledVersion = "0.2.5",
                LatestVersion = "1.6.0",
                Status = "インストール中 (28%)",
                Url = "https://example.com/pluin1.zip",
            });
            Plugins.Add(new PluginViewModel
            {
                Name = "plugin2",
                InstalledVersion = "1.9.1",
                LatestVersion = "1.9.2",
                Status = "インストール中 (96%)",
                Url = "https://example.com/pluin2.zip",
            });
            Plugins.Add(new PluginViewModel
            {
                Name = "plugin3",
                InstalledVersion = "",
                LatestVersion = "",
                Status = "",
                Url = "",
            });
        }
        private void Update()
        {
            Plugins.Clear();
            _model.Update();
        }
        private void Model_Updated(object sender, UpdatedEventArgs args)
        {
            var plugins = args.Plugins;
            foreach (var plugin in plugins)
            {
                Plugins.Add(new PluginViewModel(plugin, _model));
            }
        }
    }

    class UpdatedEventArgs
    {
        public List<DownloadablePluginInfo> Plugins { get; set; }
    }

    class Model
    {
        private readonly PluginMain _plugin;

        public event EventHandler<UpdatedEventArgs> Updated;
        public Model(PluginMain plugin)
        {
            _plugin = plugin;
        }
        internal async void Update()
        {
            var res = await _plugin.GetDataAsync(new RequestPluginList()) as ResponsePluginList;
            if (res == null)
            {
                return;
            }
            Updated?.Invoke(this, new UpdatedEventArgs
            {
                Plugins = res.PluginList,
            });
        }

        internal void InstallPlugin(string filename)
        {
            _plugin.SetRequest(new RequestInstallPlugin());
        }
    }


}
