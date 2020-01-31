namespace mcv2.MainViewPlugin
{
    class TwitchSiteOptionsCopy : OptionsCopyBase<ITwitchSiteOptions>, ITwitchSiteOptions
    {
        public bool NeedAutoSubNickname { get; set; }
        public string NeedAutoSubNicknameStr { get; set; }
    }
}
