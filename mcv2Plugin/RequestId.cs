using System;

namespace mcv2
{
    public class RequestId
    {
        private Guid _guid;
        public RequestId()
        {
            _guid = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return obj is RequestId id &&
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
