using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    interface ITwicasSiteOptions
    {
        int CommentRetrieveIntervalSec { get; set; }
        Color KiitosBackColor { get; set; }
        Color KiitosForeColor { get; set; }
        bool IsAutoSetNickname { get; set; }
        Color ItemBackColor { get; set; }
        Color ItemForeColor { get; set; }
        bool IsShowItem { get; set; }
    }
}
