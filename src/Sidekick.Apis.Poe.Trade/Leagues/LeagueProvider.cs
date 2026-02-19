using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Data;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models;
using Sidekick.Data.Trade.Models.Raw;

namespace Sidekick.Apis.Poe.Trade.Leagues;

public class LeagueProvider(
    TradeDataProvider tradeDataProvider,
    DataProvider dataProvider,
    ITradeApiClient tradeApiClient,
    IGameLanguageProvider gameLanguageProvider,
    ILogger<LeagueProvider> logger) : ILeagueProvider
{
    private readonly ILogger logger = logger;

    public async Task<List<League>> GetList(bool fromCache)
    {
        // TODO: change?

        if (fromCache)
        {
            return await FetchAllFromData();
        }

        try
        {
            return await FetchAllFromApi();
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

    private async Task<List<League>> FetchAllFromApi()
    {
        await gameLanguageProvider.Initialize();

        List<League> leagues = [];

        leagues.AddRange(await Fetch(GameType.PathOfExile2));
        leagues.AddRange(await Fetch(GameType.PathOfExile1));

        return leagues;

        async Task<List<League>> Fetch(GameType game)
        {
            var response = await tradeApiClient.FetchData<RawTradeLeague>(game, gameLanguageProvider.InvariantLanguage, "leagues");
            await dataProvider.Write(game, "trade/leagues.en.json", response);
            return response.Result
                .Where(x => x is { Id: not null, Text: not null })
                .Where(x => x is { Realm: not TradeLeagueRealm.PC, Realm: not TradeLeagueRealm.Poe2 })
                .Select(x => new League(game, $"{game.GetValueAttribute()}.{x.Id}", x.Text!))
                .ToList();
        }
    }

    private async Task<List<League>> FetchAllFromData()
    {
        await gameLanguageProvider.Initialize();

        List<League> leagues = [];

        leagues.AddRange(await GetLeaguesForGame(GameType.PathOfExile2));
        leagues.AddRange(await GetLeaguesForGame(GameType.PathOfExile1));

        return leagues;

        async Task<List<League>> GetLeaguesForGame(GameType game)
        {
            var data = await tradeDataProvider.GetLeagues(game);
            return data
                .Where(x => x is { Id: not null, Text: not null })
                .Where(x => x is { Realm: not TradeLeagueRealm.PC, Realm: not TradeLeagueRealm.Poe2 })
                .Select(x => new League(game, $"{game.GetValueAttribute()}.{x.Id}", x.Text!))
                .ToList();
        }
    }
}
