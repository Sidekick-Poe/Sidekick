using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Modifiers
{
    public class InvariantModifierProvider : IInvariantModifierProvider
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeTradeClient poeTradeClient;

        public InvariantModifierProvider(
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient)
        {
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
        }

        public List<string> IncursionRoomModifierIds { get; } = new();

        public List<string> LogbookFactionModifierIds { get; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Medium;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            var result = await GetList();

            IncursionRoomModifierIds.Clear();
            LogbookFactionModifierIds.Clear();

            foreach (var category in result)
            {
                var first = category.Entries.FirstOrDefault();
                if (first?.Id?.Split('.')[0] != "pseudo")
                {
                    return;
                }

                IncursionRoomModifierIds.AddRange(
                    category.Entries
                        .Where(x => x.Text?.StartsWith("Has Room: ") ?? false)
                        .Select(x => x.Id ?? string.Empty)
                        .ToList());

                LogbookFactionModifierIds.AddRange(
                    category.Entries
                        .Where(x => x.Text?.StartsWith("Has Logbook Faction: ") ?? false)
                        .Select(x => x.Id ?? string.Empty)
                        .ToList());
            }
        }

        public Task<List<ApiCategory>> GetList() => cacheProvider.GetOrSet("EnglishCategories", async () =>
        {
            var result = await poeTradeClient.Fetch<ApiCategory>("data/stats", useDefaultLanguage: true);
            return result.Result;
        });
    }
}
