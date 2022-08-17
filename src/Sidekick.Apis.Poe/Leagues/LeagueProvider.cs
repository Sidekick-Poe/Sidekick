using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Common.Cache;

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

            result = result.Where(x => x.Realm == LeagueRealm.PC).ToList();

            await cacheProvider.Set("Leagues", result);
            return result;
        }

        private async Task<List<League>> GetList()
        {
            var response = await poeTradeClient.Fetch<League>("data/leagues");
            return response.Result;
        }
    }
}
