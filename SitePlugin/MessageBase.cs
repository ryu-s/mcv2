using System;
using System.Collections.Generic;

namespace SitePlugin
{
    public abstract class MessageBase2 : ISiteMessage, IValueChanged
    {
        public string Raw { get; }
        public abstract SiteType SiteType { get; }
        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        public void OnValueChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs { PropertyName = propertyName });
        }
        public MessageBase2(string raw)
        {
            Raw = raw;
        }
    }
}
