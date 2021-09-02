using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Cache;

namespace Sidekick.Apis.Poe.Modifiers
{
    public class EnglishModifierProvider : IEnglishModifierProvider
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeTradeClient poeTradeClient;

        public EnglishModifierProvider(
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient)
        {
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
        }

        public List<string> IncursionRooms { get; private set; }

        public async Task Initialize()
        {
            var result = await GetList();

            foreach (var category in result)
            {
                var first = category.Entries.FirstOrDefault();
                if (first?.Id.Split('.').First() == "pseudo")
                {
                    IncursionRooms = category.Entries.Where(x => x.Text.StartsWith("Has Room: ")).Select(x => x.Id).ToList();
                }
            }
        }

        public async Task<List<ApiCategory>> GetList()
        {
            var result = await cacheProvider.GetOrSet(
                "PseudoModifierProvider",
                () => poeTradeClient.Fetch<ApiCategory>("data/stats", useDefaultLanguage: true));

            return result.Result;
        }
    }
}
