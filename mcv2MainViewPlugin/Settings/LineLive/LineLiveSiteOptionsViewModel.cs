using System;
using System.ComponentModel;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    internal class LineLiveSiteOptionsViewModel : INotifyPropertyChanged
    {
        public bool IsAutoSetNickname
        {
            get { return ChangedOptions.IsAutoSetNickname; }
            set { ChangedOptions.IsAutoSetNickname = value; }
        }

        internal ILineLiveSiteOptions ChangedOptions { get; }

        internal LineLiveSiteOptionsViewModel(ILineLiveSiteOptions siteOptions)
        {
            ChangedOptions = siteOptions;
        }
        public LineLiveSiteOptionsViewModel() : this(new LineLiveSiteOptions())
        {
            ChangedOptions = new LineLiveSiteOptions
            {
                IsAutoSetNickname = true,
                ItemCommentBackColor = Colors.Red,
                ItemCommentForeColor = Colors.Blue,
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
