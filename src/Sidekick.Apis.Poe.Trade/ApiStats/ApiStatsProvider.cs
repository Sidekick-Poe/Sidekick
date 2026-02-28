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
    public List<TradeStatDefinition> Definitions { get; private set; } = [];

    public Dictionary<string, List<TradeStatDefinition>> IdDictionary { get; } = [];

    public TradeInvariantStats InvariantStats { get; private set; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions = await dataProvider.Read<List<TradeStatDefinition>>(game, $"trade/stats.{currentGameLanguage.Language.Code}.json");
        InvariantStats = await dataProvider.Read<TradeInvariantStats>(game, $"trade/stats.invariant.json");

        foreach (var definition in Definitions)
        {
            if (!IdDictionary.TryAdd(definition.Id, [definition]))
            {
                IdDictionary[definition.Id].Add(definition);
            }
        }
    }
}
