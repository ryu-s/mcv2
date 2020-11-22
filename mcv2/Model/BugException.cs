using System;

namespace mcv2.Model
{
    [Serializable]
    public class BugException : Exception
    {
        public BugException() { }
        public BugException(string message) : base(message) { }
        public BugException(string message, Exception inner) : base(message, inner) { }
        protected BugException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
