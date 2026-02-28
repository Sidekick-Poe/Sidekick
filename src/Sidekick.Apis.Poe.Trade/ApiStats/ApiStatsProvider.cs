using Sidekick.Apis.Poe.Extensions;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.ApiStats;

public class ApiStatsProvider
(
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    DataProvider dataProvider
) : IApiStatsProvider
{
    public Dictionary<StatKey, TradeStatDefinition> Definitions { get; } = [];

    public TradeInvariantStats InvariantStats { get; private set; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        var definitions = await dataProvider.Read<List<TradeStatDefinition>>(game, $"trade/stats.{currentGameLanguage.Language.Code}.json");
        Definitions.Clear();
        foreach (var definition in definitions)
        {
            Definitions.TryAdd(new StatKey(definition.Id, definition.OptionId), definition);
        }

        InvariantStats = await dataProvider.Read<TradeInvariantStats>(game, $"trade/stats.invariant.json");
    }
}
