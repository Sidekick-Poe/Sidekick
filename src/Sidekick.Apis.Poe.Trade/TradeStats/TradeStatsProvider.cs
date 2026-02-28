using Sidekick.Apis.Poe.Extensions;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.TradeStats;

public class TradeStatsProvider
(
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    DataProvider dataProvider
) : ITradeStatsProvider
{
    public Dictionary<string, TradeStatDefinition> Definitions { get; } = [];

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
            Definitions.TryAdd(definition.Id, definition);
        }

        InvariantStats = await dataProvider.Read<TradeInvariantStats>(game, $"trade/stats.invariant.json");
    }
}
