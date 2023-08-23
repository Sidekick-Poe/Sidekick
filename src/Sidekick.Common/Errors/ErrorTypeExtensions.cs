using Sidekick.Common.Extensions;

namespace Sidekick.Common.Errors
{
    /// <summary>
    /// Extensions for error type enum.
    /// </summary>
    public static class ErrorTypeExtensions
    {
        /// <summary>
        /// Grabs the relevent url from the error type value.
        /// </summary>
        /// <param name="type">The type to get the url for.</param>
        /// <param name="additionalInformation">Possibly pass in additional information.</param>
        /// <returns>The url.</returns>
        public static string ToUrl(this ErrorType type, string? additionalInformation = null)
        {
            if (!string.IsNullOrEmpty(additionalInformation))
            {
                var escapedAdditionalInformation = additionalInformation.EncodeBase64Url();
                return $"/error/{type}/{escapedAdditionalInformation}";
            }

            return $"/error/{type}";
        }
    }
}
