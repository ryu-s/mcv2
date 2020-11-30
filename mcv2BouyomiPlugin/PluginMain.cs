using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Common.Wpf;
using LineLiveSitePlugin;
using mcv2;
using MildomSitePlugin;
using MirrativSitePlugin;
using NicoSitePlugin;
using OpenrecSitePlugin;
using PeriscopeSitePlugin;
using SitePlugin;
using TwicasSitePlugin;
using TwitchSitePlugin;
using WhowatchSitePlugin;
using YouTubeLiveSitePlugin;

namespace mcv2BouyomiPlugin
{
    /// <summary>
    /// 棒読みちゃんが起動していない可能性がある
    /// </summary>
    [Serializable]
    public class BouyomiChanNotRunException : Exception
    {
        public BouyomiChanNotRunException() { }
        protected BouyomiChanNotRunException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    static class MessageParts
    {
        public static string ToTextWithImageAlt(this IEnumerable<IMessagePart> parts)
        {
            string s = "";
            if (parts != null)
            {
                foreach (var part in parts)
                {
                    if (part is IMessageText text)
                    {
                        s += text;
                    }
                    else if (part is IMessageImage image)
                    {
                        s += image.Alt;
                    }
                }
            }
            return s;
        }
    }
    public interface IBouyomiChanClient
    {
        int AddTalkTask2(string text, Int16 voiceSpeed, Int16 voiceTone, Int16 voiceVolume, Int16 voiceTypeIndex);
        int AddTalkTask2(string text);
        void SetSettings(IBouyomiChanClientSettings settings);
    }
    public interface IBouyomiChanClientSettings { }
    public class BouyomiChanClientTcp : IBouyomiChanClient
    {
        public string HostAddr { get; set; } = "";
        public int HostPort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="voiceSpeed"></param>
        /// <param name="voiceTone"></param>
        /// <param name="voiceVolume"></param>
        /// <param name="voiceTypeIndex"></param>
        /// <returns></returns>
        /// <exception cref=""
        public int AddTalkTask2(string text, Int16 voiceSpeed, Int16 voiceTone, Int16 voiceVolume, Int16 voiceTypeIndex)
        {
            string sMessage = text;
            byte bCode = 0;
            Int16 iVoice = voiceTypeIndex;
            Int16 iVolume = voiceVolume;
            Int16 iSpeed = voiceSpeed;
            Int16 iTone = voiceTone;
            Int16 iCommand = 0x0001;
            byte[] bMessage = Encoding.UTF8.GetBytes(sMessage);
            Int32 iLength = bMessage.Length;

            try
            {
                using (var tc = new TcpClient(HostAddr, HostPort))
                using (var ns = tc.GetStream())
                using (var bw = new BinaryWriter(ns))
                {
                    bw.Write(iCommand); //コマンド（ 0:メッセージ読み上げ）
                    bw.Write(iSpeed);   //速度    （-1:棒読みちゃん画面上の設定）
                    bw.Write(iTone);    //音程    （-1:棒読みちゃん画面上の設定）
                    bw.Write(iVolume);  //音量    （-1:棒読みちゃん画面上の設定）
                    bw.Write(iVoice);   //声質    （ 0:棒読みちゃん画面上の設定、1:女性1、2:女性2、3:男性1、4:男性2、5:中性、6:ロボット、7:機械1、8:機械2、10001～:SAPI5）
                    bw.Write(bCode);    //文字列のbyte配列の文字コード(0:UTF-8, 1:Unicode, 2:Shift-JIS)
                    bw.Write(iLength);  //文字列のbyte配列の長さ
                    bw.Write(bMessage); //文字列のbyte配列
                }
            }
            catch (Exception ex)
            {
                throw new BouyomiChanNotRunException();
            }
            return 0;
        }

        public int AddTalkTask2(string text)
        {
            return AddTalkTask2(text, -1, -1, -1, 0);
        }

