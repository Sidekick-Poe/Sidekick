using Sidekick.Apis.Poe.Clients;
using Sidekick.Common.Cache;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Leagues;

public class LeagueProvider(
    ICacheProvider cacheProvider,
    IPoeTradeClient poeTradeClient,
    IGameLanguageProvider gameLanguageProvider) : ILeagueProvider
{
    public async Task<List<League>> GetList(bool fromCache)
    {
        if (fromCache)
        {
            return await cacheProvider.GetOrSet("Leagues", FetchAll);
        }

        var result = await FetchAll();
        await cacheProvider.Set("Leagues", result);
        return result;
    }

    private async Task<List<League>> FetchAll()
    {
        List<Task<List<League>>> tasks =
        [
            Fetch(GameType.PathOfExile2),
            Fetch(GameType.PathOfExile),
        ];

        var allLeagues = await Task.WhenAll(tasks);
        return allLeagues
               .SelectMany(leagues => leagues)
               .ToList();
    }

    private async Task<List<League>> Fetch(GameType game)
    {
        var response = await poeTradeClient.Fetch<ApiLeague>(game, gameLanguageProvider.InvariantLanguage, "data/leagues");
        var leagues = new List<League>();
        foreach (var apiLeague in response.Result)
        {
            if (apiLeague.Id == null || apiLeague.Text == null)
            {
                continue;
            }

            if (apiLeague.Realm != LeagueRealm.PC && apiLeague.Realm != LeagueRealm.Poe2)
            {
                continue;
            }

            var text = game switch
            {
                GameType.PathOfExile2 => $"PoE2 - {apiLeague.Text}",
                _ => $"PoE1 - {apiLeague.Text}",
            };

            var id = game switch
            {
                GameType.PathOfExile2 => $"poe2.{apiLeague.Id}",
                _ => $"poe1.{apiLeague.Id}",
            };

            leagues.Add(new(game, id, text));
        }

        return leagues;
    }
}
