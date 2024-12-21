using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Bulk.Models;
using Sidekick.Apis.Poe.Bulk.Results;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Trade.Requests;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Bulk
{
    public class BulkTradeService(
        ILogger<BulkTradeService> logger,
        IGameLanguageProvider gameLanguageProvider,
        ISettingsService settingsService,
        IPoeTradeClient poeTradeClient,
        IFilterProvider filterProvider,
        IItemStaticDataProvider itemStaticDataProvider) : IBulkTradeService
    {
        private readonly ILogger logger = logger;

        public bool SupportsBulkTrade(Item? item)
        {
            return item?.Metadata.Rarity == Rarity.Currency && itemStaticDataProvider.GetId(item.Metadata) != null;
        }

        public async Task<BulkResponseModel> SearchBulk(Item item)
        {
            logger.LogInformation("[Trade API] Querying Exchange API.");

            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var uri = $"{await GetBaseApiUrl(item.Metadata.Game)}exchange/{leagueId.GetUrlSlugForLeague()}";

            var itemId = itemStaticDataProvider.GetId(item.Metadata);
            if (itemId == null)
            {
                throw new ApiErrorException { AdditionalInformation = ["Sidekick could not find a valid item."], };
            }

            var currency = item.Metadata.Game == GameType.PathOfExile ? await settingsService.GetString(SettingKeys.PriceCheckBulkCurrency) : await settingsService.GetString(SettingKeys.PriceCheckBulkCurrencyPoE2);
            currency = filterProvider.GetPriceOption(currency);
            var minStock = await settingsService.GetInt(SettingKeys.PriceCheckBulkMinimumStock);

            var model = new BulkQueryRequest();
            model.Query.Want.Add(itemId);
            model.Query.Minimum = minStock;

            if (currency == null || currency == "chaos_divine")
            {
                if (item.Metadata.Game == GameType.PathOfExile)
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

            var json = JsonSerializer.Serialize(model, poeTradeClient.Options);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await poeTradeClient.HttpClient.PostAsync(uri, body);

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var result = JsonSerializer.Deserialize<BulkResponse?>(content, poeTradeClient.Options);
                if (result == null)
                {
                    throw new ApiErrorException();
                }

                return new BulkResponseModel(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An exception occured while parsing the API response. {data}", content);
                throw new ApiErrorException();
            }
        }

        public async Task<Uri> GetTradeUri(Item item, string queryId)
        {
            var baseUri = new Uri(await GetBaseUrl(item.Metadata.Game) + "exchange/");
            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            return new Uri(baseUri, $"{leagueId.GetUrlSlugForLeague()}/{queryId}");
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
}
