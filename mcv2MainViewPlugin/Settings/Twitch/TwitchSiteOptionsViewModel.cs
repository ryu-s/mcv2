using System;
using System.ComponentModel;
namespace mcv2.MainViewPlugin
{
    class TwitchSiteOptionsViewModel : INotifyPropertyChanged
    {
        public bool NeedAutoSubNickname
        {
            get => ChangedOptions.NeedAutoSubNickname;
            set => ChangedOptions.NeedAutoSubNickname = value;
        }
        public string NeedAutoSubNicknameStr
        {
            get => ChangedOptions.NeedAutoSubNicknameStr;
            set => ChangedOptions.NeedAutoSubNicknameStr = value;
        }
        internal ITwitchSiteOptions ChangedOptions { get; }
        internal TwitchSiteOptionsViewModel()
        {
            if (Tools.IsInDesignMode())
            {
                ChangedOptions = new TwitchSiteOptions
                {
                    NeedAutoSubNickname = true,
                    NeedAutoSubNicknameStr = "test",
                };
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        internal TwitchSiteOptionsViewModel(ITwitchSiteOptions siteOptions)
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
