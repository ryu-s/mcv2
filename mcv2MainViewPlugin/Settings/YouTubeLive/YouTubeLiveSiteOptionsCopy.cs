using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class YouTubeLiveSiteOptionsCopy : OptionsCopyBase<IYouTubeLiveSiteOptions>, IYouTubeLiveSiteOptions
    {
        public Color PaidCommentBackColor { get; set; }
        public Color PaidCommentForeColor { get; set; }
        public bool IsAutoSetNickname { get; set; }
        public bool IsAllChat { get; set; }
    }
}
