using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Raw;

namespace Sidekick.Data.Builder.Trade;

public class TradeLeagueBuilder
(
    TradeDownloader  tradeDownloader,
    IGameLanguageProvider languageProvider,
    DataProvider dataProvider
)
{
    public async Task Build()
    {
        await BuildForGame(GameType.PathOfExile1);
        await BuildForGame(GameType.PathOfExile2);
    }

    private async Task BuildForGame(GameType game)
    {
        await tradeDownloader.DownloadPath(DataType.TradeRawLeagues, languageProvider.InvariantLanguage, "leagues");
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeLeague>>>(game, DataType.TradeRawLeagues, languageProvider.InvariantLanguage);

        var leagues = result.Result.Where(x => x is { Id: not null, Text: not null })
            .Where(x => x.Realm is TradeLeagueRealm.PC or TradeLeagueRealm.Poe2)
            .Select(x => new TradeLeague()
            {
                Id = x.Id!,
                Game = game,
                Realm = x.Realm,
                Text = x.Text!,
            })
            .ToList();

        await dataProvider.Write(game, DataType.TradeLeagues, languageProvider.InvariantLanguage, leagues);
    }
}
