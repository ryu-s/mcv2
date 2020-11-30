using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using GalaSoft.MvvmLight;
using SitePlugin;

namespace mcv2YoyakuPlugin
{
    public class YoyakuUser : ViewModelBase
    {
        public YoyakuUser()
        {
        }
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged();
            }
        }
        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }
        public string DisplayName => Nickname ?? Name;
        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            set
            {
                _nickname = value;
                RaisePropertyChanged(nameof(DisplayName));
            }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(DisplayName));
            }
        }
        public SitePluginId SitePluginGuid { get; set; }
        private bool _hasCalled;

        /// <summary>
        /// 呼び出し済みか
        /// </summary>
        public bool HadCalled
        {
            get { return _hasCalled; }
            set
            {
                _hasCalled = value;
                RaisePropertyChanged();
            }
        }
        public override string ToString()
        {
            return $"{Name} id={Id}";
        }
        public override bool Equals(object obj)
        {
            if (!(obj is YoyakuUser user))
                return false;
            if (this.Id == null) return false;
            return Id.Equals(user.Id);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
