﻿using SitePlugin;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace LineLiveSitePlugin
{
    internal class MessageMetadata2 : IMessageMetadata2
    {
        private readonly ILineLiveMessage _message;
        private readonly ILineLiveSiteOptions _siteOptions;

        public bool IsNgUser => false;
        public bool IsSiteNgUser => false;//TODO:IUserにIsSiteNgUserを追加する
        public bool IsFirstComment { get; }
        public bool Is184 { get; }
        public SitePluginId SiteContextGuid { get; set; }
        public bool IsVisible
        {
            get
            {
                if (IsNgUser || IsSiteNgUser) return false;

                //TODO:ConnectedとかDisconnectedの場合、表示するエラーレベルがError以下の場合にfalseにしたい
                //→Connected,Disconnectedくらいは常に表示でも良いかも。エラーメッセージだけエラーレベルを設けようか。
                return true;
            }
        }
        public bool IsInitialComment { get; set; }
        public string UserId { get; }
        public IEnumerable<IMessagePart> UserName { get; }
        public SiteType SiteType => SiteType.LineLive;
        public string? NewNickname { get; set; }

        public MessageMetadata2(ILineLiveMessage message, ILineLiveSiteOptions siteOptions, bool isFirstComment)
        {
            _message = message;
            _siteOptions = siteOptions;
            IsFirstComment = isFirstComment;
            siteOptions.PropertyChanged += SiteOptions_PropertyChanged;
        }
        private void SiteOptions_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                default:
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
}
