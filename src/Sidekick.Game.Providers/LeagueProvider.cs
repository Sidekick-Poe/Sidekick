using Sidekick.Game;
using Sidekick.Game.Languages;
using Sidekick.Game.Leagues;
namespace Sidekick.Apis.Poe.Trade.Leagues;

public class LeagueProvider(
    DataProvider dataProvider,
    IGameLanguageProvider languageProvider)
{
    public async Task<List<League>> GetList()
    {
        return
        [
            ..await dataProvider.Read<List<League>>(GameType.PathOfExile2, GameDataType.Leagues, languageProvider.InvariantLanguage),
            ..await dataProvider.Read<List<League>>(GameType.PathOfExile1, GameDataType.Leagues, languageProvider.InvariantLanguage),
        ];
    }
}
