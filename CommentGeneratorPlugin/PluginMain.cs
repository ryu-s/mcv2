using Common;
using mcv2;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace CommentGeneratorPlugin
{
    interface IOptions : INotifyPropertyChanged
    {
        bool IsEnabled { get; set; }
        string HcgSettingFilePath { get; set; }
        bool IsMirrativeJoin { get; set; }
        void Deserialize(string s);
    }
    class Options : DynamicOptionsBase, IOptions
    {
        public bool IsEnabled { get => GetValue(); set => SetValue(value); }
        public string HcgSettingFilePath { get => GetValue(); set => SetValue(value); }
        public bool IsMirrativeJoin { get => GetValue(); set => SetValue(value); }


        protected override void Init()
        {
            Dict.Add(nameof(IsEnabled), new Item { DefaultValue = false, Predicate = b => true, Serializer = b => b.ToString(), Deserializer = s => bool.Parse(s) });
            Dict.Add(nameof(HcgSettingFilePath), new Item { DefaultValue = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "hcg", "setting.xml"), Predicate = path => System.IO.File.Exists(path), Serializer = s => s, Deserializer = s => s });
            Dict.Add(nameof(IsMirrativeJoin), new Item { DefaultValue = false, Predicate = b => true, Serializer = b => b.ToString(), Deserializer = s => bool.Parse(s) });
        }
    }
    class Data
    {
        public string Comment { get; internal set; }
        public string SiteName { get; internal set; }
        public string Nickname { get; internal set; }
    }
    [Export(typeof(IMcvPluginV1))]
    public class PluginMain : IMcvPluginV1, IDisposable
    {
        private System.Timers.Timer _writeTimer;
        private System.Timers.Timer _deleteTimer;
        private SynchronizedCollection<Data> _commentCollection = new SynchronizedCollection<Data>();
        protected virtual string CommentXmlPath { get; private set; }
        private bool _disposedValue;

        private IOptions Options { get; } = new Options();

        public PluginId Id { get; } = new PluginId();
        public IPluginHost Host { get; set; }
        public string Name { get; } = "コメジェネ連携";
        private static readonly object _lockObj = new object();
        void IMcvPluginV1.OnClosing()
        {
            _settingsView?.ForceClose();
            _writeTimer?.Stop();
            _deleteTimer?.Stop();
            //if (Options != null)
            //{
            //    Options.Save(Options, GetSettingsFilePath());
            //}
        }
        private readonly object _xmlWriteLockObj = new object();
        /// <summary>
        /// XMLファイルに書き込む
        /// </summary>
        /// <param name="xmlRootElement"></param>
        /// <param name="path"></param>
        private void WriteXml(XElement xmlRootElement, string path)
        {
            lock (_xmlWriteLockObj)
            {
                xmlRootElement.Save(path);
            }
        }
        private void _deleteTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Options.IsEnabled)
                return;

            //comment.xmlの要素を定期的に削除する
            XElement xml;
            try
            {
                if (!File.Exists(CommentXmlPath))
                    return;
                xml = XElement.Load(CommentXmlPath);
                var arr = xml.Elements().ToArray();
                var count = arr.Length;
                if (count > 1000)
                {
                    //1000件以上だったら、最後の100件以外を全て削除
                    xml.RemoveAll();
                    for (int i = count - 100; i < count; i++)
                    {
                        xml.Add(arr[i]);
                    }
                    WriteXml(xml, CommentXmlPath);
                }
            }
            catch (IOException ex)
            {
                //being used in another process
                Debug.WriteLine(ex.Message);
                return;
            }
        }

        protected virtual string GetHcgPath(string hcgSettingsFilePath)
        {
            string settingXml;
            using (var sr = new StreamReader(hcgSettingsFilePath))
            {
                settingXml = sr.ReadToEnd();
            }
            var xmlParser = DynamicXmlParser.Parse(settingXml);
            if (!xmlParser.HasElement("HcgPath"))
                return "";
            return xmlParser.HcgPath;
        }
        private void _writeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //定期的にcomment.xmlに書き込む。

            Write();
        }
        protected virtual bool IsHcgSettingFileExists()
        {
            return File.Exists(Options.HcgSettingFilePath);
        }
        private static bool HasNickname(IMcvUser? user)
        {
            return user != null && !string.IsNullOrEmpty(user.Nickname);
        }
        /// <summary>
        /// _commentCollectionの内容をファイルに書き出す
        /// </summary>
        public void Write()
        {
            if (!Options.IsEnabled || _commentCollection.Count == 0)
                return;

            //TODO:各ファイルが存在しなかった時のエラー表示
            if (string.IsNullOrEmpty(CommentXmlPath) && IsHcgSettingFileExists())
            {
                var hcgPath = GetHcgPath(Options.HcgSettingFilePath);
                CommentXmlPath = hcgPath + "\\comment.xml";
                //TODO:パスがxmlファイルで無かった場合の対応。ディレクトリの可能性も。
            }
            if (!File.Exists(CommentXmlPath))
            {
                var doc = new XmlDocument();
                var root = doc.CreateElement("log");

                doc.AppendChild(root);
                doc.Save(CommentXmlPath);
            }
            XElement xml;
            try
            {
                xml = XElement.Load(CommentXmlPath);
            }
            catch (IOException ex)
            {
                //being used in another process
                Debug.WriteLine(ex.Message);
                return;
            }
            catch (XmlException)
            {
                //Root element is missing.
                xml = new XElement("log");
            }
            lock (_lockObj)
            {
                var arr = _commentCollection.ToArray();

                foreach (var data in arr)
                {
                    var item = new XElement("comment", data.Comment);
                    item.SetAttributeValue("no", "0");
                    item.SetAttributeValue("time", Tools.ToUnixTime(GetCurrentDateTime()));
                    item.SetAttributeValue("owner", 0);
                    item.SetAttributeValue("service", data.SiteName);
                    //2019/08/25 コメジェネの仕様で、handleタグが無いと"0コメ"に置換されてしまう。だから空欄でも良いからhandleタグは必須。
                    item.SetAttributeValue("handle", data.Nickname);
                    xml.Add(item);
                }
                try
                {
                    WriteXml(xml, CommentXmlPath);
                }
                catch (IOException ex)
                {
                    //コメントの流れが早すぎるとused in another processが来てしまう。
                    //この場合、コメントが書き込まれずに消されてしまう。
                    Debug.WriteLine(ex.Message);
                    return;
                }
                _commentCollection.Clear();
            }
        }
        protected virtual DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
        private void SetOptions(string s)
        {
            Options.Deserialize(s);
        }
        void IMcvPluginV1.OnLoaded()
        {
            SetOptions(Host.LoadOptions(Id, GetSettingsFilePath()));

            _writeTimer = new System.Timers.Timer
            {
                Interval = 500
            };
            _writeTimer.Elapsed += _writeTimer_Elapsed;
            _writeTimer.Start();

            _deleteTimer = new System.Timers.Timer
            {
                Interval = 5 * 60 * 1000
            };
            _deleteTimer.Elapsed += _deleteTimer_Elapsed;
            _deleteTimer.Start();
        }

        void IMcvPluginV1.SetNotify(INotify notify)
        {
            switch (notify)
            {
                case NotifyMessageReceived msgReceived:
                    OnMessageReceived(msgReceived.Message, msgReceived.Metadata, msgReceived.User);
                    break;
                default:
                    break;
            }
        }
        void OnMessageReceived(ISiteMessage message, IMessageMetadata2 metadata, IMcvUser? user)
        {
            if (!Options.IsEnabled || metadata.IsNgUser || metadata.IsInitialComment)
            {
                return;
            }
            if (message is MirrativSitePlugin.IMirrativJoinRoom && !Options.IsMirrativeJoin)
            {
                return;
            }
            var serviceName = GetServiceName(metadata.SiteType);
            var (name, comment) = Tools.GetData(message);
            if (comment == null)
            {
                return;
            }
            if (HasNickname(user))
            {
                name = user!.Nickname;
            }
            var data = new Data
            {
                Comment = comment,
                Nickname = name ?? "",
                SiteName = serviceName,
            };
            _commentCollection.Add(data);
        }
        private string GetServiceName(SiteType siteType)
        {
            string ret;
            switch (siteType)
            {
                case SiteType.Unknown:
                    ret = "";
                    break;
                case SiteType.NicoLive:
                    ret = "nicolive";
                    break;
                case SiteType.YouTubeLive:
                    ret = "youtubelive";
                    break;
                case SiteType.Openrec:
                    ret = "openrec";
                    break;
                case SiteType.Whowatch:
                    ret = "whowatch";
                    break;
                case SiteType.LineLive:
                    ret = "linelive";
                    break;
                case SiteType.Mirrativ:
                    ret = "mirrativ";
                    break;
                case SiteType.Twicas:
                    ret = "twicas";
                    break;
                case SiteType.Twitch:
                    ret = "twitch";
                    break;
                case SiteType.Periscope:
                    ret = "periscope";
                    break;
                case SiteType.ShowRoom:
                    ret = "showroom";
                    break;
                case SiteType.Mildom:
                    ret = "mildom";
                    break;
                case SiteType.Bigo:
                    ret = "bigo";
                    break;
                default:
                    ret = "";
                    break;
            }
            return ret;
        }

        void IMcvPluginV1.SetResponse(IResponse res)
        {
            switch (res)
            {
                default:
                    break;
            }
        }
        SettingsView _settingsView;
        void IMcvPluginV1.ShowSettingView()
        {
            if (_settingsView == null)
            {
                _settingsView = new SettingsView
                {
                    DataContext = new ConfigViewModel(Options)
                };
            }
            if (Host.GetData(new RequestMainViewPosition()) is ResponseMainViewPosition resPos)
            {
                _settingsView.Topmost = false;
                _settingsView.Left = resPos.X;
                _settingsView.Top = resPos.Y;

                _settingsView.Show();
            }
            else
            {
                //バグ報告
            }
        }
        public string GetSettingsFilePath()
        {
            var dir = Host.SettingsDirPath;
            return Path.Combine(dir, $"{Name}.xml");
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _writeTimer.Dispose();
                    _deleteTimer.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
