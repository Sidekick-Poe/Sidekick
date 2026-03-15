using Sidekick.Apis.Poe.Extensions;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.StatsInvariant;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public class ApiStatsProvider
(
    ISettingsService settingsService,
    DataProvider dataProvider
) : IApiStatsProvider
{
    public StatsInvariantDetails InvariantDetails { get; private set; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        InvariantDetails = await dataProvider.Read<StatsInvariantDetails>(game, DataType.StatsInvariant);
    }
}
