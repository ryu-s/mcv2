using SitePlugin;
using System.Collections.Generic;
using System.ComponentModel;

namespace mcv2.MainViewPlugin
{
    interface IUserViewModel : INotifyPropertyChanged
    {
        bool IsNgUser { get; set; }
        bool IsSiteNgUser { get; set; }
        IEnumerable<IMessagePart> UsernameItems { get; set; }
        string Nickname { get; set; }
        string UserId { get; }
    }
}
