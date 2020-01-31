using GalaSoft.MvvmLight;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace mcv2.MainViewPlugin
{
    /// <summary>
    /// ユーザー
    /// IMcvUserにはIPropertyChangedが実装されていないからこのクラスが必要になる
    /// </summary>
    class UserViewModel : CommentDataGridViewModelBase, IUserViewModel
    {
        private readonly SitePluginId _siteContextGuid;
        private readonly IUserHost _model;
        private bool _isNgUser;
        private bool _isSiteNgUser;
        private string _nickname;
        private IEnumerable<IMessagePart> _name;
        private bool _isEnabledUserBackColor;
        private bool _isEnabledUserForeColor;
        private Color _backColor;
        private Color _foreColor;

        public UserViewModel() : base(new DynamicOptionsTest())
        {
            if (!Tools.IsInDesignMode())
            {
                throw new NotSupportedException();
            }
            UserId = "userid";
            UsernameItems = Common.MessagePartFactory.CreateMessageItems("name");
            Nickname = "nick";
            IsNgUser = true;
            IsSiteNgUser = true;
        }
        [GalaSoft.MvvmLight.Ioc.PreferredConstructor]
        public UserViewModel(SitePluginId siteContextGuid, string userId, IOptions options, IUserHost model) :
            base(options)
        {
            _siteContextGuid = siteContextGuid;
            UserId = userId;
            _model = model;
        }

        public string UserId { get; }
        public IEnumerable<IMessagePart> UsernameItems
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }
        public string Nickname
        {
            get
            {
                return _model.GetNickname(_siteContextGuid, UserId);
            }
            set
            {
                _model.SetNickname(_siteContextGuid, UserId, value);
                RaisePropertyChanged();
            }
        }
        public bool IsNgUser
        {
            get
            {
                return _model.GetIsNgUser(_siteContextGuid, UserId);
            }
            set
            {
                _model.SetIsNgUser(_siteContextGuid, UserId, value);
                RaisePropertyChanged();
            }
        }
        public bool IsSiteNgUser
        {
            get
            {
                return _model.GetIsSiteNgUser(_siteContextGuid, UserId);
            }
            set
            {
                _model.SetIsSiteNgUser(_siteContextGuid, UserId, value);
                RaisePropertyChanged();
            }
        }
        public bool IsEnabledUserBackColor
        {
            get
            {
                return _isEnabledUserBackColor;
            }
            set
            {
                _isEnabledUserBackColor = value;
                RaisePropertyChanged();
            }
        }
        public bool IsEnabledUserForeColor
        {
            get
            {
                return _isEnabledUserForeColor;
            }
            set
            {
                _isEnabledUserForeColor = value;
                RaisePropertyChanged();
            }
        }
        public Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
                RaisePropertyChanged();
            }
        }
        public Color ForeColor
        {
            get
            {
                return _foreColor;
            }
            set
            {
                _foreColor = value;
                RaisePropertyChanged();
            }
        }
    }
}
