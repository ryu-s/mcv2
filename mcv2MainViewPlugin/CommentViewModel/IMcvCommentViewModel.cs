using System.Collections.Generic;
using SitePlugin;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using System;

namespace mcv2.MainViewPlugin
{
    public interface IMcvCommentViewModel : INotifyPropertyChanged
    {
        SolidColorBrush Background { get; }
        SolidColorBrush Foreground { get; }
        IMainViewConnectionStatus ConnectionStatus { get; }
        FontFamily FontFamily { get; }
        int FontSize { get; }
        FontStyle FontStyle { get; }
        FontWeight FontWeight { get; }

        string Id { get; }
        string Info { get; }
        bool IsVisible { get; }
        IEnumerable<IMessagePart> MessageItems { get; }
        IEnumerable<IMessagePart> NameItems { get; }
        string PostTime { get; }
        IMessageImage Thumbnail { get; }
        string UserId { get; }
        TextWrapping UserNameWrapping { get; }
        SitePluginId SiteContextGuid { get; }
    }
}
