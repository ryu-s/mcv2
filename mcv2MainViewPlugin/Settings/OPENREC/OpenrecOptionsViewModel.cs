using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace mcv2.MainViewPlugin
{
    public class OpenrecOptionsViewModel : INotifyPropertyChanged
    {
        public ICommand ShowOpenStampMusicSelectorCommand { get; }
        public ICommand ShowOpenYellMusicSelectorCommand { get; }
        private void ShowOpenStampMusicSelector()
        {
            var filename = OpenFileDialog("", "音声ファイルを指定して下さい", "waveファイル|*.wav");
            if (!string.IsNullOrEmpty(filename))
            {
                StampMusicFilePath = filename;
            }
        }
        private void ShowOpenYellMusicSelector()
        {
            var filename = OpenFileDialog("", "音声ファイルを指定して下さい", "waveファイル|*.wav");
            if (!string.IsNullOrEmpty(filename))
            {
                YellMusicFilePath = filename;
            }
        }
        protected virtual string? OpenFileDialog(string defaultPath, string title, string filter)
        {
            string? ret;
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = title,
                Filter = filter
            };
            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                ret = fileDialog.FileName;
            }
            else
            {
                ret = null;
            }
            return ret;
        }
        public int StampSize
        {
            get { return ChangedOptions.StampSize; }
            set { ChangedOptions.StampSize = value; }
        }
        public bool IsPlayStampMusic
        {
            get { return ChangedOptions.IsPlayStampMusic; }
            set
            {
                ChangedOptions.IsPlayStampMusic = value;
                RaisePropertyChanged();
            }
        }
        public string StampMusicFilePath
        {
            get { return ChangedOptions.StampMusicFilePath; }
            set
            {
                ChangedOptions.StampMusicFilePath = value;
                RaisePropertyChanged();
            }
        }
        public bool IsPlayYellMusic
        {
            get { return ChangedOptions.IsPlayYellMusic; }
            set
            {
                ChangedOptions.IsPlayYellMusic = value;
                RaisePropertyChanged();
            }
        }
        public string YellMusicFilePath
        {
            get { return ChangedOptions.YellMusicFilePath; }
            set
            {
                ChangedOptions.YellMusicFilePath = value;
                RaisePropertyChanged();
            }
        }
        public bool IsAutoSetNickname
        {
            get { return ChangedOptions.IsAutoSetNickname; }
            set { ChangedOptions.IsAutoSetNickname = value; }
        }
        internal IOpenrecSiteOptions ChangedOptions { get; }

        internal OpenrecOptionsViewModel(IOpenrecSiteOptions siteOptions)
        {
            ChangedOptions = siteOptions;
            ShowOpenStampMusicSelectorCommand = new RelayCommand(ShowOpenStampMusicSelector);
            ShowOpenYellMusicSelectorCommand = new RelayCommand(ShowOpenYellMusicSelector);
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
