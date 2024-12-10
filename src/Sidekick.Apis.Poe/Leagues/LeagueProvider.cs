using Sidekick.Apis.Poe.Clients;
using Sidekick.Common.Cache;
using Sidekick.Common.Game;

namespace Sidekick.Apis.Poe.Leagues
{
    public class LeagueProvider : ILeagueProvider
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeTradeClient poeTradeClient;

        public LeagueProvider(
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient)
        {
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
        }

        public async Task<List<League>> GetList(bool fromCache)
        {
            if (fromCache)
            {
                return await cacheProvider.GetOrSet("Leagues", GetList);
            }

            var result = await GetList();
            await cacheProvider.Set("Leagues", result);
            return result;
        }

        private async Task<List<League>> GetList()
        {
            var response = await poeTradeClient.Fetch<ApiLeague>(GameType.PathOfExile, "data/leagues");
            var leagues = new List<League>();
            foreach (var apiLeague in response.Result)
            {
                if (apiLeague.Id == null || apiLeague.Text == null || apiLeague.Realm != LeagueRealm.PC)
                {
                    continue;
                }

                leagues.Add(new(apiLeague.Id, apiLeague.Text));
            }

            return leagues;
        }
    }
}
