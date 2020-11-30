using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    class MildomSiteOptionsViewModel : INotifyPropertyChanged
    {
        public bool NeedAutoSubNickname
        {
            get => ChangedOptions.NeedAutoSubNickname;
            set => ChangedOptions.NeedAutoSubNickname = value;
        }
        public bool IsShowJoinMessage
        {
            get => ChangedOptions.IsShowJoinMessage;
            set => ChangedOptions.IsShowJoinMessage = value;
        }
        public bool IsShowLeaveMessage
        {
            get => ChangedOptions.IsShowLeaveMessage;
            set => ChangedOptions.IsShowLeaveMessage = value;
        }
        public Color ItemForeColor
        {
            get => ChangedOptions.ItemForeColor;
            set => ChangedOptions.ItemForeColor = value;
        }
        public Color ItemBackColor
        {
            get => ChangedOptions.ItemBackColor;
            set => ChangedOptions.ItemBackColor = value;
        }
        internal IMildomSiteOptions ChangedOptions { get; }

        internal MildomSiteOptionsViewModel(IMildomSiteOptions siteOptions)
        {
            ChangedOptions = siteOptions;
        }
        public MildomSiteOptionsViewModel()
        {
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
            {
                var options = new MildomSiteOptions
                {
                    ItemBackColor = Colors.Blue,
                    ItemForeColor = Colors.Red,
                    NeedAutoSubNickname = true,
                    PollingIntervalSec = 30,
                };
                ChangedOptions = options;
            }
            else
            {
                throw new NotSupportedException();
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
