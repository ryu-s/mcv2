using System.ComponentModel;

namespace mcv2.MainViewPlugin
{
    interface ITwitchSiteOptions
    {
        bool NeedAutoSubNickname { get; set; }
        string NeedAutoSubNicknameStr { get; set; }
    }
}
