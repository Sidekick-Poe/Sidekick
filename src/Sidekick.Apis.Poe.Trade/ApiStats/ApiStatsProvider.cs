using Sidekick.Apis.Poe.Extensions;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade;
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

        Definitions = await dataProvider.Read<List<TradeStatDefinition>>(game, DataType.TradeStats, currentGameLanguage.Language);
        InvariantStats = await dataProvider.Read<TradeInvariantStats>(game, DataType.TradeStats);

        foreach (var definition in Definitions)
        {
            if (!IdDictionary.TryAdd(definition.Id, [definition]))
            {
                IdDictionary[definition.Id].Add(definition);
            }
        }
    }
}
