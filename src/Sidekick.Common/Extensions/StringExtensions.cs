using System.Text;
using System.Web;

namespace Sidekick.Common.Extensions
{
    /// <summary>
    /// Class containing extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Encode a string in Base64
        /// </summary>
        public static string EncodeBase64(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Decodes a Base64 string
        /// </summary>
        public static string DecodeBase64(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }

        /// <summary>
        /// Encode a string for URL transfer
        /// </summary>
        public static string EncodeUrl(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return HttpUtility.UrlEncode(input);
        }

        /// <summary>
        /// Decodes a Url Encodeded String
        /// </summary>
        public static string DecodeUrl(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return HttpUtility.UrlDecode(input);
        }

        /// <summary>
        /// Encode a string in Base64 for URL transfer
        /// </summary>
        public static string? EncodeBase64Url(this string? input)
        {
            if (input == null)
            {
                return null;
            }

            if (input.HasInvalidUrlCharacters())
            {
                return $"xurl_{input.EncodeBase64().EncodeUrl()}";
            }

            return input;
        }

        /// <summary>
        /// Decodes Base64 Url
        /// </summary>
        public static string? DecodeBase64Url(this string? input)
        {
            if (input == null)
            {
                return null;
            }

            if (input.StartsWith("xurl_"))
            {
                var substr = input.Substring(5);
                return substr.DecodeBase64();
            }

            return input;
        }

        /// <summary>
        /// Indicates if the string has invalid characters
        /// </summary>
        public static bool HasInvalidUrlCharacters(this string input)
        {
            return input.EncodeUrl() != input;
        }
    }
}
