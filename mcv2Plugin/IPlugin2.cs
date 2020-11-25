using System;

namespace mcv2
{
    public interface IMcvPluginV1
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
