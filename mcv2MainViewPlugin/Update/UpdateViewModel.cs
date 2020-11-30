using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using mcv2;
using mcv2.MainViewPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mcv2MainViewPlugin.Update
{
    enum PageType
    {
        First,//update check
        Second,
        Third,
        Forth,
        ServerError,
        NotUpdate,
    }
    static class Tools
    {
        /// <summary>
        /// aVersionは更新が必要か
        /// </summary>
        /// <param name="aVersion">1.2.3</param>
        /// <param name="bVersion">1.2.3</param>
        /// <returns>aVersion < bVersionの時にtrue</returns>
        public static bool NeedUpdate(string? aVersion, string? bVersion)
        {
            if (!(IsValidVersionNumber(aVersion) && IsValidVersionNumber(bVersion)))
            {
                return false;
            }
            var validA = aVersion!;
            var validB = bVersion!;
            var coreArr = validA.Split('.').Select(s => int.Parse(s)).ToList();
            var latestArr = validB.Split('.').Select(s => int.Parse(s)).ToList();
            var diff = Math.Abs(coreArr.Count - latestArr.Count);
            if (diff > 0)
            {
                //長さを揃える
                var arr = coreArr.Count < latestArr.Count ? coreArr : latestArr;
                arr.AddRange(Enumerable.Repeat(0, diff));
            }
            var ret = false;
            for (int i = 0; i < coreArr.Count; i++)
            {
                if (coreArr[i] == latestArr[i]) continue;
                if (coreArr[i] >= latestArr[i])
                {
                    ret = false;
                }
                else
                {
                    ret = true;
                }
                break;
            }
            return ret;
        }

        public static bool IsValidVersionNumber(string? coreVersion)
        {
            if (string.IsNullOrWhiteSpace(coreVersion)) return false;
            return Regex.IsMatch(coreVersion, "^(?:\\d+\\.)*\\d+$");
        }
    }
    class UpdateViewModel : ViewModelBase
    {

        private string _currentVersion;
        private string _latestVersion;
        private readonly IModel _host;

        public bool IsShowPageFirst
        {
            get
            {
                return _isShowPageFirst;
            }
            private set
            {
                _isShowPageFirst = value;
                RaisePropertyChanged();
            }
        }
        public bool IsShowPageSecond
        {
            get
            {
                return _isShowPageSecond;
            }
            set
            {
                _isShowPageSecond = value;
                RaisePropertyChanged();
            }
        }
        public bool IsShowPageThird
        {
            get
            {
                return _isShowPageThird;
            }
            set
            {
                _isShowPageThird = value;
                RaisePropertyChanged();
            }
        }
        public bool IsShowPageForth
        {
            get
            {
                return _isShowPageForth;
            }
            set
            {
                _isShowPageForth = value;
                RaisePropertyChanged();
            }
        }
        public bool IsShowPageNotUpdate
        {
            get
            {
                return _isShowPageNotUpdate;
            }
            set
            {
                _isShowPageNotUpdate = value;
                RaisePropertyChanged();
            }
        }
        private void ShowPage(PageType toShow)
        {
            switch (toShow)
            {
                case PageType.First:
                    IsShowPageFirst = true;
                    IsShowPageSecond = false;
                    IsShowPageThird = false;
                    IsShowPageForth = false;
                    IsShowPageNotUpdate = false;
                    break;
                case PageType.Second:
                    IsShowPageFirst = false;
                    IsShowPageSecond = true;
                    IsShowPageThird = false;
                    IsShowPageForth = false;
                    IsShowPageNotUpdate = false;
                    break;
                case PageType.Third:
                    IsShowPageFirst = false;
                    IsShowPageSecond = false;
                    IsShowPageThird = true;
                    IsShowPageForth = false;
                    IsShowPageNotUpdate = false;
                    break;
                case PageType.Forth:
                    IsShowPageFirst = false;
                    IsShowPageSecond = false;
                    IsShowPageThird = false;
                    IsShowPageForth = true;
                    IsShowPageNotUpdate = false;
                    break;
                case PageType.NotUpdate:
                    IsShowPageFirst = false;
                    IsShowPageSecond = false;
                    IsShowPageThird = false;
                    IsShowPageForth = false;
                    IsShowPageNotUpdate = true;
                    break;
            }
        }
        public string CurrentVersion
        {
            get
            {
                return _currentVersion;
            }
            set
            {
                _currentVersion = value;
                RaisePropertyChanged();
            }
        }
        public string LatestVersion
        {
            get
            {
                return _latestVersion;
            }
            set
            {
                _latestVersion = value;
                RaisePropertyChanged();
            }
        }
        public ICommand UpdateViewContentRenderedCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CloseCommand { get; }
        public UpdateViewModel()
        {
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(System.Windows.DependencyObject)).DefaultValue))
            {
                CurrentVersion = "10.20.30";
                LatestVersion = "70.80.90";
                ShowPage(PageType.Third);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        public UpdateViewModel(IModel host)
        {
            UpdateViewContentRenderedCommand = new RelayCommand(UpdateViewContentRendered);
            OkCommand = new RelayCommand(Ok);
            CancelCommand = new RelayCommand(Cancel);
            CloseCommand = new RelayCommand(Close);
            _host = host;
            ShowPage(PageType.First);

            host.NotifyReceived += Host_NotifyReceived;
        }

        private void Host_NotifyReceived(object? sender, INotify e)
        {
            switch (e)
            {
                case NotifyDownloadCoreProgress progress:
                    ProgressPercentage = progress.ProgressPercentage;
                    break;
            }
        }
        public int ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            set
            {
                _progressPercentage = value;
                RaisePropertyChanged();
            }
        }

        private async void UpdateViewContentRendered()
        {
            ShowPage(PageType.First);
            var preCoreVersion = _host.GetCoreVersion();
            var (latestVersion, latestVersionUrl) = await _host.GetLatestVersionAsync();
            var coreVersion = GetVersionNumber(preCoreVersion);
            if (Tools.NeedUpdate(coreVersion, latestVersion))
            {
                CurrentVersion = coreVersion!;
                LatestVersion = latestVersion!;
                _latestVersionUrl = latestVersionUrl!;
                ShowPage(PageType.Second);
            }
            else
            {
                ShowPage(PageType.NotUpdate);
            }
        }

        private string _latestVersionUrl;
        private bool _isShowPageFirst;
        private bool _isShowPageSecond;
        private bool _isShowPageThird;
        private bool _isShowPageForth;
        private int _progressPercentage;
        private bool _isShowPageNotUpdate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version">v1.2.3</param>
        /// <returns>1.2.3</returns>
        private string? GetVersionNumber(string version)
        {
            var match = Regex.Match(version, "v(\\d+\\.\\d+\\.\\d+)");
            if (!match.Success) return null;
            return match.Groups[1].Value;
        }
        private async void Ok()
        {
            ShowPage(PageType.Third);
            await _host.RequestUpdateCoreAsync(_latestVersionUrl);
        }
        private void Cancel()
        {

        }
        private void Close()
        {
            MessengerInstance.Send(new CloseUpdateViewMessage());
        }
    }

}
