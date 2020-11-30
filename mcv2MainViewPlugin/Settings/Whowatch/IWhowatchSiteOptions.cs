using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    public interface IWhowatchSiteOptions
    {
        bool NeedAutoSubNickname { get; set; }
        Color ItemBackColor { get; set; }
        Color ItemForeColor { get; set; }
        int LiveCheckIntervalSec { get; set; }
        int LiveDataRetrieveIntervalSec { get; set; }
    }
}
