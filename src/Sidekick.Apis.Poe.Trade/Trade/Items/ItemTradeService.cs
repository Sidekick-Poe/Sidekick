using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Models;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
namespace Sidekick.Apis.Poe.Trade.Trade.Items;

public class ItemTradeService
(
    ILogger<ItemTradeService> logger,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    IHttpClientFactory httpClientFactory
) : IItemTradeService
{
    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public async Task<TradeSearchResult<string>> Search(Item item, List<TradeFilter>? filters = null)
    {
        try
        {
            logger.LogInformation("[Trade API] Querying Trade API.");

            var query = new Query();

            if (!string.IsNullOrEmpty(item.Definition.TradeItem?.Discriminator))
            {
                query.Type = new TypeDiscriminator()
                {
                    Option = item.Definition.TradeItem.Type,
                    Discriminator = item.Definition.TradeItem.Discriminator,
                };
            }
            else
            {
                query.Type = item.Definition.TradeItem?.Type;
            }

            if (item.Definition.TradeItem?.Category == "monster" && !string.IsNullOrEmpty(item.Definition.TradeItem?.Name))
            {
                query.Term = item.Definition.TradeItem.Name;
                query.Type = null;
            }
            else if (item.Definition.UniqueItem != null && !string.IsNullOrEmpty(item.Definition.TradeItem?.Name))
            {
                query.Name = item.Definition.TradeItem.Name;
            }

            foreach (var filter in filters ?? [])
            {
                filter.PrepareTradeRequest(query, item);
            }

            var league = await settingsService.GetLeague();
            var uri = new Uri($"{currentGameLanguage.Language.GetTradeApiBaseUrl(item.Game)}search/{league}");

            var request = new QueryRequest()
            {
                Query = query,
            };
            var json = JsonSerializer.Serialize(request, JsonSerializerOptions);

            using var body = new StringContent(json, Encoding.UTF8, "application/json");
            using var httpClient = httpClientFactory.CreateClient(TradeApiClient.ClientName);
            var response = await httpClient.PostAsync(uri, body);

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TradeSearchResult<string>?>(content, JsonSerializerOptions);
            if (result != null)
            {
                return result;
            }
        }
        catch (SidekickException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[Trade API] Exception thrown while querying trade api.");
        }

        throw new ApiErrorException();
    }

    public async Task<List<TradeResult>> GetResults(GameType game, string queryId, List<string> ids)
    {
        try
        {
            logger.LogInformation($"[Trade API] Fetching Trade API Listings from Query {queryId}.");

            using var httpClient = httpClientFactory.CreateClient(TradeApiClient.ClientName);
            var response = await httpClient.GetAsync(currentGameLanguage.Language.GetTradeApiBaseUrl(game) + "fetch/" + string.Join(",", ids) + "?query=" + queryId);
            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<FetchResult<TradeResult?>>(content, JsonSerializerOptions);
            if (result == null)
            {
                return [];
            }

            return result.Result.Where(x => x != null).ToList()!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[Trade API] Exception thrown when fetching trade API listings from Query {queryId}.");
            throw new SidekickException("Sidekick could not fetch the listings from the trade API.");
        }
    }

    public async Task<Uri> GetTradeUri(GameType game, string queryId)
    {
        var baseUri = new Uri(currentGameLanguage.Language.GetTradeBaseUrl(game) + "search/");
        var league = await settingsService.GetLeague();
        return new Uri(baseUri, $"{league}/{queryId}");
    }
}
