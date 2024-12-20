using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Leagues;

public class LeagueProvider(
    ICacheProvider cacheProvider,
    IPoeTradeClient poeTradeClient,
    IGameLanguageProvider gameLanguageProvider,
    ILogger<LeagueProvider> logger) : ILeagueProvider
{
    private readonly ILogger logger = logger;

    public async Task<List<League>> GetList(bool fromCache)
    {
        if (fromCache)
        {
            return await cacheProvider.GetOrSet("Leagues", FetchAll);
        }

        try
        {
            var result = await FetchAll();
            await cacheProvider.Set("Leagues", result);
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[LeagueProvider] Error fetching leagues.");
            throw new ApiErrorException("Failed to load league data.");
        }
    }

    private async Task<List<League>> FetchAll()
    {
        await gameLanguageProvider.Initialize();

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

            leagues.Add(new(game, $"{game.GetValueAttribute()}.{apiLeague.Id}", apiLeague.Text));
        }

        return leagues;
    }
}
