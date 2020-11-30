using System;
namespace SitePlugin
{
    public sealed class SitePluginId
    {
        private Guid _guid;
        public SitePluginId(Guid guid)//起動する毎に値が変わると何かと不便そうだから固定値を代入する形にした
        {
            _guid = guid;
        }

        public override bool Equals(object? obj)
        {
            return obj is SitePluginId id &&
                   _guid.Equals(id._guid);
        }

        public override int GetHashCode()
        {
            return -2045414129 + _guid.GetHashCode();
        }
        public override string ToString()
        {
            return _guid.ToString();
        }
    }
}

