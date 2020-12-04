using SitePlugin;
using System.Windows.Media;
using System.Windows;
using System;
using System.Collections.Generic;

namespace YouTubeLiveSitePlugin
{
    internal class YouTubeLiveMessageMetadata2 : IMessageMetadata2
    {
        private readonly IYouTubeLiveMessage _message;
        private readonly IYouTubeLiveSiteOptions _siteOptions;

        public bool IsNgUser => false;
        public bool IsSiteNgUser => false;//TODO:IUserにIsSiteNgUserを追加する
        public bool IsFirstComment { get; }
        public string? SiteName { get; }
        public bool Is184 { get; }
        public bool IsVisible
        {
            get
            {
                if (IsNgUser || IsSiteNgUser) return false;

                //TODO:ConnectedとかDisconnectedの場合、表示するエラーレベルがError以下の場合にfalseにしたい
                return true;
            }
        }
        public bool IsInitialComment { get; set; }
        public SitePluginId SiteContextGuid { get; set; }
        public string UserId { get; set; }
        public IEnumerable<IMessagePart> UserName { get; set; }
        public SiteType SiteType => SiteType.YouTubeLive;

        public string? NewNickname { get; set; }

        public YouTubeLiveMessageMetadata2(IYouTubeLiveMessage message, IYouTubeLiveSiteOptions siteOptions, bool isFirstComment)
        {
            _message = message;
            _siteOptions = siteOptions;
            IsFirstComment = isFirstComment;
        }
        #region INotifyPropertyChanged
        [NonSerialized]
        private System.ComponentModel.PropertyChangedEventHandler? _propertyChanged;
        /// <summary>
        /// 
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            _propertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
