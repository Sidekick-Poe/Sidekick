using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Common.Settings;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.ApiStats;

public class ApiStatsProvider
(
    TradeDataProvider tradeDataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService
) : IApiStatsProvider
{
    public List<TradeStatDefinition> Definitions { get; private set; } = [];

    public TradeInvariantStats InvariantStats { get; private set; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Definitions = await tradeDataProvider.GetStats(game, currentGameLanguage.Language.Code);
        InvariantStats = await tradeDataProvider.GetInvariantStats(game);
    }

    public bool IsMatch(string id, string text)
    {
        var definition = Definitions.FirstOrDefault(definition => definition.Id == id);
        return definition?.Pattern.IsMatch(text) ?? false;
    }
}
