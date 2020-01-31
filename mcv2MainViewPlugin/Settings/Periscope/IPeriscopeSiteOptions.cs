using System;
using System.ComponentModel;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    interface IPeriscopeSiteOptions
    {
        Color ItemCommentBackColor { get; set; }
        Color ItemCommentForeColor { get; set; }
        bool IsAutoSetNickname { get; set; }
        bool IsShowJoinMessage { get; set; }
        bool IsShowLeaveMessage { get; set; }
    }
}
