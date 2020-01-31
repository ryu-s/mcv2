using System;
using System.ComponentModel;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    internal class WhowatchSiteOptionsViewModel : INotifyPropertyChanged
    {
        public bool NeedAutoSubNickname
        {
            get => ChangedOptions.NeedAutoSubNickname;
            set => ChangedOptions.NeedAutoSubNickname = value;
        }
        public Color ItemBackColor
        {
            get => ChangedOptions.ItemBackColor;
            set => ChangedOptions.ItemBackColor = value;
        }
        public Color ItemForeColor
        {
            get => ChangedOptions.ItemForeColor;
            set => ChangedOptions.ItemForeColor = value;
        }
        internal IWhowatchSiteOptions ChangedOptions { get; }

        internal WhowatchSiteOptionsViewModel(IWhowatchSiteOptions siteOptions)
        {
            ChangedOptions = siteOptions;
        }
        public WhowatchSiteOptionsViewModel()
        {
            ChangedOptions = new WhowatchSiteOptions()
            {
                ItemBackColor = Colors.Red,
                ItemForeColor = Colors.Blue,
                NeedAutoSubNickname = true,
            };
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
