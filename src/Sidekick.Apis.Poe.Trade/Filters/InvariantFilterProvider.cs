using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Filters.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Extensions;
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
    public FilterDefinition? VeiledDefinition { get; private set; }
    public FilterDefinition? FracturedDefinition { get; private set; }
    public FilterDefinition? MirroredDefinition { get; private set; }
    public FilterDefinition? SanctifiedDefinition { get; private set; }

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var cacheKey = $"{game.GetValueAttribute()}_Filters";

        var result = await cacheProvider.GetOrSet(cacheKey,
                                                  () => tradeApiClient.FetchData<ApiFilterCategory>(game, gameLanguageProvider.InvariantLanguage, "filters"),
                                                  (cache) =>
                                                  {
                                                      return cache.Result.Any(x => x.Id == "type_filters") && cache.Result.Any(x => x.Id == "trade_filters");
                                                  });
        if (result == null) throw new SidekickException("Could not fetch filters from the trade API.");

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

                if (category.Id == "misc_filters" && filter.Id == "veiled")
                {
                    VeiledDefinition = new FilterDefinition()
                    {
                        CategoryId = category.Id,
                        FilterId = filter.Id,
                    };
                }

                if (category.Id == "misc_filters" && filter.Id == "fractured_item")
                {
                    FracturedDefinition = new FilterDefinition()
                    {
                        CategoryId = category.Id,
                        FilterId = filter.Id,
                    };
                }

                if (category.Id == "misc_filters" && filter.Id == "mirrored")
                {
                    MirroredDefinition = new FilterDefinition()
                    {
                        CategoryId = category.Id,
                        FilterId = filter.Id,
                    };
                }

                if (category.Id == "misc_filters" && filter.Id == "sanctified")
                {
                    SanctifiedDefinition = new FilterDefinition()
                    {
                        CategoryId = category.Id,
                        FilterId = filter.Id,
                    };
                }
            }
        }
    }
}
