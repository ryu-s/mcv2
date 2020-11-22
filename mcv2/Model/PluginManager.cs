using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace mcv2.Model
{
    class PluginInfo
    {
        public string Name { get; set; }
        public PluginId PluginId { get; set; }
    }
    public class PluginManagerExceptionEventArgs
    {
        public Exception Exception { get; set; }
    }
    public class PluginManager
    {
        public event EventHandler<IPlugin2> PluginAdded;
        //ここで発生した例外をCoreに通知する方法としてeventが一番疎結合を保てる気がする。
        public event EventHandler<PluginManagerExceptionEventArgs> ExceptionOccurred;
        //public event EventHandler<IPlugin> PluginRemoved;
        readonly List<IPlugin2> _plugins = new List<IPlugin2>();
        /// <summary>
        /// ディレクトリからプラグインを読み込む
        /// </summary>
        /// <param name="host"></param>
        /// <param name="dirPath"></param>
        public void LoadPluginFromDir(IPluginHost host, string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                //フォルダが存在しない旨通知する。
                //ここで問題が発生した場合
                return;
            }
            var pluginDirs = Directory.GetDirectories(dirPath);
            //var list = new List<DirectoryCatalog>();
            var def = new ImportDefinition(d => d.ContractName == typeof(IPlugin2).FullName, "", ImportCardinality.ExactlyOne, false, false);
            var plugins = new List<IPlugin2>();
            foreach (var pluginDir in pluginDirs)
            {
                var files = Directory.GetFiles(pluginDir).Where(s => s.EndsWith("Plugin.dll"));//ファイル名がPlugin.dllで終わるアセンブリだけ探す
                foreach (var file in files)
                {
                    var filename = Path.GetFileName(file);
                    if (filename == "SitePlugin.dll" || filename == "Plugin.dll") continue;
                    if (filename == "mcv2Plugin.dll") continue;
                    try
                    {
                        var catalog = new AssemblyCatalog(file);
                        if (catalog.Count() == 0)
                        {
                            //Exportが無い
                            continue;
                        }
                        var con = new CompositionContainer(catalog);
                        var plugin = con.GetExport<IPlugin2>()?.Value;
                        if (plugin == null) continue;
                        AddPlugin(host, plugin);
                    }
                    catch (Exception ex)
                    {
                        ExceptionOccurred?.Invoke(this, new PluginManagerExceptionEventArgs { Exception = ex });
                        Debug.WriteLine(ex.Message);
                    }
                }
                //list.Add(new DirectoryCatalog(pluginDir));
            }
            //_plugins = plugins;
            //foreach (var plugin in _plugins)
            //{
            //    plugin.Host = host;
            //    plugin.OnLoaded();
            //    PluginAdded?.Invoke(this, plugin);
            //}
        }

        internal PluginInfo GetPluginInfo(PluginId pluginId)
        {
            var plugin = _pluginDict[pluginId];
            return new PluginInfo
            {
                PluginId = pluginId,
                Name = plugin.Name,
            };
        }

        readonly Dictionary<PluginId, IPlugin2> _pluginDict = new Dictionary<PluginId, IPlugin2>();
        /// <summary>
        /// プラグインのインスタンスを直接追加する
        /// </summary>
        /// <param name="plugin"></param>
        public void AddPlugin(IPluginHost host, IPlugin2 plugin)
        {
            if (plugin.Id == null)
            {
                throw new ArgumentException("plugin.Idがnull");
            }
            plugin.Host = host;
            _plugins.Add(plugin);
            _pluginDict.Add(plugin.Id, plugin);
            plugin.OnLoaded();

            //読み込み済みのブラウザを共有する
            var resBrowser = host.GetData(new RequestBrowsers()) as ResponseBrowsers;
            foreach (var browser in resBrowser.BrowserProfiles)
            {
                plugin.SetNotify(new NotifyBrowserAdded
                {
                    BrowserId = browser.Id,
                    BrowserName = browser.Type.ToString(),
                    ProfileName = browser.ProfileName,
                });
            }
            //読み込み済みのサイトを共有する
            var resSites = host.GetData(new RequestSites()) as ResponseSites;
            if (resSites == null)
            {
                throw new BugException();
            }
            foreach (var (siteId, displayName) in resSites.Sites)
            {
                plugin.SetNotify(new NotifySiteAdded
                {
                    SiteId = siteId,
                    SiteName = displayName,
                });
            }

            PluginAdded?.Invoke(this, plugin);
        }

        internal void SetResponse(PluginId pluginId, IResponse res)
        {
            var plugin = GetPlugin(pluginId);
            plugin.SetResponse(res);

        }

        private IPlugin2 GetPlugin(PluginId pluginId)
        {
            return _plugins.Find(p => p.Id == pluginId);
        }

        internal void SetNotify(INotify notify)
        {
            foreach (var plugin in _plugins)
            {
                plugin.SetNotify(notify);
            }
        }

        internal List<LoadedPlugin> GetLoadedPlugins()
        {
            return _plugins.Select(x => new LoadedPlugin(x.Id, x.Name)).ToList();
        }
        /// <summary>
        /// 指定したプラグインの設定画面を開く
        /// </summary>
        /// <param name="targetPluginId"></param>
        internal void ShowPluginSettingView(PluginId targetPluginId)
        {
            if (_pluginDict.TryGetValue(targetPluginId, out var plugin))
            {
                plugin.ShowSettingView();
            }
        }

        internal void UnloadPlugins()
        {
            _plugins.Clear();
            _pluginDict.Clear();
        }
        ~PluginManager()
        {
            foreach (var plugin in _plugins)
            {
                if (plugin is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
