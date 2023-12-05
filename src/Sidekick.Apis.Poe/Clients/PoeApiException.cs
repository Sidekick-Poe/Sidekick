using System.Runtime.Serialization;

namespace Sidekick.Apis.Poe.Clients
{
    [Serializable]
    public class PoeApiException : Exception
    {
        public PoeApiException()
        {
        }

        public PoeApiException(string? message) : base(message)
        {
        }

        public PoeApiException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PoeApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
