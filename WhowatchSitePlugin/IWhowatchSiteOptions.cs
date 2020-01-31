using System.ComponentModel;
using System.Windows.Media;

namespace WhowatchSitePlugin
{
    public interface IWhowatchSiteOptions : INotifyPropertyChanged
    {
        bool NeedAutoSubNickname { get; set; }
        int LiveCheckIntervalSec { get; set; }
        int LiveDataRetrieveIntervalSec { get; set; }
        string Serialize();
        void Deserialize(string s);
        IWhowatchSiteOptions Clone();
        void Set(IWhowatchSiteOptions siteOptions);
    }
}
