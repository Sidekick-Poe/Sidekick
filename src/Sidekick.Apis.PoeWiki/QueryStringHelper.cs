using System;
using System.Collections.Generic;
using System.Web;

namespace Sidekick.Apis.PoeWiki;

public static class QueryStringHelper
{
    public static string ToQueryString(List<KeyValuePair<string, string>> parameters)
    {
        if (parameters == null || parameters.Count == 0)
        {
            return string.Empty;
        }

        var queryString = new List<string>();
        foreach (var parameter in parameters)
        {
            // Encode both the key and value to ensure proper URL encoding
            var encodedKey = HttpUtility.UrlEncode(parameter.Key);
            var encodedValue = HttpUtility.UrlEncode(parameter.Value);

            // Combine key and value as a query parameter
            queryString.Add($"{encodedKey}={encodedValue}");
        }

        // Join all parameters with '&' and prepend a '?' for query string
        return "?" + string.Join("&", queryString);
    }

    // Overload for dictionary input
    public static string ToQueryString(Dictionary<string, string> parameters)
    {
        if (parameters == null || parameters.Count == 0)
        {
            return string.Empty;
        }

        var queryString = new List<KeyValuePair<string, string>>();
        foreach (var parameter in parameters)
        {
            queryString.Add(new KeyValuePair<string, string>(parameter.Key, parameter.Value));
        }

        return ToQueryString(queryString);
    }
}
