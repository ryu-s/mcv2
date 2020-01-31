using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace mcv2
{
    public interface IPluginHost
    {
        string LoadOptions(PluginId pluginId, string filename);
        void SaveOptions(PluginId pluginId, string filename, string optionsStr);
        [Obsolete]
        string LoadOptions(string path);
        string SettingsDirPath { get; }
        void SetRequest(PluginId id, IRequest req);
        Task SetRequestAsync(PluginId pluginId, IRequestAsync req);
        IResponse GetData(IRequest req);
        Task<IResponse> GetDataAsync(IRequestAsync req);
        [Obsolete]
        void SaveOptions(string path, string optionsStr);
        void SetError(Exception ex);
        void SetError(string msg, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingFileLineNumber = 0);
    }
}
