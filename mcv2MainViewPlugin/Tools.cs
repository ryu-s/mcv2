using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcv2.MainViewPlugin
{
    static class Tools
    {
        public static bool IsInDesignMode()
        {
            return (bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(System.Windows.DependencyObject)).DefaultValue);
        }
    }
}
