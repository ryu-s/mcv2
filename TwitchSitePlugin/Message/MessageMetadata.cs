using SitePlugin;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace TwitchSitePlugin
{
    internal class MessageMetadata2 : IMessageMetadata2
    {
        private readonly ITwitchMessage _message;
        private readonly ITwitchSiteOptions _siteOptions;

        public bool IsNgUser => false;
        public bool IsSiteNgUser => false;
        public bool IsFirstComment { get; }
        public bool Is184 { get; }
        public ICommentProvider CommentProvider { get; }
        public bool IsInitialComment { get; set; }
        public SitePluginId SiteContextGuid { get; set; }
        public string UserId { get; set; }
        public IEnumerable<IMessagePart> UserName { get; set; }
        public SiteType SiteType => SiteType.Twitch;
        public MessageMetadata2(ITwitchMessage message, ITwitchSiteOptions siteOptions, ICommentProvider cp, bool isFirstComment)
        {
            _message = message;
            _siteOptions = siteOptions;
            IsFirstComment = isFirstComment;
            CommentProvider = cp;

            //TODO:siteOptionsのpropertyChangedが発生したら関係するプロパティの変更通知を出したい

            siteOptions.PropertyChanged += SiteOptions_PropertyChanged;
        }
        private void SiteOptions_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                //case nameof(_siteOptions.ItemBackColor):
                //    if (_message is ITwitchItem)
                //    {
                //        RaisePropertyChanged(nameof(BackColor));
                //    }
                //    break;
                //case nameof(_siteOptions.ItemForeColor):
                //    if (_message is ITwitchItem)
                //    {
                //        RaisePropertyChanged(nameof(ForeColor));
                //    }
                //    break;
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
}
