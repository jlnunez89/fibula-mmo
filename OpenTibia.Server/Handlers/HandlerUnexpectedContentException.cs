using System;
using System.Runtime.Serialization;

namespace OpenTibia.Server.Handlers
{
    [Serializable]
    public class HandlerUnexpectedContentException : Exception
    {
        public HandlerUnexpectedContentException()
        {
        }

        public HandlerUnexpectedContentException(string message) : base(message)
        {
        }

        public HandlerUnexpectedContentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HandlerUnexpectedContentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}