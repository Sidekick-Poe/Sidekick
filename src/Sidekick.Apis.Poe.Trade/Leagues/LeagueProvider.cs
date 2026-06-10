using Sidekick.Data;
using Sidekick.Data.Languages;
using Sidekick.Data.Leagues;
namespace Sidekick.Apis.Poe.Trade.Leagues;

public class LeagueProvider(
    DataProvider dataProvider,
    IGameLanguageProvider languageProvider) : ILeagueProvider
{
    public async Task<List<League>> GetList()
    {
        return
        [
            ..await dataProvider.Read<List<League>>(GameType.PathOfExile2, DataType.Leagues, languageProvider.InvariantLanguage),
            ..await dataProvider.Read<List<League>>(GameType.PathOfExile1, DataType.Leagues, languageProvider.InvariantLanguage),
        ];
    }
}
