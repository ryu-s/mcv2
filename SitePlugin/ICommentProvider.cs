using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ryu_s.BrowserCookie;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Net;

namespace SitePlugin
{
    public interface ICommentProvider
    {
        event EventHandler<ConnectedEventArgs> Connected;
        event EventHandler<IMessageContext2> MessageReceived;

        //event EventHandler<List<ICommentViewModel>> PastCommentsReceived;
        event EventHandler<IMetadata> MetadataUpdated;
        //Task PostCommentAsync(string text);
        Task ConnectAsync(string input, IBrowserProfile2? browserProfile);
        void Disconnect();
        //IEnumerable<ICommentViewModel> GetUserComments(IUser user);
        bool CanConnect { get; }
        bool CanDisconnect { get; }
        event EventHandler CanConnectChanged;
        event EventHandler CanDisconnectChanged;
        //TODO:どのアカウントでログインしているのかConnectionViewに表示したい
        //Task<IMyInfo> GetMyInfo(IBrowserProfile browserProfile);
        //IUser GetUser(string userId);

        Task PostCommentAsync(string text);

        //bool IsLoggedIn { get; }

        //bool IsConnected { get; }
        Task<ICurrentUserInfo> GetCurrentUserInfo(IBrowserProfile2? browserProfile);
        SitePluginId SiteContextGuid { get; }
    }
    public class ConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// 入力値の保存が必要か
        /// YouTubeLiveの場合であれば、放送URLはfalse,channelURLはtrue
        /// </summary>
        public bool IsInputStoringNeeded { get; set; }
        /// <summary>
        /// 次回起動時にリストアするURL
        /// </summary>
        public string? UrlToRestore { get; set; }
    }
}
