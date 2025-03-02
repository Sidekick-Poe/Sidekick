using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Clients;

public class PoeTradeClient(
    ILogger<PoeTradeClient> logger,
    IHttpClientFactory httpClientFactory) : IPoeTradeClient
{
    public JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task<FetchResult<TReturn>> Fetch<TReturn>(GameType game, IGameLanguage language, string path)
    {
        var name = typeof(TReturn).Name;

        try
        {
            using var httpClient = httpClientFactory.CreateClient(ClientNames.TradeClient);
            var response = await httpClient.GetAsync(language.GetTradeApiBaseUrl(game) + path);
            if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.MovedPermanently)
            {
                var redirectUrl = response.Headers.Location?.ToString();
                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    // Follow redirect manually
                    response = await httpClient.GetAsync(redirectUrl);
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                logger.LogError($"[Trade Client] Response Error! Status: {response.StatusCode}, Content: {contentString}");
                throw new Exception("[Trade Client] Could not understand the API response.");
            }

            var content = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<FetchResult<TReturn>>(content, Options);
            if (result == null)
            {
                throw new Exception($"[Trade Client] Could not understand the API response.");
            }

            return result;
        }
        catch (Exception)
        {
            logger.LogInformation($"[Trade Client] Could not fetch {name} at {language.GetTradeApiBaseUrl(game) + path}.");
            throw;
        }
    }
}
