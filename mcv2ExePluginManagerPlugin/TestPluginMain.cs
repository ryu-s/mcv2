using mcv2;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcv2ExePluginManagerPlugin
{
    [Export(typeof(IMcvPluginV1))]
    public class PluginMain : IMcvPluginV1
    {
        public PluginId Id { get; } = new PluginId();
        public IPluginHost Host { get; set; }
        public string Name { get; } = "exeプラグインマネージャー";

        public void OnClosing()
        {
        }

        public void OnLoaded()
        {
        }

        public void SetNotify(INotify notify)
        {
        }

        public void SetResponse(IResponse res)
        {
        }

        public void ShowSettingView()
        {
        }
    }
}