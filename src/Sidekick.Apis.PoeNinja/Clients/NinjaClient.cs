using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoeNinja.Clients;

/// <summary>
/// Only for POE2.
/// https://poe2scout.com/api/swagger
/// </summary>
public class NinjaClient
(
    ISettingsService settingsService,
    IHttpClientFactory httpClientFactory,
    ILogger<NinjaClient> logger
) : INinjaClient
{
    private static readonly Uri apiBaseUrl = new("https://poe.ninja/");

    public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
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
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        return client;
    }

    public async Task<TResponse?> Fetch<TResponse>(GameType game, string path, Dictionary<string, string?>? parameters = null)
        where TResponse : class
    {
        parameters ??= new();

        var gamePath = game == GameType.PathOfExile ? "poe1/api/" : "poe2/api/";

        var league = await settingsService.GetLeague();
        league = league?.Replace(' ', '+');
        parameters.TryAdd("league", league);

        var query = string.Join("&", parameters.Select(x => x.Key + "=" + Uri.EscapeDataString(x.Value?.ToString() ?? string.Empty)));
        var url = new Uri($"{apiBaseUrl}{gamePath}{path}?{query}");

        try
        {
            using var client = GetHttpClient();
            var response = await client.GetAsync(url);
            var responseStream = await response.Content.ReadAsStreamAsync();
            var results = await JsonSerializer.DeserializeAsync<TResponse>(responseStream, JsonSerializerOptions);
            if (results != null) return results;

            logger.LogError("[PoeNinja] Could not fetch items from poe.ninja");
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PoeNinja] Could not fetch items from poe.ninja");
        }

        return null;
    }
}
