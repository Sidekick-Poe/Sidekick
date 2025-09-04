using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Filters.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Filters;

public class InvariantFilterProvider
(
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
    IGameLanguageProvider gameLanguageProvider
) : IInvariantFilterProvider
{
    public FilterDefinition? DesecratedDefinition { get; private set; }

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_Filters";

        var result = await cacheProvider.GetOrSet(cacheKey,
                                                  () => tradeApiClient.FetchData<ApiFilterCategory>(game, gameLanguageProvider.InvariantLanguage, "filters"),
                                                  (cache) =>
                                                  {
                                                      return cache.Result.Any(x => x.Id == "type_filters") && cache.Result.Any(x => x.Id == "trade_filters");
                                                  });

        DesecratedDefinition = null;

        foreach (var category in result.Result)
        {
            foreach (var filter in category.Filters)
            {
                if (category.Id == "misc_filters" && filter.Id == "desecrated")
                {
                    DesecratedDefinition = new FilterDefinition()
                    {
                        CategoryId = category.Id,
                        FilterId = filter.Id,
                    };
                }
            }
        }
    }
}
