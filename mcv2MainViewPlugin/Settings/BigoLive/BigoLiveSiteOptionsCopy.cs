using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class BigoLiveSiteOptionsCopy : OptionsCopyBase<IBigoSiteOptions>, IBigoSiteOptions
    {
        public bool IsAutoSetNickname { get; set; }
    }
}
