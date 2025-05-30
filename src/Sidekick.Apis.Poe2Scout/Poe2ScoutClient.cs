
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe2Scout.Api;
using Sidekick.Apis.Poe2Scout.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe2Scout;

/// <summary>
/// Only for POE2.
/// https://poe2scout.com/api/swagger
/// https://poe2scout.com/api/items was made for Sidekick.
/// </summary>
public class Poe2ScoutClient
(
    ICacheProvider cacheProvider,
    ISettingsService settingsService,
    IHttpClientFactory httpClientFactory,
    ILogger<Poe2ScoutClient> logger
) : IPoe2ScoutClient
{
    private static readonly Uri baseUrl = new("https://poe2scout.com");
    private static readonly Uri apiBaseUrl = new("https://poe2scout.com/api/");

    private const string CacheKey = "Poe2Scout_Items";

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task<Poe2ScoutPrice?> GetPriceInfo(Item item)
    {
        if (item.Header.Rarity == Rarity.Normal || item.Header.Rarity == Rarity.Magic || item.Header.Rarity == Rarity.Rare)
        {
            return null;
        }

        await ClearCacheIfExpired();

        var prices = await GetPrices();

        // Same category and ignore items with no prices.
        var query = prices.Where(x => x.CategoryApiId == item.Header.Category && x.Price != 0).ToList();

        // Match by name or type, or name with type as a fallback.
        var price = query.FirstOrDefault(x => x.Name == item.Invariant?.Name)
                    ?? query.FirstOrDefault(x => x.Type == item.Invariant?.Type)
                    ?? query.FirstOrDefault(x => x.Name == item.Invariant?.Type);

        return price;
    }

    public async Task<List<Poe2ScoutPrice>?> GetUniquesFromType(Item item)
    {
        await ClearCacheIfExpired();

        var prices = await GetPrices();

        return prices.Where(x => x.CategoryApiId == item.Header.Category && x.Price != 0).Where(x => x.Name == item.Invariant?.Type || x.Type == item.Invariant?.Type).OrderByDescending(x => x.Price).ToList();
    }

    private async Task<List<Poe2ScoutPrice>> GetPrices()
    {
        var cachedItems = await cacheProvider.Get<List<Poe2ScoutPrice>>(CacheKey, (cache) => cache.Count != 0);
        if (cachedItems != null && cachedItems.Count != 0)
        {
            return cachedItems;
        }

        var items = await FetchItems();
        if (items.Count != 0)
        {
            await cacheProvider.Set(CacheKey, items);
        }

        return items;
    }

    /// <summary>
    /// Work in progress, the website does not support urls with league and item directly.
    /// </summary>
    public Uri GetDetailsUri(Poe2ScoutPrice price)
    {
        if (price.CategoryApiId == null || price.CategoryApiId == Category.Unknown)
        {
            return baseUrl;
        }

        return new Uri(baseUrl, $"economy/{price.CategoryApiId.GetValueAttribute().ToLowerInvariant()}?search={price.Name}");
    }

    private async Task ClearCacheIfExpired()
    {
        var lastClear = await settingsService.GetDateTime(SettingKeys.Poe2ScoutLastClear);
        var isCacheTimeValid = lastClear.HasValue && DateTimeOffset.Now - lastClear <= TimeSpan.FromHours(6);
        if (isCacheTimeValid)
        {
            return;
        }

        cacheProvider.Delete(CacheKey);

        await settingsService.Set(SettingKeys.Poe2ScoutLastClear, DateTimeOffset.Now);
    }

    private HttpClient GetHttpClient()
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        return client;
    }

    private async Task<List<Poe2ScoutPrice>> FetchItems()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var url = new Uri($"{apiBaseUrl}items?league={leagueId.GetUrlSlugForLeague()}");

        try
        {
            using var client = GetHttpClient();
            var response = await client.GetAsync(url);
            var responseStream = await response.Content.ReadAsStreamAsync();
            var results = await JsonSerializer.DeserializeAsync<List<Poe2ScoutItem>>(responseStream, JsonSerializerOptions);

            if (results == null)
            {
                return [];
            }

            return results.Select(result => new Poe2ScoutPrice()
                {
                    Name = result.Name ?? result.Text,
                    Type = result.Type,
                    CategoryApiId = result.CategoryApiId != null ? result.CategoryApiId == "waystones" ? Category.Map : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.CategoryApiId).GetEnumFromValue<Category>() : Category.Unknown,
                    Price = result.CurrentPrice,
                    PriceLogs = result.PriceLogs?.Where(x => x != null)
                                                 // Reverse order to get earliest date first.
                                                 .OrderBy(x => x!.Time)
                                                 // Round to 3 decimal places.
                                                 .Select(x => new Poe2ScoutPriceLog { Price = Math.Round(x!.Price!.Value, 3), Time = x!.Time })
                                                 .ToList()!,
                    LastUpdated = DateTimeOffset.Now
                }).ToList();
        }
        catch
        {
            logger.LogWarning("[Poe2Scout] Could not fetch items from poe2scout.com");
        }

        return [];
    }
}
