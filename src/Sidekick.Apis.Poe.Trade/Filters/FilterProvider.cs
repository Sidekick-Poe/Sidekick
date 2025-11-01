using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Filters.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Filters;

public class FilterProvider
(
    ITradeApiClient tradeApiClient,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    IInvariantFilterProvider invariantFilterProvider
) : IFilterProvider
{
    public ApiFilter? TypeCategory { get; private set; }

    public ApiFilter? TradePrice { get; private set; }

    public ApiFilter? TradeIndexed { get; private set; }

    public ApiFilter? Desecrated { get; private set; }

    public ApiFilter? Veiled { get; private set; }

    public ApiFilter? Fractured { get; private set; }

    public ApiFilter? Mirrored { get; private set; }

    public ApiFilter? Foulborn { get; private set; }

    public ApiFilter? Sanctified { get; private set; }

    public ApiFilterCategory? RequirementsCategory { get; private set; }

    public ApiFilterCategory? MiscellaneousCategory { get; private set; }

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

        TypeCategory = GetApiFilter("type_filters", "category");
        TradePrice = GetApiFilter("trade_filters", "price");
        TradeIndexed = GetApiFilter("trade_filters", "indexed");

        RequirementsCategory = GetApiFilterCategory("req_filters");
        MiscellaneousCategory = GetApiFilterCategory("misc_filters");

        if (invariantFilterProvider.DesecratedDefinition != null)
        {
            Desecrated = GetApiFilter(invariantFilterProvider.DesecratedDefinition.CategoryId, invariantFilterProvider.DesecratedDefinition.FilterId);
        }

        if (invariantFilterProvider.VeiledDefinition != null)
        {
            Veiled = GetApiFilter(invariantFilterProvider.VeiledDefinition.CategoryId, invariantFilterProvider.VeiledDefinition.FilterId);
        }

        if (invariantFilterProvider.FracturedDefinition != null)
        {
            Fractured = GetApiFilter(invariantFilterProvider.FracturedDefinition.CategoryId, invariantFilterProvider.FracturedDefinition.FilterId);
        }

        if (invariantFilterProvider.MirroredDefinition != null)
        {
            Mirrored = GetApiFilter(invariantFilterProvider.MirroredDefinition.CategoryId, invariantFilterProvider.MirroredDefinition.FilterId);
        }

        if (invariantFilterProvider.FoulbornDefinition != null)
        {
            Foulborn = GetApiFilter(invariantFilterProvider.FoulbornDefinition.CategoryId, invariantFilterProvider.FoulbornDefinition.FilterId);
        }

        if (invariantFilterProvider.SanctifiedDefinition != null)
        {
            Sanctified = GetApiFilter(invariantFilterProvider.SanctifiedDefinition.CategoryId, invariantFilterProvider.SanctifiedDefinition.FilterId);
        }
    }

    public string? GetPriceOption(string? price) => TradePrice?.Option.Options.SingleOrDefault(x => x.Id == price)?.Id;

    public string? GetTradeIndexedOption(string? id) => TradeIndexed?.Option.Options.SingleOrDefault(x => x.Id == id)?.Id;

    private ApiFilterCategory? GetApiFilterCategory(string categoryId)
    {
        return Filters.FirstOrDefault(x => x.Id == categoryId);
    }

    private ApiFilter? GetApiFilter(string categoryId, string filterId)
    {
        return GetApiFilterCategory(categoryId)?.Filters.FirstOrDefault(x => x.Id == filterId);
    }
}
