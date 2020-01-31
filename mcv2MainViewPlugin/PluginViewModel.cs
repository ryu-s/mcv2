using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace mcv2.MainViewPlugin
{
    class PluginMenuItemViewModel : ViewModelBase
    {
        public string Name { get; }
        public ObservableCollection<PluginMenuItemViewModel> Children { get; } = new ObservableCollection<PluginMenuItemViewModel>();
        private RelayCommand _show;
        public ICommand ShowSettingViewCommand
        {
            //以前はコンストラクタ中でICommandに代入していたが、項目をクリックしてもTest()が呼ばれないことがあった。今の状態に書き換えたら問題なくなった。何故だ？IPluginを保持するようにしたから？GCで無くなっちゃってたとか？
            get
            {
                if (_show == null)
                {
                    _show = new RelayCommand(() => Test(PluginId));
                }
                return _show;
            }
        }

        private readonly IModel _host;
        public PluginId PluginId { get; }

        public PluginMenuItemViewModel(IModel host, PluginId id, string name)
        {
            _host = host;
            PluginId = id;
            Name = name;
        }

        private void Test(PluginId pluginId)
        {
            try
            {
                _host.RequestShowPluginSettingView(pluginId);
            }
            catch (Exception ex)
            {
                _host.SendError(ex, "", "");
            }
        }
    }
}
