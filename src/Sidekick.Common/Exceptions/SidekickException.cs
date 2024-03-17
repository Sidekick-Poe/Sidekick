using Sidekick.Common.Extensions;

namespace Sidekick.Common.Exceptions
{
    /// <summary>
    /// Represents a sidekick exception.
    /// </summary>
    public abstract class SidekickException : Exception
    {
        public SidekickException(string message) :
            base(message)
        {
        }

        public SidekickException(string message, string? additionalInformation) :
            base(message)
        {
            AdditionalInformation = additionalInformation;
        }

        public string? AdditionalInformation { get; }

        /// <summary>
        /// Grabs the relevent url from the error type value.
        /// </summary>
        /// <returns>The url.</returns>
        public string ToUrl()
        {
            if (!string.IsNullOrEmpty(AdditionalInformation))
            {
                var escapedAdditionalInformation = AdditionalInformation.EncodeBase64Url();
                return $"/error/{Message.EncodeBase64Url()}/{escapedAdditionalInformation}";
            }

            return $"/error/{Message.EncodeBase64Url()}";
        }
    }
}
