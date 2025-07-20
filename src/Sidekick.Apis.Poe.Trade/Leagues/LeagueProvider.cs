using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Trade.Leagues;

public class LeagueProvider(
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
    IGameLanguageProvider gameLanguageProvider,
    ILogger<LeagueProvider> logger) : ILeagueProvider
{
    private readonly ILogger logger = logger;

    public async Task<List<League>> GetList(bool fromCache)
    {
        if (fromCache)
        {
            return await cacheProvider.GetOrSet("Leagues", FetchAll, (cache) => cache.Count != 0);
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
            throw new ApiErrorException { AdditionalInformation = ["If the official trade website is down, Sidekick will not work.", "Please try again later or open a ticket on GitHub."], };
        }
    }

    private async Task<List<League>> FetchAll()
    {
        await gameLanguageProvider.Initialize();

        List<League> leagues = [];

        leagues.AddRange(await Fetch(GameType.PathOfExile2));

        leagues.AddRange(await Fetch(GameType.PathOfExile));

        return leagues;
    }

    private async Task<List<League>> Fetch(GameType game)
    {
        var response = await tradeApiClient.Fetch<ApiLeague>(game, gameLanguageProvider.InvariantLanguage, "data/leagues");
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
