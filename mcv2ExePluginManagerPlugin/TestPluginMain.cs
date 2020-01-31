using mcv2;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcv2ExePluginManagerPlugin
{
    [Export(typeof(IPlugin2))]
    public class PluginMain : IPlugin2
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