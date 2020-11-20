using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace MirrativSitePlugin
{
    internal interface IMirrativMessageMetadata2 : IMessageMetadata2
    {

    }
    internal abstract class MessageMetadataBase2 : IMirrativMessageMetadata2
    {
        protected readonly IMirrativSiteOptions _siteOptions;
        public virtual bool IsNgUser => false;
        public bool IsSiteNgUser => false;//TODO:IUserにIsSiteNgUserを追加する
        public virtual bool IsFirstComment => false;
        public string SiteName { get; }
        public bool Is184 { get; }
        public bool IsInitialComment { get; set; }
        public SitePluginId SiteContextGuid { get; set; }
        public string UserId { get; }
        public IEnumerable<IMessagePart> UserName { get; }
        public SiteType SiteType => SiteType.Mirrativ;

        protected MessageMetadataBase2(IMirrativSiteOptions siteOptions)
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
        protected internal ConnectedMessageMetadata2(IMirrativConnected connected, IMirrativSiteOptions siteOptions)
            : base(siteOptions)
        {
        }
    }
    internal class DisconnectedMessageMetadata2 : MessageMetadataBase2
    {
        protected internal DisconnectedMessageMetadata2(IMirrativDisconnected disconnected, IMirrativSiteOptions siteOptions)
            : base(siteOptions)
        {
        }
    }
    internal class JoinMessageMetadata2 : MessageMetadataBase2
    {
        public JoinMessageMetadata2(IMirrativJoinRoom join, IMirrativSiteOptions siteOptions)
            : base(siteOptions)
        {
        }
    }
    internal class ItemMessageMetadata2 : MessageMetadataBase2
    {
        private readonly IMirrativItem _item;

        public ItemMessageMetadata2(IMirrativItem item, IMirrativSiteOptions siteOptions)
            : base(siteOptions)
        {
            _item = item;
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
        public CommentMessageMetadata2(IMirrativComment comment, IMirrativSiteOptions siteOptions, bool isFirstComment)
            : base(siteOptions)
        {
            _isFirstComment = isFirstComment;
        }
    }
}
