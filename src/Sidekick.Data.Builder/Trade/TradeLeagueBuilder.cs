using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade.Models;
using Sidekick.Data.Trade.Models.Raw;

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
        await tradeDownloader.DownloadPath(languageProvider.InvariantLanguage, "leagues");
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeLeague>>>(game, "trade/raw/leagues.en.json");

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

        await dataProvider.Write(game, $"trade/leagues.invariant.json", leagues);
    }
}
