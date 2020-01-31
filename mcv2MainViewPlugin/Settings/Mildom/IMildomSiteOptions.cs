using System.ComponentModel;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    interface IMildomSiteOptions : INotifyPropertyChanged
    {
        bool NeedAutoSubNickname { get; set; }
        bool IsShowJoinMessage { get; set; }
        bool IsShowLeaveMessage { get; set; }
        Color ItemForeColor { get; set; }
        Color ItemBackColor { get; set; }
        int PollingIntervalSec { get; set; }
    }
}
