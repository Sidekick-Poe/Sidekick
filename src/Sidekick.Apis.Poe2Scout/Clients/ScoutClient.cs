using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data.Extensions;

namespace Sidekick.Apis.Poe2Scout.Clients;

/// <summary>
/// Only for POE2.
/// https://poe2scout.com/api/swagger
/// </summary>
public class ScoutClient
(
    ISettingsService settingsService,
    IHttpClientFactory httpClientFactory,
    ILogger<ScoutClient> logger
) : IScoutClient
{
    private static readonly Uri apiBaseUrl = new("https://poe2scout.com/api/");

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    private HttpClient GetHttpClient()
    {
        var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(15);
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        return client;
    }

    public async Task<TResponse?> Fetch<TResponse>(string path, Dictionary<string, string?>? parameters = null)
    where TResponse : class
    {
        parameters ??= new();
        var game = await settingsService.GetGame();
        var league = await settingsService.GetLeague();
        if (string.IsNullOrEmpty(league))
        {
            logger.LogError("[Poe2Scout] Could not fetch {Path}: no league is selected", path);
            return null;
        }

        path = BuildPath(path, game.GetValueAttribute(), league, parameters);

        var query = string.Join("&",
                                parameters
                                    .Where((x) => x.Value != null)
                                    .Select((x) => x.Key + "=" + Uri.EscapeDataString(x.Value ?? string.Empty)));

        var url = new Uri(query.Length > 0 ? $"{apiBaseUrl}{path}?{query}" : $"{apiBaseUrl}{path}");

        try
        {
            using var client = GetHttpClient();
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                logger.LogError("[Poe2Scout] Could not fetch {Url}. StatusCode={StatusCode}. Body={Body}", url, response.StatusCode, responseBody);
                return null;
            }

            var responseStream = await response.Content.ReadAsStreamAsync();
            var results = await JsonSerializer.DeserializeAsync<TResponse>(responseStream, JsonSerializerOptions);
            if (results != null) return results;

            logger.LogError("[Poe2Scout] Could not fetch items from poe2scout.com");
        }
        catch (Exception e)
        {
            logger.LogError(e, "[Poe2Scout] Could not fetch items from poe2scout.com");
        }

        return null;
    }

    private static string BuildPath(string path, string game, string league, Dictionary<string, string?> parameters)
    {
        if (path.Equals("items/categories", StringComparison.OrdinalIgnoreCase))
        {
            parameters.TryAdd("LeagueName", league);
            return $"{game}/Items/Categories";
        }

        if (path.Equals("items", StringComparison.OrdinalIgnoreCase))
        {
            return $"{game}/Leagues/{Uri.EscapeDataString(league)}/Items";
        }

        if (path.StartsWith("items/", StringComparison.OrdinalIgnoreCase) &&
            path.EndsWith("/history", StringComparison.OrdinalIgnoreCase))
        {
            parameters["LogCount"] = TakeParameter(parameters, "logCount", "LogCount") ?? "24";

            var referenceCurrency = TakeParameter(parameters, "referenceCurrency", "ReferenceCurrency");
            if (!string.IsNullOrEmpty(referenceCurrency))
            {
                parameters["ReferenceCurrency"] = referenceCurrency;
            }

            var itemId = path.Substring("items/".Length, path.Length - "items/".Length - "/history".Length);
            return $"{game}/Leagues/{Uri.EscapeDataString(league)}/Items/{Uri.EscapeDataString(itemId)}/History";
        }

        if (path.Equals("currencyExchange/PairHistory", StringComparison.OrdinalIgnoreCase))
        {
            var currencyOneItemId = TakeParameter(parameters, "currencyOneItemId", "CurrencyOneItemId");
            var currencyTwoItemId = TakeParameter(parameters, "currencyTwoItemId", "CurrencyTwoItemId");
            parameters["Limit"] = TakeParameter(parameters, "limit", "Limit") ?? "24";

            return $"{game}/Leagues/{Uri.EscapeDataString(league)}/Currencies/Pairs/{currencyOneItemId}/{currencyTwoItemId}/History";
        }

        return path;
    }

    private static string? TakeParameter(Dictionary<string, string?> parameters, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (parameters.Remove(key, out var value))
            {
                return value;
            }
        }

        return null;
    }
}
