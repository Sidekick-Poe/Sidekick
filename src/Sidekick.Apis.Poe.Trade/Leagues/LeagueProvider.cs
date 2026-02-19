using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Exceptions;
using Sidekick.Data.Builder.Trade;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.Leagues;

public class LeagueProvider(
    TradeDataProvider tradeDataProvider,
    TradeLeagueBuilder tradeLeagueBuilder,
    ILogger<LeagueProvider> logger) : ILeagueProvider
{
    private readonly ILogger logger = logger;

    public async Task<List<TradeLeague>> GetList(bool fromCache)
    {
        if (!fromCache)
        {
            try
            {
                await tradeLeagueBuilder.Build();
            }
            catch (Exception e)
            {
                logger.LogError(e, "[LeagueProvider] Error fetching leagues.");
                throw new ApiErrorException
                {
                    AdditionalInformation = ["If the official trade website is down, Sidekick will not work.", "Please try again later or open a ticket on GitHub."],
                };
            }
        }

        return
        [
            ..await tradeDataProvider.GetLeagues(GameType.PathOfExile2),
            ..await tradeDataProvider.GetLeagues(GameType.PathOfExile1),
        ];
    }
}
