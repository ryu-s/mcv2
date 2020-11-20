using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace NicoSitePlugin
{
    internal interface INicoMessageMetadata2 : IMessageMetadata2
    {

    }
    internal abstract class MessageMetadataBase2 : INicoMessageMetadata2
    {
        protected readonly INicoSiteOptions _siteOptions;

        public virtual bool IsNgUser => false;
        public bool IsSiteNgUser => false;//TODO:IUserにIsSiteNgUserを追加する
        public virtual bool IsFirstComment => false;
        public string SiteName { get; }
        public bool Is184 { get; }
        public virtual bool IsVisible
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
        public SiteType SiteType => SiteType.NicoLive;

        protected MessageMetadataBase2(INicoSiteOptions siteOptions)
        {
            _siteOptions = siteOptions;
            siteOptions.PropertyChanged += SiteOptions_PropertyChanged;
        }


        protected void SiteOptions_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_siteOptions.IsAutoSetNickname):
                    break;
            }
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
    internal class CommentMessageMetadata2 : MessageMetadataBase2
    {
        public override bool IsNgUser => false;

        private readonly INicoComment _comment;
        private bool _isFirstComment;
        public override bool IsFirstComment => _isFirstComment;
        public override bool IsVisible => base.IsVisible;
        public CommentMessageMetadata2(INicoComment comment, INicoSiteOptions siteOptions, bool isFirstComment)
            : base(siteOptions)
        {
            _comment = comment;
            _isFirstComment = isFirstComment;
        }
    }
    internal class InfoMessageMetadata2 : MessageMetadataBase2
    {

        public InfoMessageMetadata2(INicoInfo info, INicoSiteOptions siteOptions)
            : base(siteOptions)
        {
        }
    }
    internal class ConnectedMessageMetadata2 : MessageMetadataBase2
    {
        private readonly INicoConnected _connected;

        public ConnectedMessageMetadata2(INicoConnected connected, INicoSiteOptions siteOptions)
            : base(siteOptions)
        {
            _connected = connected;
        }
    }
    internal class AdMessageMetadata2 : MessageMetadataBase2
    {
        private readonly INicoAd _ad;

        public AdMessageMetadata2(INicoAd ad, INicoSiteOptions siteOptions)
            : base(siteOptions)
        {
            _ad = ad;
        }
    }
    internal class ItemMessageMetadata2 : MessageMetadataBase2
    {
        private readonly INicoItem _item;

        public ItemMessageMetadata2(INicoItem item, INicoSiteOptions siteOptions)
            : base(siteOptions)
        {
            _item = item;
        }
    }
    internal class DisconnectedMessageMetadata2 : MessageMetadataBase2
    {

        public DisconnectedMessageMetadata2(INicoConnected comment, INicoSiteOptions siteOptions)
            : base(siteOptions)
        {
        }
    }
}
