using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class TwicasSiteOptionsCopy : OptionsCopyBase<ITwicasSiteOptions>, ITwicasSiteOptions
    {
        public int CommentRetrieveIntervalSec { get; set; }
        public Color KiitosBackColor { get; set; }
        public Color KiitosForeColor { get; set; }
        public bool IsAutoSetNickname { get; set; }
        public Color ItemBackColor { get; set; }
        public Color ItemForeColor { get; set; }
        public bool IsShowItem { get; set; }
    }
}
