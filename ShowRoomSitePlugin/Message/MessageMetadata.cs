﻿using SitePlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace ShowRoomSitePlugin
{
    internal class MessageMetadata2 : IMessageMetadata2
    {
        private readonly IShowRoomMessage _message;
        private readonly IShowRoomSiteOptions _siteOptions;

        public bool IsNgUser => false;
        public bool IsSiteNgUser => false;//TODO:IUserにIsSiteNgUserを追加する
        public bool IsFirstComment { get; }
        public bool Is184 { get; }
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
        public SitePluginId SiteContextGuid { get; set; }
        public string UserId { get; set; }
        public IEnumerable<IMessagePart> UserName { get; set; }
        public SiteType SiteType => SiteType.ShowRoom;
        public string? NewNickname { get; set; }

        public MessageMetadata2(IShowRoomMessage message, IShowRoomSiteOptions siteOptions, bool isFirstComment)
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
                //case nameof(_siteOptions.ItemBackColor):
                //    if (_message is IShowRoomItem)
                //    {
                //        RaisePropertyChanged(nameof(BackColor));
                //    }
                //    break;
                //case nameof(_siteOptions.ItemForeColor):
                //    if (_message is IShowRoomItem)
                //    {
                //        RaisePropertyChanged(nameof(ForeColor));
                //    }
                //    break;
                case nameof(_siteOptions.IsShowJoinMessage):
                    if (_message is IShowRoomJoin)
                    {
                        RaisePropertyChanged(nameof(IsVisible));
                    }
                    break;
                case nameof(_siteOptions.IsShowLeaveMessage):
                    if (_message is IShowRoomLeave)
                    {
                        RaisePropertyChanged(nameof(IsVisible));
                    }
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
