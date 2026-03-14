using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Languages;

namespace Sidekick.Data.Builder.Trade;

public class TradeBuilder(
    ILogger<TradeDownloader> logger,
    IOptions<SidekickConfiguration> configuration,
    TradeInvariantStatBuilder invariantStatBuilder,
    TradeLeagueBuilder leagueBuilder,
    TradeStatBuilder statBuilder)
{
    public async Task Build(IGameLanguage language)
    {
        try
        {
            await leagueBuilder.Build(language);
            await invariantStatBuilder.Build(language);
            await statBuilder.Build(language);
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, $"Failed to download trade data for {language.Code}.");
        }
    }
}
