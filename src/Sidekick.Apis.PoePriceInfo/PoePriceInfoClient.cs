using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.PoePriceInfo.Api;
using Sidekick.Apis.PoePriceInfo.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoePriceInfo
{
    public class PoePriceInfoClient : IPoePriceInfoClient
    {
        private readonly JsonSerializerOptions options;
        private readonly HttpClient client;
        private readonly ISettings settings;
        private readonly ILogger<PoePriceInfoClient> logger;

        public PoePriceInfoClient(
            ISettings settings,
            ILogger<PoePriceInfoClient> logger,
            IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://www.poeprices.info/api");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
            client.Timeout = TimeSpan.FromSeconds(60);
            options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            this.settings = settings;
            this.logger = logger;
        }

        public async Task<PricePrediction?> GetPricePrediction(Item item)
        {
            if (item.Metadata.Rarity != Rarity.Rare || item.Original.Text == null)
            {
                return null;
            }

            try
            {
                var encodedItem = Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Original.Text));
                var response = await client.GetAsync("?l=" + settings.LeagueId + "&i=" + encodedItem);
                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<PriceInfoResult>(content, options);

                if (result == null)
                {
                    return null;
                }

                if (result.Min == 0 && result.Max == 0)
                {
                    return null;
                }

                return new PricePrediction()
                {
                    ConfidenceScore = result.ConfidenceScore,
                    Currency = result.Currency,
                    Max = result.Max ?? 0,
                    Min = result.Min ?? 0,
                };
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error while trying to get price prediction from poeprices.info.");
            }

            return null;
        }
    }
}
