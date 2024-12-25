using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.PoeNinja.Api;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoeNinja
{
    /// <summary>
    /// https://poe.ninja/swagger
    /// </summary>
    public class PoeNinjaClient : IPoeNinjaClient
    {
        private static readonly Uri baseUrl = new("https://poe.ninja/");
        private static readonly Uri apiBaseUrl = new("https://poe.ninja/api/data/");

        private readonly ICacheProvider cacheProvider;
        private readonly ISettingsService settingsService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<PoeNinjaClient> logger;
        private readonly JsonSerializerOptions options;

        public PoeNinjaClient(
            ICacheProvider cacheProvider,
            ISettingsService settingsService,
            IHttpClientFactory httpClientFactory,
            ILogger<PoeNinjaClient> logger)
        {
            this.cacheProvider = cacheProvider;
            this.settingsService = settingsService;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;

            options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        private HttpClient GetHttpClient()
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
            return client;
        }

        public async Task<NinjaPrice?> GetPriceInfo(
            string? englishName,
            string? englishType,
            Category category,
            int? gemLevel = null,
            int? mapTier = null,
            bool? isRelic = false,
            int? numberOfLinks = null)
        {
            if ((englishName ?? englishType) == "Chaos Orb")
            {
                return new NinjaPrice()
                {
                    BaseType = "Chaos Orb",
                    LastUpdated = DateTimeOffset.Now,
                    Name = "Chaos Orb",
                    Price = 1,
                };
            }

            await ClearCacheIfExpired();
            var prices = await GetPrices(category);

            var query = prices.Where(x => x.Name == englishName || x.Name == englishType);

            if (category == Category.Gem && gemLevel != null)
            {
                query = query.Where(x => x.GemLevel == gemLevel);
            }

            if (isRelic != null)
            {
                query = query.Where(x => x.IsRelic == isRelic);
            }

            if (category == Category.Map && mapTier != null)
            {
                query = query.Where(x => x.MapTier == mapTier);
            }

            if (numberOfLinks != null)
            {
                // Poe.ninja has pricings for <5, 5 and 6 links.
                // <5 being 0 links in their API.
                query = query.Where(x => x.Links == (numberOfLinks >= 5 ? numberOfLinks : 0));
            }

            return query.MinBy(x => x.Corrupted);
        }

        public async Task<NinjaPrice?> GetClusterPrice(
            string englishGrantText,
            int passiveCount,
            int itemLevel)
        {
            var normalizedItemLevel = itemLevel switch
            {
                >= 84 => 84,
                >= 75 => 75,
                >= 68 => 68,
                >= 50 => 50,
                _ => 1,
            };

            await ClearCacheIfExpired();
            var prices = await GetPrices(Category.Jewel);

            var query = prices
                .Where(x => x.Name == englishGrantText)
                .Where(x => x.ItemLevel == normalizedItemLevel)
                .Where(x => x.SmallPassiveCount == passiveCount);

            return query.FirstOrDefault();
        }

        public async Task<Uri> GetDetailsUri(NinjaPrice ninjaPrice)
        {
            if (string.IsNullOrWhiteSpace(ninjaPrice.DetailsId))
            {
                return baseUrl;
            }

            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            return new Uri(baseUrl, $"{GetLeagueUri(leagueId)}/{ninjaPrice.ItemType.GetValueAttribute()}/{ninjaPrice.DetailsId}");

        }

        /// <summary>
        /// Get Poe.ninja's league uri from POE's API league id.
        /// </summary>
        private string GetLeagueUri(string? leagueId)
        {
            leagueId = leagueId.GetUrlSlugForLeague();
            return leagueId switch
            {
                "Standard" => "standard",
                "Hardcore" => "hardcore",
                not null when leagueId.Contains("Hardcore") => "challengehc",
                _ => "challenge"
            };
        }

        private string GetCacheKey(ItemType itemType) => $"PoeNinja_{itemType}";

        private async Task ClearCacheIfExpired()
        {
            var lastClear = await settingsService.GetDateTime(SettingKeys.PoeNinjaLastClear);
            var isCacheTimeValid = lastClear.HasValue && DateTimeOffset.Now - lastClear <= TimeSpan.FromHours(6);
            if (isCacheTimeValid)
            {
                return;
            }

            foreach (var value in Enum.GetValues<ItemType>())
            {
                cacheProvider.Delete(GetCacheKey(value));
            }

            await settingsService.Set(SettingKeys.PoeNinjaLastClear, DateTimeOffset.Now);
        }

        public async Task SaveItemsToCache(ItemType itemType, List<NinjaPrice> prices)
        {
            prices = prices
                .GroupBy(x => (x.Name,
                               x.Corrupted,
                               x.MapTier,
                               x.GemLevel,
                               x.Links))
                .Select(grouping => grouping.OrderBy(x => x.Price).First())
                .ToList();

            await cacheProvider.Set(GetCacheKey(itemType), prices);
        }

        private async Task<IEnumerable<NinjaPrice>> GetPrices(Category category)
        {
            var itemTypes = GetApiItemTypes(category);
            var tasks = new List<Task<IList<NinjaPrice>>>();

            foreach (var itemType in itemTypes)
            {
                tasks.Add(GetItems(itemType));
            }

            var prices = await Task.WhenAll(tasks);
            return prices.SelectMany(x => x);
        }

        private async Task<IList<NinjaPrice>> GetItems(ItemType itemType)
        {
            var cachedItems = await cacheProvider.Get<List<NinjaPrice>>(GetCacheKey(itemType), (cache) => cache.Any());
            if (cachedItems is
                {
                    Count: > 0
                })
            {
                return cachedItems;
            }

            var items = (itemType switch
            {
                ItemType.Currency => await FetchCurrencies(itemType),
                ItemType.Fragment => await FetchCurrencies(itemType),
                _ => await FetchItems(itemType),
            }).ToList();

            if (items.Count != 0)
            {
                await SaveItemsToCache(itemType, items);
            }

            return items;
        }

        private async Task<IEnumerable<NinjaPrice>> FetchItems(ItemType itemType)
        {
            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var url = new Uri($"{apiBaseUrl}itemoverview?league={leagueId.GetUrlSlugForLeague()}&type={itemType}");

            try
            {
                using var client = GetHttpClient();
                var response = await client.GetAsync(url);
                var responseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<PoeNinjaQueryResult<PoeNinjaItem>>(responseStream, options);
                if (result == null)
                {
                    return [];
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
                            BaseType = x.BaseType,
                            ItemLevel = x.ItemLevel,
                            SmallPassiveCount = x.ClusterSmallPassiveCount,
                        });
            }
            catch (Exception)
            {
                logger.LogWarning("[PoeNinja] Could not fetch {itemType} from poe.ninja", itemType);
            }

            return [];
        }

        private async Task<IEnumerable<NinjaPrice>> FetchCurrencies(ItemType itemType)
        {
            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var url = new Uri($"{apiBaseUrl}currencyoverview?league={leagueId.GetUrlSlugForLeague()}&type={itemType}");

            try
            {
                using var client = GetHttpClient();
                var response = await client.GetAsync(url);
                var responseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<PoeNinjaQueryResult<PoeNinjaCurrency>>(responseStream, options);
                if (result == null)
                {
                    return [];
                }

                return result.Lines
                        .Where(x => x.Receive?.Value != null)
                        .Select(x => new NinjaPrice()
                        {
                            Corrupted = false,
                            Price = x.Receive?.Value ?? 0,
                            LastUpdated = DateTimeOffset.Now,
                            Name = x.CurrencyTypeName,
                            DetailsId = x.DetailsId,
                            ItemType = itemType,
                            SparkLine = x.ReceiveSparkLine ?? x.LowConfidenceReceiveSparkLine,
                        });
            }
            catch
            {
                logger.LogInformation("[PoeNinja] Could not fetch {itemType} from poe.ninja", itemType);
            }

            return [];
        }

        private IEnumerable<ItemType> GetApiItemTypes(Category category)
        {
            switch (category)
            {
                case Category.Accessory:
                    yield return ItemType.UniqueAccessory;
                    yield break;

                case Category.Armour:
                    yield return ItemType.UniqueArmour;
                    yield break;

                case Category.Flask:
                    yield return ItemType.UniqueFlask;
                    yield break;

                case Category.Jewel:
                    yield return ItemType.UniqueJewel;
                    yield return ItemType.ClusterJewel;
                    yield break;

                case Category.Weapon:
                    yield return ItemType.UniqueWeapon;
                    yield break;

                case Category.Currency:
                    yield return ItemType.Currency;
                    yield return ItemType.Fragment;
                    yield return ItemType.DeliriumOrb;
                    yield return ItemType.Incubator;
                    yield return ItemType.Oil;
                    yield return ItemType.Incubator;
                    yield return ItemType.Scarab;
                    yield return ItemType.Fossil;
                    yield return ItemType.Resonator;
                    yield return ItemType.Essence;
                    yield return ItemType.Resonator;
                    yield return ItemType.Artifact;
                    yield return ItemType.KalguuranRune;
                    yield return ItemType.Omen;
                    yield return ItemType.Tattoo;
                    yield break;

                case Category.DivinationCard:
                    yield return ItemType.DivinationCard;
                    yield break;

                case Category.Map:
                    yield return ItemType.Map;
                    yield return ItemType.Fragment;
                    yield return ItemType.Scarab;
                    yield return ItemType.Invitation;
                    yield return ItemType.BlightedMap;
                    yield return ItemType.BlightRavagedMap;
                    yield return ItemType.UniqueMap;
                    yield break;

                case Category.Gem:
                    yield return ItemType.SkillGem;
                    yield break;

                case Category.ItemisedMonster:
                    yield return ItemType.Beast;
                    yield break;
            }
        }
    }
}
