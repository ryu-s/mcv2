using System;

namespace mcv2.MainViewPlugin
{
    class BrowserViewModel
    {
        public string DisplayName { get; }
        public Guid BrowserId { get; }
        public BrowserViewModel(string displayName, string profileName, Guid browserd)
        {
            DisplayName = !string.IsNullOrEmpty(profileName) ? $"{displayName}({profileName})" : displayName;
            BrowserId = browserd;
        }
    }
}
