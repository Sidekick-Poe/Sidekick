using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Bulk.Models;
using Sidekick.Apis.Poe.Trade.Bulk.Results;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Static;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Bulk;

public class BulkTradeService
(
    ILogger<BulkTradeService> logger,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    IFilterProvider filterProvider,
    IItemStaticDataProvider itemStaticDataProvider,
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
        return item?.Header.Rarity == Rarity.Currency && itemStaticDataProvider.Get(item.Header) != null;
    }

    public async Task<BulkResponseModel> SearchBulk(Item item)
    {
        logger.LogInformation("[Trade API] Querying Exchange API.");

        var league = await settingsService.GetLeague();
        var uri = $"{await GetBaseApiUrl(item.Header.Game)}exchange/{league}";

        var staticItem = itemStaticDataProvider.Get(item.Header);
        if (staticItem == null)
        {
            throw new ApiErrorException
            {
                AdditionalInformation = ["Sidekick could not find a valid item."],
            };
        }

        var currency = item.Header.Game == GameType.PathOfExile ? await settingsService.GetString(SettingKeys.PriceCheckCurrency) : await settingsService.GetString(SettingKeys.PriceCheckCurrencyPoE2);
        currency = filterProvider.GetPriceOption(currency);
        var minStock = await settingsService.GetInt(SettingKeys.PriceCheckBulkMinimumStock);

        var model = new BulkQueryRequest();
        model.Query.Want.Add(staticItem.Id);
        model.Query.Minimum = minStock;

        if (currency == null || currency == "chaos_divine" || currency == "exalted_divine")
        {
            if (item.Header.Game == GameType.PathOfExile)
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
        var statusKey = item.Header.Game == GameType.PathOfExile ? SettingKeys.PriceCheckStatusPoE1 : SettingKeys.PriceCheckStatusPoE2;
        var status = await settingsService.GetString(statusKey);
        if (status == Status.Securable || status == Status.Available) status = Status.OnlineLeague;
        model.Query.Status.Option = status ?? Status.OnlineLeague;

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
        var baseUri = new Uri(await GetBaseUrl(item.Header.Game) + "exchange/");
        var league = await settingsService.GetLeague();
        return new Uri(baseUri, $"{league}/{queryId}");
    }

    private async Task<string> GetBaseApiUrl(GameType game)
    {
        var useInvariant = await settingsService.GetBool(SettingKeys.UseInvariantTradeResults);
        return useInvariant ? gameLanguageProvider.InvariantLanguage.GetTradeApiBaseUrl(game) : gameLanguageProvider.Language.GetTradeApiBaseUrl(game);
    }

    private async Task<string> GetBaseUrl(GameType game)
    {
        var useInvariant = await settingsService.GetBool(SettingKeys.UseInvariantTradeResults);
        return useInvariant ? gameLanguageProvider.InvariantLanguage.GetTradeBaseUrl(game) : gameLanguageProvider.Language.GetTradeBaseUrl(game);
    }
}
