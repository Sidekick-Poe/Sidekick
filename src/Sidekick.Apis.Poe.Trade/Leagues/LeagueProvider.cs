using Microsoft.Extensions.Logging;
using Sidekick.Common.Exceptions;
using Sidekick.Data;
using Sidekick.Data.Builder.Leagues;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade;

namespace Sidekick.Apis.Poe.Trade.Leagues;

public class LeagueProvider(
    DataProvider dataProvider,
    LeagueBuilder leagueBuilder,
    IGameLanguageProvider languageProvider,
    ILogger<LeagueProvider> logger) : ILeagueProvider
{
    public async Task<List<TradeLeague>> GetList(bool fromCache)
    {
        if (!fromCache)
        {
            try
            {
                await leagueBuilder.Build(languageProvider.InvariantLanguage);
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
            ..await dataProvider.Read<List<TradeLeague>>(GameType.PathOfExile2, DataType.Leagues, languageProvider.InvariantLanguage),
            ..await dataProvider.Read<List<TradeLeague>>(GameType.PathOfExile1, DataType.Leagues, languageProvider.InvariantLanguage),
        ];
    }
}
