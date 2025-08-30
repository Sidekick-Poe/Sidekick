using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Filters.Models;
using Sidekick.Common;
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
    ICacheProvider cacheProvider
) : IFilterProvider
{
    public List<ApiFilterOption> TypeCategoryOptions { get; private set; } = [];
    public List<ApiFilterOption> TradePriceOptions { get; private set; } = [];
    public List<ApiFilterOption> TradeIndexedOptions { get; private set; } = [];

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        if (SidekickConfiguration.IsPoeApiDown) return;

        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_Filters";

        var result = await cacheProvider.GetOrSet(cacheKey, () => tradeApiClient.FetchData<ApiFilter>(game, gameLanguageProvider.Language, "filters"),
                                                  (cache) =>
                                                  {
                                                      return cache.Result.Any(x => x.Id == "type_filters") && cache.Result.Any(x => x.Id == "trade_filters");
                                                  });

        TypeCategoryOptions = result.Result
            .First(x => x.Id == "type_filters").Filters
            .First(x => x.Id == "category").Option!.Options;

        TradePriceOptions = result.Result
            .First(x => x.Id == "trade_filters").Filters
            .First(x => x.Id == "price").Option!.Options;

        TradeIndexedOptions = result.Result
            .First(x => x.Id == "trade_filters").Filters
            .First(x => x.Id == "indexed").Option!.Options;
    }

    public string? GetPriceOption(string? price) =>
        TradePriceOptions.SingleOrDefault(x => x.Id == price)?.Id;

    public string? GetTradeIndexedOption(string? id) =>
        TradeIndexedOptions.SingleOrDefault(x => x.Id == id)?.Id;
}
