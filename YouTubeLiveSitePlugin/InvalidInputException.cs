using System;
using System.Runtime.Serialization;

namespace YouTubeLiveSitePlugin
{
    [Serializable]
    internal class InvalidInputException : Exception
    {
        public string Details { get; }
        public InvalidInputException(string message, string details) : base(message)
        {
            Details = details;
        }

        public InvalidInputException(string message, string details, Exception innerException) : base(message, innerException)
        {
            Details = details;
        }

        protected InvalidInputException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Details = "";
        }
    }
}