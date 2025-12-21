using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiStatic;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Trade.Bulk.Models;
using Sidekick.Apis.Poe.Trade.Trade.Bulk.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Bulk.Results;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Bulk;

public class BulkTradeService
(
    ILogger<BulkTradeService> logger,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    IApiStaticDataProvider apiStaticDataProvider,
    IHttpClientFactory httpClientFactory
) : IBulkTradeService
{
    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public bool SupportsBulkTrade(Item? item)
    {
        return item?.Properties.Rarity == Rarity.Currency && apiStaticDataProvider.Get(item.Name, item.Type) != null;
    }

    public async Task<BulkResponseModel> SearchBulk(Item item)
    {
        logger.LogInformation("[Trade API] Querying Exchange API.");

        var league = await settingsService.GetLeague();
        var uri = $"{await GetBaseApiUrl(item.Game)}exchange/{league}";

        var staticItem = apiStaticDataProvider.Get(item.Name, item.Type);
        if (staticItem == null)
        {
            throw new ApiErrorException
            {
                AdditionalInformation = ["Sidekick could not find a valid item."],
            };
        }

        var currency = item.Game == GameType.PathOfExile1 ? await settingsService.GetString(CurrencyFilter.SettingKeyPoe1) : await settingsService.GetString(CurrencyFilter.SettingKeyPoe2);
        var minStock = await settingsService.GetInt(SettingKeys.PriceCheckBulkMinimumStock);

        var model = new BulkQueryRequest();
        model.Query.Want.Add(staticItem.Id);
        model.Query.Minimum = minStock;

        if (currency == null || currency == "chaos_divine" || currency == "exalted_divine")
        {
            if (item.Game == GameType.PathOfExile1)
            {
                model.Query.Have.Add(model.Query.Want.Any(x => x == "chaos") ? "divine" : "chaos");
            }
            else
            {
                model.Query.Have.Add(model.Query.Want.Any(x => x == "exalted") ? "divine" : "exalted");
            }
        }
        else
        {
            model.Query.Have.Add(currency);
        }

        // Trade Settings
        model.Query.Status.Option = PlayerStatusFilterFactory.OnlineLeague;

        var json = JsonSerializer.Serialize(model, JsonSerializerOptions);
        using var body = new StringContent(json, Encoding.UTF8, "application/json");
        using var httpClient = httpClientFactory.CreateClient(TradeApiClient.ClientName);
        var response = await httpClient.PostAsync(uri, body);
        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var result = JsonSerializer.Deserialize<BulkResponse?>(content, JsonSerializerOptions);
            if (result == null)
            {
                throw new ApiErrorException();
            }

            return new BulkResponseModel(result);
        }
        catch (SidekickException)
        {
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception occurred while parsing the API response. {data}", content);
            throw new ApiErrorException();
        }
    }

    public async Task<Uri> GetTradeUri(Item item, string queryId)
    {
        var baseUri = new Uri(await GetBaseUrl(item.Game) + "exchange/");
        var league = await settingsService.GetLeague();
        return new Uri(baseUri, $"{league}/{queryId}");
    }

    private Task<string> GetBaseApiUrl(GameType game)
    {
        return Task.FromResult(gameLanguageProvider.Language.GetTradeApiBaseUrl(game));
    }

    private Task<string> GetBaseUrl(GameType game)
    {
        return Task.FromResult(gameLanguageProvider.Language.GetTradeBaseUrl(game));
    }
}
