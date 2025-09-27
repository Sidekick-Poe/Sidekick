using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.PoeNinja.Api;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoeNinja;

/// <summary>
/// Provides a client to communicate with the poe.ninja API.
/// </summary>
/// <remarks>
/// Only for POE1.
/// </remarks>
public class PoeNinjaClient(
    ICacheProvider cacheProvider,
    ISettingsService settingsService,
    IHttpClientFactory httpClientFactory,
    ILogger<PoeNinjaClient> logger) : IPoeNinjaClient
{
    private static readonly Uri baseUrl = new("https://poe.ninja/");
    private static readonly Uri apiBaseUrl = new("https://poe.ninja/api/data/");

    private static readonly List<ItemType> itemTypes =
    [
        ItemType.ClusterJewel,
        ItemType.UniqueAccessory,
        ItemType.UniqueArmour,
        ItemType.UniqueFlask,
        ItemType.UniqueJewel,
        ItemType.UniqueWeapon,
        ItemType.Currency,
        ItemType.Fragment,
        ItemType.DeliriumOrb,
        ItemType.Incubator,
        ItemType.Oil,
        ItemType.Incubator,
        ItemType.Scarab,
        ItemType.Fossil,
        ItemType.Resonator,
        ItemType.Essence,
        ItemType.Resonator,
        ItemType.Artifact,
        ItemType.KalguuranRune,
        ItemType.Omen,
        ItemType.Tattoo,
        ItemType.Runegraft,
        ItemType.DivinationCard,
        ItemType.Map,
        ItemType.Fragment,
        ItemType.Scarab,
        ItemType.Invitation,
        ItemType.BlightedMap,
        ItemType.BlightRavagedMap,
        ItemType.UniqueMap,
        ItemType.AllflameEmber,
        ItemType.Beast,
        ItemType.SkillGem,
    ];

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private HttpClient GetHttpClient()
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        return client;
    }

    public async Task<NinjaPrice?> GetPriceInfo(
        string? invariantText,
        int? gemLevel = null,
        int? mapTier = null,
        bool? isRelic = false,
        int? numberOfLinks = null)
    {
        if (invariantText == "Chaos Orb")
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
        var prices = await GetPrices();

        var query = prices.Where(x => x.Name == invariantText);

        if (gemLevel is > 0)
        {
            query = query.Where(x => x.GemLevel == gemLevel);
        }

        if (isRelic != null)
        {
            query = query.Where(x => x.IsRelic == isRelic);
        }

        if (mapTier is > 0)
        {
            query = query.Where(x => x.MapTier == mapTier);
        }

        if (numberOfLinks is > 0)
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
        var prices = await GetPrices();

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

        var league = await settingsService.GetLeague();

        var ninjaLeagues = await GetLeagues();
        var ninjaLeague = ninjaLeagues.FirstOrDefault(x => x.Name == league);

        if (ninjaLeague == null)
        {
            logger.LogWarning("[PoeNinja] Could not find league {LeagueId} in poe.ninja", league);
            return baseUrl;
        }

        return new Uri(baseUrl, $"economy/{ninjaLeague.Url}/{ninjaPrice.ItemType.GetValueAttribute()}/{ninjaPrice.DetailsId}");

    }

    private static string GetCacheKey(ItemType itemType) => $"PoeNinja_{itemType}";

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

    private async Task SaveItemsToCache(ItemType itemType, List<NinjaPrice> prices)
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

    private async Task<IEnumerable<NinjaPrice>> GetPrices()
    {
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

    private async Task<IList<NinjaEconomyLeague>> GetLeagues()
    {
        var cachedLeagues = await cacheProvider.Get<List<NinjaEconomyLeague>>("PoeNinja_Leagues", (cache) => cache.Any());
        if (cachedLeagues is
            {
                Count: > 0
            })
        {
            return cachedLeagues;
        }
        var leagues = await FetchLeagues();
        if (leagues.Any())
        {
            await cacheProvider.Set("PoeNinja_Leagues", leagues.ToList());
        }
        return leagues.ToList();
    }

    private async Task<List<NinjaEconomyLeague>> FetchLeagues()
    {
        var url = new Uri($"{apiBaseUrl}index-state");

        try
        {
            using var client = GetHttpClient();
            var response = await client.GetAsync(url);
            var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<NinjaIndexState>(responseStream, JsonSerializerOptions);
            if (result == null)
            {
                return [];
            }

            return result.EconomyLeagues;
        }
        catch (Exception)
        {
            logger.LogWarning("[PoeNinja] Could not fetch leagues from poe.ninja");
        }

        return [];
    }

    private async Task<IEnumerable<NinjaPrice>> FetchItems(ItemType itemType)
    {
        var league = await settingsService.GetLeague();
        var url = new Uri($"{apiBaseUrl}itemoverview?league={league}&type={itemType}");

        try
        {
            using var client = GetHttpClient();
            var response = await client.GetAsync(url);
            var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<PoeNinjaQueryResult<PoeNinjaItem>>(responseStream, JsonSerializerOptions);
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
        var league = await settingsService.GetLeague();
        var url = new Uri($"{apiBaseUrl}currencyoverview?league={league}&type={itemType}");

        try
        {
            using var client = GetHttpClient();
            var response = await client.GetAsync(url);
            var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<PoeNinjaQueryResult<PoeNinjaCurrency>>(responseStream, JsonSerializerOptions);
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
}
