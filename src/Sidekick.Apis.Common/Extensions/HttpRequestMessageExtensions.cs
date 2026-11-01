namespace Sidekick.Apis.Common.Extensions;

public static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Adds or updates a specific cookie key-value pair inside the Cookie header of an HttpRequestMessage.
    /// </summary>
    public static void AddOrUpdateCookie(this HttpRequestMessage request, string cookieName, string cookieValue)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(cookieName)) throw new ArgumentException("Cookie name cannot be empty.", nameof(cookieName));

        var existingCookieHeader = string.Empty;
        if (request.Headers.TryGetValues("Cookie", out var cookieValues))
        {
            existingCookieHeader = cookieValues.FirstOrDefault() ?? string.Empty;
        }

        var updatedCookies = existingCookieHeader
            .Split([';'], StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim())
            .Where(c => !c.StartsWith($"{cookieName}=", StringComparison.OrdinalIgnoreCase))
            .ToList();

        updatedCookies.Add($"{cookieName}={cookieValue}");

        var newCookieHeaderValue = string.Join("; ", updatedCookies);
        request.Headers.Remove("Cookie");
        request.Headers.Add("Cookie", newCookieHeaderValue);
    }

    /// <summary>
    /// Merges an external cookie string into the Cookie header of an HttpRequestMessage,
    /// updating existing keys and appending new ones.
    /// </summary>
    public static void AddOrUpdateCookie(this HttpRequestMessage request, string newCookieString)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(newCookieString)) return;

        var currentCookieHeaderValue = string.Empty;
        if (request.Headers.TryGetValues("Cookie", out var rawCookieHeaderValues))
        {
            currentCookieHeaderValue = rawCookieHeaderValues.FirstOrDefault() ?? string.Empty;
        }

        var existingCookiesDictionary = ParseCookieStringToDictionary(currentCookieHeaderValue);
        var incomingCookiesDictionary = ParseCookieStringToDictionary(newCookieString);

        foreach (var incomingCookie in incomingCookiesDictionary)
        {
            existingCookiesDictionary[incomingCookie.Key] = incomingCookie.Value;
        }

        var mergedCookieHeaderValue = string.Join("; ", existingCookiesDictionary.Select(cookie => $"{cookie.Key}={cookie.Value}"));

        request.Headers.Remove("Cookie");
        request.Headers.Add("Cookie", mergedCookieHeaderValue);
    }

    private static Dictionary<string, string> ParseCookieStringToDictionary(string rawCookieString)
    {
        var parsedCookiesCollection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(rawCookieString)) return parsedCookiesCollection;

        var cookieKeyValuePairSegments = rawCookieString.Split([';'], StringSplitOptions.RemoveEmptyEntries);
        foreach (var pairSegment in cookieKeyValuePairSegments)
        {
            var trimmedKeyValueSegment = pairSegment.Trim();
            var keyValueSeparatorIndex = trimmedKeyValueSegment.IndexOf('=');

            var isMalformedSegment = keyValueSeparatorIndex <= 0;
            if (isMalformedSegment) continue;

            var cookieName = trimmedKeyValueSegment.Substring(0, keyValueSeparatorIndex).Trim();
            var cookieValue = trimmedKeyValueSegment.Substring(keyValueSeparatorIndex + 1).Trim();

            parsedCookiesCollection[cookieName] = cookieValue;
        }

        return parsedCookiesCollection;
    }
}
