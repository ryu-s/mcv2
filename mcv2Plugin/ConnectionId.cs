using System;
namespace SitePlugin
{
    public class ConnectionId
    {
        private Guid _guid;
        public ConnectionId()
        {
            _guid = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return obj is ConnectionId id &&
                   _guid.Equals(id._guid);
        }

        public override int GetHashCode()
        {
            return -2045414129 + _guid.GetHashCode();
        }
    }
}

