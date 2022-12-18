using Sidekick.Common.Extensions;

namespace Sidekick.Common.Errors
{
    public static class ErrorTypeExtensions
    {
        public static string ToUrl(this ErrorType type, string additionalInformation = null)
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