        public void SetSettings(IBouyomiChanClientSettings settings)
        {
            if (!(settings is BouyomiChanClientSettingsTcp tcp)) return;
            if (tcp.HostAddr != null)
            {
                HostAddr = tcp.HostAddr;
            }
            if (tcp.HostPort != null)
            {
                HostPort = tcp.HostPort.Value;
            }
        }
    }
    class BouyomiChanClientSettingsTcp : IBouyomiChanClientSettings
    {
        public string? HostAddr { get; set; }
        public int? HostPort { get; set; }
    }
    [Export(typeof(IMcvPluginV1))]
    public class PluginMain : IMcvPluginV1
    {
        public PluginId Id { get; } = new PluginId();
        public string Name { get; } = "棒読みちゃん連携";
        public IPluginHost Host { get; set; }
        private readonly Options _options = new Options();
        //private readonly FNF.Utility.BouyomiChanClient _bouyomiChanClient = new FNF.Utility.BouyomiChanClient();
        private readonly IBouyomiChanClient _bouyomiChanClient;
        Process? _bouyomiChanProcess;
        public PluginMain()
        {
            _bouyomiChanClient = new BouyomiChanClientTcp
            {
                HostAddr = _options.HostAddr,
                HostPort = _options.HostPort,
            };
            _options.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Options.HostAddr):
                        _bouyomiChanClient.SetSettings(new BouyomiChanClientSettingsTcp { HostAddr = _options.HostAddr });
                        break;
                    case nameof(Options.HostPort):
                        _bouyomiChanClient.SetSettings(new BouyomiChanClientSettingsTcp { HostPort = _options.HostPort });
                        break;
                    default:
                        break;
                }
            };
        }
        public void OnClosing()
        {
        }
        public string GetSettingsFilePath()
        {
            //ここでRemotingExceptionが発生。終了時の処理だが、既にHostがDisposeされてるのかも。
            var dir = Host.SettingsDirPath;
            return System.IO.Path.Combine(dir, $"{Name}.xml");
        }
        public void OnLoaded()
        {
            System.Diagnostics.Debug.WriteLine($"mcv2BouyomiPlugin::PluginMain::OnLoaded()");
            try
            {
                var s = Host.LoadOptions(GetSettingsFilePath());
                _options.Deserialize(s);
            }
            catch (System.IO.FileNotFoundException) { }
            try
            {
                if (_options.IsExecBouyomiChanAtBoot && !IsExecutingProcess("BouyomiChan"))
                {
                    StartBouyomiChan();
                }
            }
            catch (Exception) { }
        }
        private void StartBouyomiChan()
        {
            if (_bouyomiChanProcess == null && System.IO.File.Exists(_options.BouyomiChanPath))
            {
                _bouyomiChanProcess = Process.Start(_options.BouyomiChanPath);
                _bouyomiChanProcess.EnableRaisingEvents = true;
                _bouyomiChanProcess.Exited += BouyomiChanProcess_Exited;
            }
        }
        private void BouyomiChanProcess_Exited(object sender, EventArgs e)
        {
            try
            {
                _bouyomiChanProcess?.Close();//2018/03/25ここで_bouyomiChanProcessがnullになる場合があった
            }
            catch { }
            _bouyomiChanProcess = null;
        }
        public void SetNotify(INotify notify)
        {
            switch (notify)
            {
                case NotifyMessageReceived msgReceived:
                    OnMessageReceived(msgReceived.Message, msgReceived.Metadata, msgReceived.User);
                    break;
            }
        }
        private void OnMessageReceived(ISiteMessage message, IMessageMetadata2 messageMetadata, IMcvUser user)
        {
            if (!_options.IsEnabled || messageMetadata.IsNgUser || messageMetadata.IsInitialComment || (messageMetadata.Is184 && !_options.Want184Read))
                return;

            var (name, comment) = GetData(message, _options);

            //nameがnullでは無い場合かつUser.Nicknameがある場合はNicknameを採用
            if (!string.IsNullOrEmpty(name) && user != null && !string.IsNullOrEmpty(user.Nickname))
            {
                name = user.Nickname;
            }
            try
            {
                //棒読みちゃんが事前に起動されていたらそれを使いたい。
                //起動していなかったら起動させて、準備ができ次第それ以降のコメントを読んで貰う

                //とりあえず何も確認せずにコメントを送信する。起動していなかったら例外が起きる。

                var dataToRead = "";//棒読みちゃんに読んでもらう文字列
                if (_options.IsReadHandleName && !string.IsNullOrEmpty(name))
                {
                    dataToRead += name;

                    if (_options.IsAppendNickTitle)
                    {
                        dataToRead += _options.NickTitle;
                    }
                }
                if (_options.IsReadComment && !string.IsNullOrEmpty(comment))
                {
                    if (!string.IsNullOrEmpty(dataToRead))//空欄で無い場合、つまり名前も読み上げる場合は名前とコメントの間にスペースを入れる。こうすると名前とコメントの間で一呼吸入れてくれる
                    {
                        dataToRead += " ";
                    }
                    dataToRead += comment;
                }
                if (string.IsNullOrEmpty(dataToRead))
                {
                    return;
                }
                TalkText(dataToRead);
            }
            catch (BouyomiChanNotRunException)
            {
                //多分棒読みちゃんが起動していない。
                if (_bouyomiChanProcess == null && System.IO.File.Exists(_options.BouyomiChanPath))
                {
                    _bouyomiChanProcess = Process.Start(_options.BouyomiChanPath);
                    _bouyomiChanProcess.EnableRaisingEvents = true;
                    _bouyomiChanProcess.Exited += (s, e) =>
                    {
                        try
                        {
                            _bouyomiChanProcess?.Close();//2018/03/25ここで_bouyomiChanProcessがnullになる場合があった
                        }
                        catch { }
                        _bouyomiChanProcess = null;
                    };
                }
                //起動するまでの間にコメントが投稿されたらここに来てしまうが諦める。
            }
            catch (Exception ex)
            {

            }
        }
        private Int16 Convert(int n, Int16 min, Int16 max, Int16 defaultValue)
        {
            if (n >= min && n <= max) return (Int16)n;
            else return defaultValue;
        }
        private Int16 VoiceTypeSelectedIndex2VoiceTypeId(int voiceTypeIndex)
        {
            //0:棒読みちゃん画面上の設定、1:女性1、2:女性2、3:男性1、4:男性2、5:中性、6:ロボット、7:機械1、8:機械2、10001～:SAPI5
            const Int16 offset = 10001;
            if (voiceTypeIndex >= 0 && voiceTypeIndex <= 8)
            {
                return (Int16)voiceTypeIndex;
            }
            else if (voiceTypeIndex >= 9 && voiceTypeIndex <= (Int16.MaxValue - offset))
            {
                return (Int16)(voiceTypeIndex - 9 + offset);
            }
            else
            {
                return 0;
            }
        }
        private int TalkText(string text)
        {
            if (_options.IsVoiceTypeSpecfied)
            {
                return _bouyomiChanClient.AddTalkTask2(
                    text,
                    Convert(_options.VoiceSpeed, -1, Int16.MaxValue, -1),
                    Convert(_options.VoiceTone, -1, Int16.MaxValue, -1),
                    Convert(_options.VoiceVolume, -1, Int16.MaxValue, -1),
                    VoiceTypeSelectedIndex2VoiceTypeId(_options.VoiceTypeSelectedIndex)
                );
            }
            else
            {
                return _bouyomiChanClient.AddTalkTask2(text);
            }
        }
        public void SetResponse(IResponse res)
        {
        }

        /// <summary>
        /// 指定したプロセス名を持つプロセスが起動中か
        /// </summary>
        /// <param name="processName">プロセス名</param>
        /// <returns></returns>
        private bool IsExecutingProcess(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }
        private static (string name, string comment) GetData(ISiteMessage message, Options options)
        {
            string name = null;
            string comment = null;
            if (false) { }
            else if (message is IYouTubeLiveMessage youTubeLiveMessage)
            {
                switch (youTubeLiveMessage.YouTubeLiveMessageType)
                {
                    case YouTubeLiveMessageType.Connected:
                        if (options.IsYouTubeLiveConnect)
                        {
                            name = null;
                            comment = (youTubeLiveMessage as IYouTubeLiveConnected).Text;
                        }
                        break;
                    case YouTubeLiveMessageType.Disconnected:
                        if (options.IsYouTubeLiveDisconnect)
                        {
                            name = null;
                            comment = (youTubeLiveMessage as IYouTubeLiveDisconnected).Text;
                        }
                        break;
                    case YouTubeLiveMessageType.Comment:
                        if (options.IsYouTubeLiveComment)
                        {
                            if (options.IsYouTubeLiveCommentNickname)
                            {
                                name = (youTubeLiveMessage as IYouTubeLiveComment).NameItems.ToText();
                            }
                            if (options.IsYouTubeLiveCommentStamp)
                            {
                                comment = (youTubeLiveMessage as IYouTubeLiveComment).CommentItems.ToTextWithImageAlt();
                            }
                            else
                            {
                                comment = (youTubeLiveMessage as IYouTubeLiveComment).CommentItems.ToText();
                            }
                        }
                        break;
                    case YouTubeLiveMessageType.Superchat:
                        if (options.IsYouTubeLiveSuperchat)
                        {
                            if (options.IsYouTubeLiveSuperchatNickname)
                            {
                                name = (youTubeLiveMessage as IYouTubeLiveSuperchat).NameItems.ToText();
                            }
                            //TODO:superchat中のスタンプも読ませるべきでは？
                            comment = (youTubeLiveMessage as IYouTubeLiveSuperchat).CommentItems.ToText();
                        }
                        break;
                }
            }
            else if (message is IOpenrecMessage openrecMessage)
            {
                switch (openrecMessage.OpenrecMessageType)
                {
                    case OpenrecMessageType.Connected:
                        if (options.IsOpenrecConnect)
                        {
                            name = null;
                            comment = (openrecMessage as IOpenrecConnected).Text;
                        }
                        break;
                    case OpenrecMessageType.Disconnected:
                        if (options.IsOpenrecDisconnect)
                        {
                            name = null;
                            comment = (openrecMessage as IOpenrecDisconnected).Text;
                        }
                        break;
                    case OpenrecMessageType.Comment:
                        if (options.IsOpenrecComment)
                        {
                            if (options.IsOpenrecCommentNickname)
                            {
                                name = (openrecMessage as IOpenrecComment).NameItems.ToText();
                            }
                            comment = (openrecMessage as IOpenrecComment).MessageItems.ToText();
                        }
                        break;
                }
            }
            else if (message is ITwitchMessage twitchMessage)
            {
                switch (twitchMessage.TwitchMessageType)
                {
                    case TwitchMessageType.Connected:
                        if (options.IsTwitchConnect)
                        {
                            name = null;
                            comment = (twitchMessage as ITwitchConnected).Text;
                        }
                        break;
                    case TwitchMessageType.Disconnected:
                        if (options.IsTwitchDisconnect)
                        {
                            name = null;
                            comment = (twitchMessage as ITwitchDisconnected).Text;
                        }
                        break;
                    case TwitchMessageType.Comment:
                        if (options.IsTwitchComment)
                        {
                            if (options.IsTwitchCommentNickname)
                            {
                                name = (twitchMessage as ITwitchComment).DisplayName;
                            }
                            comment = (twitchMessage as ITwitchComment).CommentItems.ToText();
                        }
                        break;
                }
            }
            else if (message is INicoMessage NicoMessage)
            {
                switch (NicoMessage.NicoMessageType)
                {
                    case NicoMessageType.Connected:
                        if (options.IsNicoConnect)
                        {
                            name = null;
                            comment = (NicoMessage as INicoConnected).Text;
                        }
                        break;
                    case NicoMessageType.Disconnected:
                        if (options.IsNicoDisconnect)
                        {
                            name = null;
                            comment = (NicoMessage as INicoDisconnected).Text;
                        }
                        break;
                    case NicoMessageType.Comment:
                        if (options.IsNicoComment)
                        {
                            if (options.IsNicoCommentNickname)
                            {
                                name = (NicoMessage as INicoComment).UserName;
                            }
                            comment = (NicoMessage as INicoComment).Text;
                        }
                        break;
                    case NicoMessageType.Item:
                        if (options.IsNicoItem)
                        {
                            if (options.IsNicoItemNickname)
                            {
                                //name = (NicoMessage as INicoItem).NameItems.ToText();
                            }
                            comment = (NicoMessage as INicoItem).Text;
                        }
                        break;
                    case NicoMessageType.Ad:
                        if (options.IsNicoAd)
                        {
                            name = null;
                            comment = (NicoMessage as INicoAd).Text;
                        }
                        break;
                }
            }
            else if (message is ITwicasMessage twicasMessage)
            {
                switch (twicasMessage.TwicasMessageType)
                {
                    case TwicasMessageType.Connected:
                        if (options.IsTwicasConnect)
                        {
                            name = null;
                            comment = (twicasMessage as ITwicasConnected).Text;
                        }
                        break;
                    case TwicasMessageType.Disconnected:
                        if (options.IsTwicasDisconnect)
                        {
                            name = null;
                            comment = (twicasMessage as ITwicasDisconnected).Text;
                        }
                        break;
                    case TwicasMessageType.Comment:
                        if (options.IsTwicasComment)
                        {
                            if (options.IsTwicasCommentNickname)
                            {
                                name = (twicasMessage as ITwicasComment).UserName;
                            }
                            comment = (twicasMessage as ITwicasComment).CommentItems.ToText();
                        }
                        break;
                    case TwicasMessageType.Item:
                        if (options.IsTwicasItem)
                        {
                            if (options.IsTwicasItemNickname)
                            {
                                name = (twicasMessage as ITwicasItem).UserName;
                            }
                            comment = (twicasMessage as ITwicasItem).CommentItems.ToTextWithImageAlt();
                        }
                        break;
                }
            }
            else if (message is ILineLiveMessage lineLiveMessage)
            {
                switch (lineLiveMessage.LineLiveMessageType)
                {
                    case LineLiveMessageType.Connected:
                        if (options.IsLineLiveConnect)
                        {
                            name = null;
                            comment = (lineLiveMessage as ILineLiveConnected).Text;
                        }
                        break;
                    case LineLiveMessageType.Disconnected:
                        if (options.IsLineLiveDisconnect)
                        {
                            name = null;
                            comment = (lineLiveMessage as ILineLiveDisconnected).Text;
                        }
                        break;
                    case LineLiveMessageType.Comment:
                        if (options.IsLineLiveComment)
                        {
                            if (options.IsLineLiveCommentNickname)
                            {
                                name = (lineLiveMessage as ILineLiveComment).DisplayName;
                            }
                            comment = (lineLiveMessage as ILineLiveComment).Text;
                        }
                        break;
                }
            }
            else if (message is IWhowatchMessage whowatchMessage)
            {
                switch (whowatchMessage.WhowatchMessageType)
                {
                    case WhowatchMessageType.Connected:
                        if (options.IsWhowatchConnect)
                        {
                            name = null;
                            comment = (whowatchMessage as IWhowatchConnected).Text;
                        }
                        break;
                    case WhowatchMessageType.Disconnected:
                        if (options.IsWhowatchDisconnect)
                        {
                            name = null;
                            comment = (whowatchMessage as IWhowatchDisconnected).Text;
                        }
                        break;
                    case WhowatchMessageType.Comment:
                        if (options.IsWhowatchComment)
                        {
                            if (options.IsWhowatchCommentNickname)
                            {
                                name = (whowatchMessage as IWhowatchComment).UserName;
                            }
                            comment = (whowatchMessage as IWhowatchComment).Comment;
                        }
                        break;
                    case WhowatchMessageType.Item:
                        if (options.IsWhowatchItem)
                        {
                            if (options.IsWhowatchItemNickname)
                            {
                                name = (whowatchMessage as IWhowatchItem).UserName;
                            }
                            comment = (whowatchMessage as IWhowatchItem).Comment;
                        }
                        break;
                }
            }
            else if (message is IMirrativMessage mirrativMessage)
            {
                switch (mirrativMessage.MirrativMessageType)
                {
                    case MirrativMessageType.Connected:
                        if (options.IsMirrativConnect)
                        {
                            name = null;
                            comment = (mirrativMessage as IMirrativConnected).Text;
                        }
                        break;
                    case MirrativMessageType.Disconnected:
                        if (options.IsMirrativDisconnect)
                        {
                            name = null;
                            comment = (mirrativMessage as IMirrativDisconnected).Text;
                        }
                        break;
                    case MirrativMessageType.Comment:
                        if (options.IsMirrativComment)
                        {
                            if (options.IsMirrativCommentNickname)
                            {
                                name = (mirrativMessage as IMirrativComment).UserName;
                            }
                            comment = (mirrativMessage as IMirrativComment).Text;
                        }
                        break;
                    case MirrativMessageType.JoinRoom:
                        if (options.IsMirrativJoinRoom)
                        {
                            name = null;
                            comment = (mirrativMessage as IMirrativJoinRoom).Text;
                        }
                        break;
                    case MirrativMessageType.Item:
                        if (options.IsMirrativItem)
                        {
                            name = null;
                            comment = (mirrativMessage as IMirrativItem).Text;
                        }
                        break;
                }
            }
            else if (message is IPeriscopeMessage PeriscopeMessage)
            {
                switch (PeriscopeMessage.PeriscopeMessageType)
                {
                    case PeriscopeMessageType.Connected:
                        if (options.IsPeriscopeConnect)
                        {
                            name = null;
                            comment = (PeriscopeMessage as IPeriscopeConnected).Text;
                        }
                        break;
                    case PeriscopeMessageType.Disconnected:
                        if (options.IsPeriscopeDisconnect)
                        {
                            name = null;
                            comment = (PeriscopeMessage as IPeriscopeDisconnected).Text;
                        }
                        break;
                    case PeriscopeMessageType.Comment:
                        if (options.IsPeriscopeComment)
                        {
                            if (options.IsPeriscopeCommentNickname)
                            {
                                name = (PeriscopeMessage as IPeriscopeComment).DisplayName;
                            }
                            comment = (PeriscopeMessage as IPeriscopeComment).Text;
                        }
                        break;
                    case PeriscopeMessageType.Join:
                        if (options.IsPeriscopeJoin)
                        {
                            name = null;
                            comment = (PeriscopeMessage as IPeriscopeJoin).Text;
                        }
                        break;
                    case PeriscopeMessageType.Leave:
                        if (options.IsPeriscopeLeave)
                        {
                            name = null;
                            comment = (PeriscopeMessage as IPeriscopeLeave).Text;
                        }
                        break;
                }
            }
            else if (message is IMildomMessage MildomMessage)
            {
                switch (MildomMessage.MildomMessageType)
                {
                    case MildomMessageType.Connected:
                        if (options.IsMildomConnect)
                        {
                            name = null;
                            comment = (MildomMessage as IMildomConnected).Text;
                        }
                        break;
                    case MildomMessageType.Disconnected:
                        if (options.IsMildomDisconnect)
                        {
                            name = null;
                            comment = (MildomMessage as IMildomDisconnected).Text;
                        }
                        break;
                    case MildomMessageType.Comment:
                        if (options.IsMildomComment)
                        {
                            if (options.IsMildomCommentNickname)
                            {
                                name = (MildomMessage as IMildomComment).UserName;
                            }
                            comment = (MildomMessage as IMildomComment).CommentItems.ToText();
                        }
                        break;
                    case MildomMessageType.JoinRoom:
                        if (options.IsMildomJoin)
                        {
                            name = null;
                            comment = (MildomMessage as IMildomJoinRoom).CommentItems.ToText();
                        }
                        break;
                        //case MildomMessageType.Leave:
                        //    if (_options.IsMildomLeave)
                        //    {
                        //        name = null;
                        //        comment = (MildomMessage as IMildomLeave).CommentItems.ToText();
                        //    }
                        //    break;
                }
            }
            return (name, comment);
        }
        ConfigView _settingsView;
        public void ShowSettingView()
        {
            if (_settingsView == null)
            {
                _settingsView = new ConfigView
                {
                    DataContext = new ConfigViewModel(_options)
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
    }
}
