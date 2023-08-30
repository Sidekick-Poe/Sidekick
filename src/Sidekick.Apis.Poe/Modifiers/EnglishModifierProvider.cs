using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Initialization;

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

        public List<string> IncursionRooms { get; } = new();

        public List<string> LogbookFactions { get; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Medium;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            var result = await cacheProvider.GetOrSet("EnglishCategories", GetCategories);

            IncursionRooms.Clear();
            LogbookFactions.Clear();

            foreach (var category in result)
            {
                var first = category.Entries.FirstOrDefault();
                if (first?.Id?.Split('.').First() == "pseudo")
                {
                    IncursionRooms.AddRange(
                        category.Entries
                            .Where(x => x.Text?.StartsWith("Has Room: ") ?? false)
                            .Select(x => x.Id ?? string.Empty)
                            .ToList());

                    LogbookFactions.AddRange(
                        category.Entries
                            .Where(x => x.Text?.StartsWith("Has Logbook Faction: ") ?? false)
                            .Select(x => x.Id ?? string.Empty)
                            .ToList());
                }
            }
        }

        public Task<List<ApiCategory>> GetList() => cacheProvider.GetOrSet("EnglishCategories", GetCategories);

        private async Task<List<ApiCategory>> GetCategories()
        {
            var result = await poeTradeClient.Fetch<ApiCategory>("data/stats", useDefaultLanguage: true);
            return result.Result;
        }
    }
}
