using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Builder.StatsInvariant;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
namespace Sidekick.Data.Builder.Trade;

public class TradeFilterBuilder
(
    ILogger<StatsInvariantBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    DataProvider dataProvider
)
{
    public async Task Build(IGameLanguage language)
    {
        try
        {
            await BuildForGame(GameType.PathOfExile1, language);
            await BuildForGame(GameType.PathOfExile2, language);
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, "Failed to build trade filters.");
        }
    }

    private async Task BuildForGame(GameType game, IGameLanguage language)
    {
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeFilterCategory>>>(game, DataType.RawTradeFilters, language);
        await dataProvider.Write(game, DataType.TradeFilters, language, result.Result);
    }
}
