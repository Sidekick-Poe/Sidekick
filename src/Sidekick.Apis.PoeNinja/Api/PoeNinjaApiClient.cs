using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.PoeNinja.Api.Models;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Apis.PoeNinja.Repository;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoeNinja.Api
{
    /// <summary>
    /// https://poe.ninja/swagger
    /// </summary>
    public class PoeNinjaApiClient : IPoeNinjaApiClient
    {
        private static readonly Uri POE_NINJA_API_BASE_URL = new("https://poe.ninja/api/data/");

        private readonly HttpClient client;
        private readonly ILogger logger;
        private readonly ISettings settings;
        private readonly IPoeNinjaRepository poeNinjaRepository;
        private readonly JsonSerializerOptions options;

        public PoeNinjaApiClient(
            IHttpClientFactory httpClientFactory,
            ILogger<PoeNinjaClient> logger,
            ISettings settings,
            IPoeNinjaRepository poeNinjaRepository)
        {
            this.logger = logger;
            this.settings = settings;
            this.poeNinjaRepository = poeNinjaRepository;
            options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        }

        public async Task<List<NinjaPrice>> FetchPrices(ItemType itemType)
        {
            List<NinjaPrice>? items;

            if (itemType == ItemType.Currency || itemType == ItemType.Fragment)
            {
                var result = await FetchCurrencies(itemType);
                items = result;
            }
            else
            {
                var result = await FetchItems(itemType);
                items = result;
            }

            if (items != null)
            {
                await poeNinjaRepository.SavePrices(itemType, items);
            }

            return items ?? new();
        }

        private async Task<List<NinjaPrice>?> FetchItems(ItemType itemType)
        {
            var url = new Uri($"{POE_NINJA_API_BASE_URL}itemoverview?league={settings.LeagueId}&type={itemType}");

            try
            {
                var response = await client.GetAsync(url);
                var responseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<PoeNinjaQueryResult<PoeNinjaItem>>(responseStream, options);
                if (result == null)
                {
                    return null;
                }

                return result.Lines
                        .Select(x => new NinjaPrice()
                        {
                            Corrupted = x.Corrupted,
                            Price = x.ChaosValue,
                            LastUpdated = DateTimeOffset.Now,
                            Name = x.Name,
                            MapTier = x.MapTier,
                            GemLevel = x.GemLevel,
                            DetailsId = x.DetailsId,
                            ItemType = itemType,
                            SparkLine = x.SparkLine ?? x.LowConfidenceSparkLine,
                            IsRelic = x.ItemClass == 9, // 3 for Unique, 9 for Relic Unique.
                            Links = x.Links,
                        })
                        .ToList();
            }
            catch (Exception)
            {
                logger.LogWarning("[PoeNinja] Could not fetch {itemType} from poe.ninja", itemType);
            }

            return null;
        }

        private async Task<List<NinjaPrice>?> FetchCurrencies(ItemType itemType)
        {
            var url = new Uri($"{POE_NINJA_API_BASE_URL}currencyoverview?league={settings.LeagueId}&type={itemType}");

            try
            {
                var response = await client.GetAsync(url);
                var responseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<PoeNinjaQueryResult<PoeNinjaCurrency>>(responseStream, options);
                if (result == null)
                {
                    return null;
                }

                return result.Lines
                        .Where(x => x.Receive?.Value != null)
                        .Select(x => new NinjaPrice()
                        {
                            Corrupted = false,
                            Price = x.Receive.Value,
                            LastUpdated = DateTimeOffset.Now,
                            Name = x.CurrencyTypeName,
                            DetailsId = x.DetailsId,
                            ItemType = itemType,
                            SparkLine = x.ReceiveSparkLine ?? x.LowConfidenceReceiveSparkLine,
                        })
                        .ToList();
            }
            catch
            {
                logger.LogInformation("[PoeNinja]Could not fetch {itemType} from poe.ninja", itemType);
            }

            return null;
        }
    }
}
