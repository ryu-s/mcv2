using mcv2;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcv2PluginInstallerPlugin
{
    [Export(typeof(IMcvPluginV1))]
    public class PluginMain : IMcvPluginV1
    {
        public PluginId Id { get; } = new PluginId();
        public IPluginHost Host { get; set; }
        public string Name { get; } = "プラグインインストーラー";
        MainViewModel _vm;
        Model _model;
        public void OnClosing()
        {
        }

        public void OnLoaded()
        {
            _model = new Model(this);
            _vm = new MainViewModel(_model);

        }

        public void SetNotify(INotify notify)
        {
        }

        public void SetResponse(IResponse res)
        {
        }

        public void ShowSettingView()
        {
            if (Host.GetData(new RequestMainViewPosition()) is ResponseMainViewPosition resPos)
            {
                var left = resPos.X;
                var top = resPos.Y;
                var view = new MainView
                {
                    Left = left,
                    Top = top,
                    DataContext = _vm
                };
                view.Show();
            }
            else
            {
                //バグ報告
            }
        }

        internal Task<IResponse> GetDataAsync(IRequestAsync req)
        {
            return Host.GetDataAsync(req);
        }
        internal void SetRequest(IRequest req)
        {
            Host.SetRequest(Id, req);
        }
    }
}
