using System;

namespace mcv2
{
    public class PluginId
    {
        private Guid _guid;
        public PluginId()
        {
            _guid = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return obj is PluginId id &&
                   _guid.Equals(id._guid);
        }

        public override int GetHashCode()
        {
            return -2045414129 + _guid.GetHashCode();
        }
    }

}
