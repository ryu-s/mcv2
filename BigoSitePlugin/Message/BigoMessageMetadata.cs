using SitePlugin;
using System.Windows.Media;
using System.Windows;
using System;
using System.Collections.Generic;

namespace BigoSitePlugin
{
    internal class BigoMessageMetadata2 : IMessageMetadata2
    {
        private readonly IBigoMessage _message;
        private readonly IBigoSiteOptions _siteOptions;

        public bool IsNgUser => false;
        public bool IsSiteNgUser => false;//TODO:IUserにIsSiteNgUserを追加する
        public bool IsFirstComment { get; }
        public string SiteName { get; }
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
        public string UserId { get; }
        public IEnumerable<IMessagePart> UserName { get; }
        public SiteType SiteType => SiteType.Bigo;

        public BigoMessageMetadata2(IBigoMessage message, IBigoSiteOptions siteOptions, bool isFirstComment)
        {
            _message = message;
            _siteOptions = siteOptions;
            IsFirstComment = isFirstComment;
        }
        #region INotifyPropertyChanged
        [NonSerialized]
        private System.ComponentModel.PropertyChangedEventHandler _propertyChanged;
        /// <summary>
        /// 
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
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
