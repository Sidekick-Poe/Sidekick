using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;

namespace Sidekick.Apis.Poe.Trade.Clients;

public class TradeApiClient
(
    IHttpClientFactory httpClientFactory,
    ILogger<TradeApiClient> logger
) : ITradeApiClient
{
    public const string ClientName = "PoeTradeClient";

    public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public static string GetDataFileName(GameType game, IGameLanguage language, string path)
    {
        return $"{game.GetValueAttribute()}.{language.Code}.{path}.json";
    }

    public async Task<FetchResult<TReturn>> FetchData<TReturn>(GameType game, IGameLanguage language, string path)
    {
        try
        {
            using var httpClient = httpClientFactory.CreateClient(ClientName);
            var response = await httpClient.GetAsync(language.GetTradeApiBaseUrl(game) + "data/" + path);
            var content = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<FetchResult<TReturn>>(content, JsonSerializerOptions);
            if (result != null && result.Result.Count != 0) return result;

            logger.LogWarning("[Trade Client] Failed to parse the API response.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "[Trade Client] Failed to parse the API response.");
        }

        var dataFilePath = Path.Combine(AppContext.BaseDirectory, "wwwroot/data/" + GetDataFileName(game, language, path));
        logger.LogInformation("[Trade Client] Attempting to load data from local file: {dataFilePath}", dataFilePath);

        // Read from the local file as a fallback
        if (!File.Exists(dataFilePath))
        {
            throw new SidekickException("[Trade Client] Could not understand the API response.");
        }

        try
        {
            await using var fileStream = File.OpenRead(dataFilePath);
            var result = await JsonSerializer.DeserializeAsync<FetchResult<TReturn>>(fileStream, JsonSerializerOptions);
            if (result != null && result.Result.Count > 0)
            {
                logger.LogInformation("[Trade Client] Successfully loaded data from local file.");
                return result;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Trade Client] Failed to load data from local file.");
        }

        throw new SidekickException("[Trade Client] Could not understand the API response. Could not read data from local data file.");
    }

    public async Task<Stream> FetchData(GameType game, IGameLanguage language, string path)
    {
        using var httpClient = httpClientFactory.CreateClient(ClientName);
        var response = await httpClient.GetAsync(language.GetTradeApiBaseUrl(game) + "data/" + path);
        return await response.Content.ReadAsStreamAsync();
    }
}
