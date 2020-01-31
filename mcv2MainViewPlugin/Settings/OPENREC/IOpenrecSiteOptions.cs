using System.ComponentModel;

namespace mcv2.MainViewPlugin
{
    public interface IOpenrecSiteOptions : INotifyPropertyChanged
    {
        int StampSize { get; set; }
        bool IsPlayStampMusic { get; set; }
        string StampMusicFilePath { get; set; }
        bool IsPlayYellMusic { get; set; }
        string YellMusicFilePath { get; set; }
        bool IsAutoSetNickname { get; set; }
    }
}
