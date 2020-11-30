using Common;
using mcv2.Model;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace mcv2
{
    class Program
    {
        //static readonly ILogger Logger = new Common.LoggerTest();
        static readonly ILogger Logger2 = new LoggerTest2();
        ICoreOptions _coreOptions = new CoreOptions();
        static readonly string optionsFilePath = System.IO.Path.Combine("settings", "core.txt");
        [STAThread]
        //static async Task Main(string[] args)
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var app = new AppNoStartupUri
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };
            app.InitializeComponent();
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            var p = new Program();
            var t = p.StartAsync();
            Handle(t);
            app.Run();
        }
        static async void Handle(Task t)
        {
            try
            {
                await Task.Yield();
                await t;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger2.LogException(ex);
            }
            Application.Current.Shutdown();
        }
        public async Task StartAsync()
        {
            var pluginManager = new Model.PluginManager();
            var io = new IOTest();
            var sitePluginManager = new Model.SitePluginManager(Logger2, io);
            var optionsStr = await io.ReadFileAsync(optionsFilePath);
            if (optionsStr != null)
            {
                _coreOptions.Deserialize(optionsStr);
            }
            Model.IUserStore userStore = new SqliteUserStore(System.IO.Path.Combine("settings", "coreusers.db"), Logger2);
            await userStore.InitAsync();
            var model = new Model.Model(pluginManager, sitePluginManager, userStore, Logger2, io, _coreOptions);
            try
            {
                await model.Run();
            }
            finally
            {
                var s = _coreOptions.Serialize();
                await io.WriteFileAsync(optionsFilePath, s);

                await userStore.SaveAsync();
            }
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            try
            {
                Logger2.LogException(ex, "UnhandledException");
                var s = LoggerTest2.Parse(Logger2.GetLogData());
                using (var sw = new System.IO.StreamWriter("error.txt", true))
                {
                    sw.WriteLine(s);
                }
            }
            catch { }
        }
        private string GetTitle()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var title = asm.GetName().Name;
            return title;
        }
        private string GetVersion()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version;
            var s = $"v{ver.Major}.{ver.Minor}.{ver.Build}";
            return s;
        }
        /// <summary>
        /// エラー情報をサーバに送信する
        /// </summary>
        /// <param name="errorData"></param>
        /// <param name="title"></param>
        /// <param name="version"></param>
        private void SendErrorReport(string errorData, string title, string version)
        {
            if (string.IsNullOrEmpty(errorData))
            {
                return;
            }
            var fileStreamContent = new StreamContent(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(errorData)));
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                client.DefaultRequestHeaders.Add("User-Agent", $"{title} {version}");
                formData.Add(fileStreamContent, "error", title + "_" + version + "_" + "error.txt");
                var t = client.PostAsync("http://int-main.net/upload", formData);
                var response = t.Result;
                if (!response.IsSuccessStatusCode)
                {
                }
                else
                {
                }
            }
        }
        /// <summary>
        /// error.txtがあったらサーバに送信して削除する
        /// </summary>
        private void SendErrorFile()
        {
            if (System.IO.File.Exists("error.txt"))
            {
                string errorContent;
                using (var sr = new System.IO.StreamReader("error.txt"))
                {
                    errorContent = sr.ReadToEnd();
                }
                SendErrorReport(errorContent, GetTitle(), GetVersion());
                System.IO.File.Delete("error.txt");
            }
        }
        //private static string GetSiteOptionsPath(IOptions options, string displayName)
        //{
        //    var path = System.IO.Path.Combine(options.SettingsDirPath, displayName + ".txt");
        //    return path;
        //}
    }
}