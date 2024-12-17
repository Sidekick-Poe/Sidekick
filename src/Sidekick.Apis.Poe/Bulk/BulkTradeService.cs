using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Bulk.Models;
using Sidekick.Apis.Poe.Bulk.Results;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Enums;
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
        IItemStaticDataProvider itemStaticDataProvider) : IBulkTradeService
    {
        private readonly ILogger logger = logger;

        public bool SupportsBulkTrade(Item? item)
        {
            return item?.Metadata.Rarity == Rarity.Currency && itemStaticDataProvider.GetId(item.Metadata) != null;
        }

        public async Task<BulkResponseModel> SearchBulk(Item item, TradeCurrency currency, int minStock)
        {
            logger.LogInformation("[Trade API] Querying Exchange API.");

            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var uri = $"{await GetBaseApiUrl(item.Metadata.Game)}exchange/{leagueId.GetUrlSlugForLeague()}";

            var itemId = itemStaticDataProvider.GetId(item.Metadata);
            if (itemId == null)
            {
                throw new ApiErrorException("[Trade API] Could not find a valid item.");
            }

            var model = new BulkQueryRequest();
            model.Query.Want.Add(itemId);
            model.Query.Minimum = minStock;

            if (currency == TradeCurrency.ChaosEquivalent || currency == TradeCurrency.ChaosOrDivine)
            {
                if (model.Query.Want.Any(x => x == TradeCurrency.Chaos.GetValueAttribute()))
                {
                    model.Query.Have.Add(TradeCurrency.Divine.GetValueAttribute()!);
                }
                else
                {
                    model.Query.Have.Add(TradeCurrency.Chaos.GetValueAttribute()!);
                }
            }
            else
            {
                var have = currency.GetValueAttribute();
                model.Query.Have.Add(have!);
            }

            var json = JsonSerializer.Serialize(model, poeTradeClient.Options);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await poeTradeClient.HttpClient.PostAsync(uri, body);

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("[Trade API] Querying failed: {responseCode} {responseMessage}", response.StatusCode, content);
                logger.LogWarning("[Trade API] Uri: {uri}", uri);
                logger.LogWarning("[Trade API] Query: {query}", json);

                var errorResult = JsonSerializer.Deserialize<ErrorResult>(content, poeTradeClient.Options);
                throw new ApiErrorException(errorResult?.Error?.Message);
            }

            try
            {
                var result = JsonSerializer.Deserialize<BulkResponse?>(content, poeTradeClient.Options);
                if (result == null)
                {
                    throw new ApiErrorException("[Trade API] Could not understand the API response.");
                }

                return new BulkResponseModel(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An exception occured while parsing the API response. {data}", content);
                throw new ApiErrorException("[Trade API] An exception occured while parsing the API response.");
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
