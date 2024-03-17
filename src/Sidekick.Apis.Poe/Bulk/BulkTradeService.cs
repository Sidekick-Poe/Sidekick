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
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Bulk
{
    public class BulkTradeService : IBulkTradeService
    {
        private readonly ILogger logger;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly ISettings settings;
        private readonly IPoeTradeClient poeTradeClient;
        private readonly IItemStaticDataProvider itemStaticDataProvider;

        public BulkTradeService(ILogger<BulkTradeService> logger,
            IGameLanguageProvider gameLanguageProvider,
            ISettings settings,
            IPoeTradeClient poeTradeClient,
            IItemStaticDataProvider itemStaticDataProvider)
        {
            this.logger = logger;
            this.gameLanguageProvider = gameLanguageProvider;
            this.settings = settings;
            this.poeTradeClient = poeTradeClient;
            this.itemStaticDataProvider = itemStaticDataProvider;
        }

        public bool SupportsBulkTrade(Item item)
        {
            return item.Metadata.Rarity == Rarity.Currency && itemStaticDataProvider.GetId(item.Metadata.Name, item.Metadata.Type) != null;
        }

        public async Task<BulkResponseModel> SearchBulk(Item item, TradeCurrency currency, int minStock)
        {
            logger.LogInformation("[Trade API] Querying Exchange API.");

            if (gameLanguageProvider.Language == null)
            {
                throw new ApiErrorException("[Trade API] Could not find a valid language.");
            }

            var uri = $"{gameLanguageProvider.Language.PoeTradeApiBaseUrl}exchange/{settings.LeagueId}";

            var itemId = itemStaticDataProvider.GetId(item.Metadata.Name, item.Metadata.Type);
            if (itemId == null)
            {
                throw new ApiErrorException("[Trade API] Could not find a valid item.");
            }

            var model = new BulkQueryRequest();
            model.Query.Want.Add(itemId);
            model.Query.Minimum = minStock;

            var have = currency.GetValueAttribute();
            if (have == null || currency == TradeCurrency.ChaosEquivalent || currency == TradeCurrency.ChaosOrDivine)
            {
                model.Query.Have.Add(TradeCurrency.Chaos.GetValueAttribute()!);
            }
            else
            {
                model.Query.Have.Add(have);
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

        public Uri GetTradeUri(Item item, string queryId)
        {
            Uri? baseUri = gameLanguageProvider.Language?.PoeTradeExchangeBaseUrl;
            if (baseUri == null)
            {
                throw new Exception("[Trade API] Could not find the trade uri.");
            }

            return new Uri(baseUri, $"{settings.LeagueId}/{queryId}");
        }
    }
}
