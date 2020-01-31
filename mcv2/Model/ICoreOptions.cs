namespace mcv2.Model
{
    public interface ICoreOptions
    {
        string PluginDirPath { get; set; }
        string SettingsDirPath { get; set; }
        bool IsAutoUpdateCheck { get; set; }
        double MainViewPositionX { get; set; }
        double MainViewPositionY { get; set; }
    }


}
