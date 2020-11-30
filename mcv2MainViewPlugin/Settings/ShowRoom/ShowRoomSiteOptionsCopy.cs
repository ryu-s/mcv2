using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class ShowRoomSiteOptionsCopy : OptionsCopyBase<IShowRoomSiteOptions>, IShowRoomSiteOptions
    {
        public Color ItemCommentBackColor { get; set; }
        public Color ItemCommentForeColor { get; set; }
        public bool IsAutoSetNickname { get; set; }
        public bool IsShowJoinMessage { get; set; }
        public bool IsShowLeaveMessage { get; set; }
        public bool IsIgnore50Counts { get; set; }
    }
}
