using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace MildomSitePlugin
{
    internal interface IMildomMessageMetadata2 : IMessageMetadata2
    {

    }
    internal abstract class MessageMetadataBase2 : IMildomMessageMetadata2
    {
        protected readonly IMildomSiteOptions _siteOptions;

        public virtual bool IsNgUser => false;
        public bool IsSiteNgUser => false;//TODO:IUserにIsSiteNgUserを追加する
        public virtual bool IsFirstComment => false;
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
        public SiteType SiteType => SiteType.Mildom;
        public string? NewNickname { get; set; }
        protected MessageMetadataBase2(IMildomSiteOptions siteOptions)
        {
            _siteOptions = siteOptions;
            siteOptions.PropertyChanged += SiteOptions_PropertyChanged;
        }
        private void SiteOptions_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_siteOptions.NeedAutoSubNickname):
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
    internal class ConnectedMessageMetadata2 : MessageMetadataBase2
    {
        protected internal ConnectedMessageMetadata2(IMildomConnected connected, IMildomSiteOptions siteOptions)
            : base(siteOptions)
        {
        }
    }
    internal class DisconnectedMessageMetadata2 : MessageMetadataBase2
    {
        protected internal DisconnectedMessageMetadata2(IMildomDisconnected disconnected, IMildomSiteOptions siteOptions)
            : base(siteOptions)
        {
        }
    }
    internal class JoinMessageMetadata2 : MessageMetadataBase2
    {
        public JoinMessageMetadata2(IMildomJoinRoom _, IMildomSiteOptions siteOptions)
            : base(siteOptions)
        {
        }
    }
    internal class GiftMessageMetadata2 : MessageMetadataBase2
    {
        public GiftMessageMetadata2(MildomGift _, IMildomSiteOptions siteOptions)
            : base(siteOptions)
        {
        }
    }
    internal class CommentMessageMetadata2 : MessageMetadataBase2
    {
        public override bool IsNgUser => false;
        private bool _isFirstComment;
        public override bool IsFirstComment
        {
            get
            {
                return base.IsFirstComment;
            }
        }
        public CommentMessageMetadata2(IMildomComment comment, IMildomSiteOptions siteOptions, bool isFirstComment)
            : base(siteOptions)
        {
            _isFirstComment = isFirstComment;
        }
    }
}
