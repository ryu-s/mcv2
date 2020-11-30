using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class WhowatchSiteOptionsCopy : OptionsCopyBase<IWhowatchSiteOptions>, IWhowatchSiteOptions
    {
        public bool NeedAutoSubNickname { get; set; }
        public Color ItemBackColor { get; set; }
        public Color ItemForeColor { get; set; }
        public int LiveCheckIntervalSec { get; set; }
        public int LiveDataRetrieveIntervalSec { get; set; }
    }
}
