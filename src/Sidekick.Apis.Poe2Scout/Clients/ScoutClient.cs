using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Common.Settings;

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
        var league = await settingsService.GetLeague();
        parameters.TryAdd("league", league);

        var query = string.Join("&",
                                parameters.Select((x) => x.Key + "=" +
                                                         Uri.EscapeDataString(x.Value?.ToString() ?? string.Empty)));
        var url = new Uri($"{apiBaseUrl}{path}?{query}");

        try
        {
            using var client = GetHttpClient();
            var response = await client.GetAsync(url);
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
}
