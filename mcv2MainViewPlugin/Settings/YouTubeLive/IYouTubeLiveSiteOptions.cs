using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    interface IYouTubeLiveSiteOptions
    {
        Color PaidCommentBackColor { get; set; }
        Color PaidCommentForeColor { get; set; }
        bool IsAutoSetNickname { get; set; }
        bool IsAllChat { get; set; }
    }
}
