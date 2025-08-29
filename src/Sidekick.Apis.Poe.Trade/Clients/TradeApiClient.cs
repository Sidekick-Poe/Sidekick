using System.Text.Json;
using System.Text.Json.Serialization;
using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Trade.Clients;

public class TradeApiClient
(
    IHttpClientFactory httpClientFactory
) : ITradeApiClient
{
    public const string ClientName = "PoeTradeClient";

    public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task<FetchResult<TReturn>> Fetch<TReturn>(GameType game, IGameLanguage language, string path)
    {
        using var httpClient = httpClientFactory.CreateClient(ClientName);
        var response = await httpClient.GetAsync(language.GetTradeApiBaseUrl(game) + path);
        var content = await response.Content.ReadAsStreamAsync();

        var result = await JsonSerializer.DeserializeAsync<FetchResult<TReturn>>(content, JsonSerializerOptions);
        if (result == null) throw new SidekickException("[Trade Client] Could not understand the API response.");

        return result;
    }

    public async Task<Stream> Fetch(GameType game, IGameLanguage language, string path)
    {
        using var httpClient = httpClientFactory.CreateClient(ClientName);
        var response = await httpClient.GetAsync(language.GetTradeApiBaseUrl(game) + path);
        return await response.Content.ReadAsStreamAsync();
    }
}
