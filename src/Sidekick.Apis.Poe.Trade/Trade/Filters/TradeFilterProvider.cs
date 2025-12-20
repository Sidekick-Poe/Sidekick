using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Models;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters;

public class TradeFilterProvider
(
    ITradeApiClient tradeApiClient,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    IServiceProvider serviceProvider
) : ITradeFilterProvider
{
    public ApiFilter? TypeCategory => GetApiFilter("type_filters", "category");
    public ApiFilter? TradePrice => GetApiFilter("trade_filters", "price");
    public ApiFilter? TradeIndexed => GetApiFilter("trade_filters", "indexed");
    public ApiFilter? Desecrated => GetApiFilter("misc_filters", "desecrated");
    public ApiFilter? Veiled => GetApiFilter("misc_filters", "veiled");
    public ApiFilter? Fractured => GetApiFilter("misc_filters", "fractured_item");
    public ApiFilter? Mirrored => GetApiFilter("misc_filters", "mirrored");
    public ApiFilter? Foulborn => GetApiFilter("misc_filters", "foulborn_item");
    public ApiFilter? Sanctified => GetApiFilter("misc_filters", "sanctified");

    public ApiFilterCategory? RequirementsCategory => GetApiFilterCategory("req_filters");
    public ApiFilterCategory? MiscellaneousCategory => GetApiFilterCategory("misc_filters");
    public ApiFilterCategory? TradeCategory => GetApiFilterCategory("trade_filters");

    private List<ApiFilterCategory> Filters { get; set; } = [];

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantFilters";

        var result = await cacheProvider.GetOrSet(cacheKey,
                                                  () => tradeApiClient.FetchData<ApiFilterCategory>(game, gameLanguageProvider.Language, "filters"),
                                                  (cache) =>
                                                  {
                                                      return cache.Result.Any(x => x.Id == "type_filters") && cache.Result.Any(x => x.Id == "trade_filters");
                                                  });
        if (result == null) throw new SidekickException("Could not fetch filters from the trade API.");

        Filters = result.Result;
    }

    public string? GetPriceOption(string? price) => TradePrice?.Option.Options.SingleOrDefault(x => x.Id == price)?.Id;

    public string? GetTradeIndexedOption(string? id) => TradeIndexed?.Option.Options.SingleOrDefault(x => x.Id == id)?.Id;

    public ApiFilterCategory? GetApiFilterCategory(string categoryId)
    {
        return Filters.FirstOrDefault(x => x.Id == categoryId);
    }

    public ApiFilter? GetApiFilter(string categoryId, string filterId)
    {
        return GetApiFilterCategory(categoryId)?.Filters.FirstOrDefault(x => x.Id == filterId);
    }

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        if (TradeCategory?.Title == null) return [];

        var result = new List<TradeFilter>();

        var statusFilter = await serviceProvider.GetRequiredService<PlayerStatusFilter>()
            .GetFilter(item);
        if(statusFilter != null) result.Add(statusFilter);

        var currencyFilter = await serviceProvider.GetRequiredService<CurrencyFilter>()
            .GetFilter(item);
        if(currencyFilter != null) result.Add(currencyFilter);

        if (result.Count == 0) return [];
        return [new ExpandableFilter(TradeCategory.Title, result.ToArray())];
    }
}
