using System.ComponentModel;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class MirrativSiteOptionsCopy : OptionsCopyBase<IMirrativSiteOptions>, IMirrativSiteOptions
    {
        public bool NeedAutoSubNickname { get; set; }
        public bool IsShowJoinMessage { get; set; }
        public bool IsShowLeaveMessage { get; set; }
        public Color ItemForeColor { get; set; }
        public Color ItemBackColor { get; set; }
        public int PollingIntervalSec { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
