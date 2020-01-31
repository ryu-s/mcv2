using System;
using System.ComponentModel;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    public class NicoSiteOptionsViewModel : INotifyPropertyChanged
    {
        public int OfficialRoomsRetrieveCount
        {
            get { return ChangedOptions.OfficialRoomsRetrieveCount; }
            set { ChangedOptions.OfficialRoomsRetrieveCount = value; }
        }
        public bool IsShow184
        {
            get => ChangedOptions.IsShow184;
            set => ChangedOptions.IsShow184 = value;
        }
        public bool IsShow184Id
        {
            get => ChangedOptions.IsShow184Id;
            set => ChangedOptions.IsShow184Id = value;
        }
        public bool IsAutoSetNickname
        {
            get { return ChangedOptions.IsAutoSetNickname; }
            set { ChangedOptions.IsAutoSetNickname = value; }
        }
        public bool IsAutoGetUsername
        {
            get => ChangedOptions.IsAutoGetUsername;
            set => ChangedOptions.IsAutoGetUsername = value;
        }
        public Color AdBackColor
        {
            get => ChangedOptions.AdBackColor;
            set => ChangedOptions.AdBackColor = value;
        }
        public Color AdForeColor
        {
            get => ChangedOptions.AdForeColor;
            set => ChangedOptions.AdForeColor = value;
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
        internal INicoSiteOptions ChangedOptions { get; }

        public NicoSiteOptionsViewModel()
        {
            //if(IsInDesigner)
            ChangedOptions = new NicoSiteOptions { OfficialRoomsRetrieveCount = 5 };

        }
        internal NicoSiteOptionsViewModel(INicoSiteOptions siteOptions)
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
