using System;

namespace mcv2
{
    public interface IPlugin2
    {
        PluginId Id { get; }
        IPluginHost Host { get; set; }
        string Name { get; }
        [Obsolete("Notifyでやるべきでは？")]
        void OnLoaded();
        [Obsolete("Notifyでやるべきでは？")]
        void OnClosing();
        void SetResponse(IResponse res);
        void SetNotify(INotify notify);
        void ShowSettingView();
    }
}
