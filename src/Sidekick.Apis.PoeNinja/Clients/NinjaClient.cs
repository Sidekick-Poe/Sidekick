using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Game;
using Sidekick.Game.Providers;

namespace Sidekick.Apis.PoeNinja.Clients;

/// <summary>
/// Only for POE2.
/// https://poe2scout.com/api/swagger
/// </summary>
public class NinjaClient
(
    IHttpClientFactory httpClientFactory,
    ILogger<NinjaClient> logger,
    LeagueProvider leagueProvider
) : INinjaClient
{
    private static readonly Uri ApiBaseUrl = new("https://poe.ninja/");

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
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        return client;
    }

    public async Task<TResponse?> Fetch<TResponse>(GameType game, string path, Dictionary<string, string?>? parameters = null)
        where TResponse : class
    {
        parameters ??= new();

        var gamePath = game == GameType.Poe1 ? "poe1/api/" : "poe2/api/";

        var leagueValue = leagueProvider.Current.Id.Replace(' ', '+');
        parameters.TryAdd("league", leagueValue);

        var query = string.Join("&", parameters.Select(x => x.Key + "=" + x.Value?.ToString()));
        var url = new Uri($"{ApiBaseUrl}{gamePath}{path}?{query}");

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
