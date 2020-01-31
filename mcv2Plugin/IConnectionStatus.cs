using SitePlugin;
using System;
using System.Runtime.CompilerServices;

namespace mcv2
{
    public interface IConnectionStatus
    {
        ConnectionId ConnectionId { get; }
        string Name { get; }
        string Input { get; }
        SitePluginId? Site { get; }
        Guid? Browser { get; }
        bool IsConnected { get; }
        string LoggedInUserName { get; }
    }
    public interface IConnectionStatusDiff
    {
        ConnectionId ConnectionId { get; }
        string? Name { get; }
        string? Input { get; }
        SitePluginId? Site { get; }
        Guid? Browser { get; }
        bool? IsConnected { get; }
        string? LoggedInUserName { get; }
    }
    public static class IConnectionStatusDiffTools
    {
        public static bool HasChanged(this IConnectionStatusDiff diff)
        {
            return diff.Name != null || diff.Input != null
                || diff.Site != null || diff.Browser.HasValue
                || diff.IsConnected.HasValue || diff.LoggedInUserName != null;
        }
    }
    public class ConnectionStatusDiff : IConnectionStatusDiff
    {
        public ConnectionId ConnectionId { get; }
        public string? Name { get; set; }
        public string? Input { get; set; }
        public SitePluginId? Site { get; set; }
        public Guid? Browser { get; set; }
        public bool? IsConnected { get; set; }
        public bool? Disconnect { get; set; }
        public string? LoggedInUserName { get; set; }

        public ConnectionStatusDiff(ConnectionId id)
        {
            ConnectionId = id;
        }
    }
}
