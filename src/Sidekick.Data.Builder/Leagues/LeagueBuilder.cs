using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Data.Builder.Trade;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Leagues;
namespace Sidekick.Data.Builder.Leagues;

public class LeagueBuilder
(
    ILogger<LeagueBuilder> logger,
    IOptions<SidekickConfiguration> configuration,
    TradeDownloader  tradeDownloader,
    IGameLanguageProvider languageProvider,
    DataProvider dataProvider
)
{
    public async Task Build(IGameLanguage language)
    {
        if (language.Code != languageProvider.InvariantLanguage.Code) return;

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

            logger.LogError(ex, "Failed to build trade leagues.");
        }
    }

    private async Task BuildForGame(GameType game, IGameLanguage language)
    {
        await tradeDownloader.DownloadPath(DataType.RawTradeLeagues, language, "leagues");
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeLeague>>>(game, DataType.RawTradeLeagues, language);

        var leagues = result.Result.Where(x => x is { Id: not null, Text: not null })
            .Where(x => x.Realm is LeagueRealm.PC or LeagueRealm.Poe2)
            .Select(x => new League()
            {
                Id = x.Id!,
                Game = game,
                Realm = x.Realm,
                Text = x.Text!,
            })
            .ToList();

        await dataProvider.Write(game, DataType.Leagues, language, leagues);
    }
}
