using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Filters.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Languages;
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

    public ApiFilter? Desecrated { get; set; }

    private List<ApiFilterCategory> Filters { get; set; } = [];

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantFilters";

        var result = await cacheProvider.GetOrSet(cacheKey,
                                                  () => tradeApiClient.FetchData<ApiFilterCategory>(game, gameLanguageProvider.Language, "filters"),
                                                  (cache) =>
                                                  {
                                                      return cache.Result.Any(x => x.Id == "type_filters") && cache.Result.Any(x => x.Id == "trade_filters");
                                                  });
        Filters = result.Result;

        TypeCategory = GetApiFilter("type_filters", "category");
        TradePrice = GetApiFilter("trade_filters", "price");
        TradeIndexed = GetApiFilter("trade_filters", "indexed");

        if (invariantFilterProvider.DesecratedDefinition != null)
        {
            Desecrated = GetApiFilter(invariantFilterProvider.DesecratedDefinition.CategoryId, invariantFilterProvider.DesecratedDefinition.FilterId);
        }
    }

    public string? GetPriceOption(string? price) => TradePrice?.Option.Options.SingleOrDefault(x => x.Id == price)?.Id;

    public string? GetTradeIndexedOption(string? id) => TradeIndexed?.Option.Options.SingleOrDefault(x => x.Id == id)?.Id;

    private ApiFilter? GetApiFilter(string categoryId, string filterId)
    {
        return Filters.FirstOrDefault(x => x.Id == categoryId)?.Filters.FirstOrDefault(x => x.Id == filterId);
    }
}
