﻿using System;
using System.ComponentModel;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class TwicasSiteOptionsViewModel : INotifyPropertyChanged
    {
        public int CommentRetrieveIntervalSec
        {
            get { return ChangedOptions.CommentRetrieveIntervalSec; }
            set { ChangedOptions.CommentRetrieveIntervalSec = value; }
        }
        public Color KiitosBackColor
        {
            get { return ChangedOptions.KiitosBackColor; }
            set { ChangedOptions.KiitosBackColor = value; }
        }
        public Color KiitosForeColor
        {
            get { return ChangedOptions.KiitosForeColor; }
            set { ChangedOptions.KiitosForeColor = value; }
        }
        public Color ItemBackColor
        {
            get { return ChangedOptions.ItemBackColor; }
            set { ChangedOptions.ItemBackColor = value; }
        }
        public Color ItemForeColor
        {
            get { return ChangedOptions.ItemForeColor; }
            set { ChangedOptions.ItemForeColor = value; }
        }
        public bool IsAutoSetNickname
        {
            get { return ChangedOptions.IsAutoSetNickname; }
            set { ChangedOptions.IsAutoSetNickname = value; }
        }
        public bool IsShowItem
        {
            get => ChangedOptions.IsShowItem;
            set => ChangedOptions.IsShowItem = value;
        }
        internal ITwicasSiteOptions ChangedOptions { get; }

        internal TwicasSiteOptionsViewModel(ITwicasSiteOptions siteOptions)
        {
            ChangedOptions = siteOptions;
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
