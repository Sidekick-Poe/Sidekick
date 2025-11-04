using System.Text;
using System.Web;

namespace Sidekick.Common.Extensions;

/// <summary>
///     Class containing extension methods for strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Encode a string in Base64
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
    ///     Decodes a Base64 string
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
    ///     Encode a string in Base64 for URL transfer
    /// </summary>
    public static string? EncodeBase64Url(this string? input)
    {
        if (input == null)
        {
            return null;
        }

        var url = input.EncodeBase64();
        url = url.Replace('+', '-')
            .Replace('/', '_')
            .Replace('=', '.');
        return $"base64_{url}";
    }

    /// <summary>
    ///     Decodes Base64 Url
    /// </summary>
    public static string? DecodeBase64Url(this string? input)
    {
        if (input == null)
        {
            return null;
        }

        if (!input.StartsWith("base64_"))
        {
            return input;
        }

        var url = input.Substring(7)
            .Replace('-', '+')
            .Replace('_', '/')
            .Replace('.', '=');
        url = url.DecodeBase64();
        return url;
    }

    public static int GetDeterministicHashCode(this string str)
    {
        unchecked
        {
            var hash1 = (5381 << 16) + 5381;
            var hash2 = hash1;

            for (var i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}
