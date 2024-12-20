using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Metadata.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Filters;

public class FilterProvider
(
    IPoeTradeClient poeTradeClient,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    ICacheProvider cacheProvider
) : IFilterProvider
{
    public List<ApiFilterOption> ApiItemCategories { get; set; } = new();

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_Filters";

        var result = await cacheProvider.GetOrSet(cacheKey, () => poeTradeClient.Fetch<ApiFilter>(game, gameLanguageProvider.Language, "data/filters"));

        ApiItemCategories = result.Result.First(x => x.Id == "type_filters").Filters
            .First(x => x.Id == "category").Option!.Options;
    }

}
