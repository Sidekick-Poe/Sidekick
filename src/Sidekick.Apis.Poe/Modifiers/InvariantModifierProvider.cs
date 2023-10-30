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

        public string ClusterJewelSmallPassiveModifierId { get; private set; } = null!;

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Medium;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            var result = await GetList();
            InitializeIncursionRooms(result);
            InitializeLogbookFactions(result);
            InitializeClusterJewel(result);
        }

        private void InitializeIncursionRooms(List<ApiCategory> apiCategories)
        {
            IncursionRoomModifierIds.Clear();
            foreach (var apiCategory in apiCategories)
            {
                var first = apiCategory.Entries.FirstOrDefault();
                if (first?.Id?.Split('.')[0] != "pseudo")
                {
                    return;
                }

                IncursionRoomModifierIds.AddRange(
                    apiCategory.Entries
                        .Where(x => x.Text?.StartsWith("Has Room: ") ?? false)
                        .Select(x => x.Id ?? string.Empty)
                        .ToList());
            }
        }

        private void InitializeLogbookFactions(List<ApiCategory> apiCategories)
        {
            LogbookFactionModifierIds.Clear();
            foreach (var apiCategory in apiCategories)
            {
                var first = apiCategory.Entries.FirstOrDefault();
                if (first?.Id?.Split('.')[0] != "pseudo")
                {
                    return;
                }

                LogbookFactionModifierIds.AddRange(
                    apiCategory.Entries
                        .Where(x => x.Text?.StartsWith("Has Logbook Faction: ") ?? false)
                        .Select(x => x.Id ?? string.Empty)
                        .ToList());
            }
        }

        private void InitializeClusterJewel(List<ApiCategory> apiCategories)
        {
            foreach (var apiCategory in apiCategories)
            {
                var first = apiCategory.Entries.FirstOrDefault();
                if (first?.Id?.Split('.')[0] != "enchant")
                {
                    continue;
                }

                foreach (var apiModifier in apiCategory.Entries)
                {
                    if (apiModifier.Text == "Adds # Passive Skills")
                    {
                        ClusterJewelSmallPassiveModifierId = apiModifier.Id;
                    }
                }
            }
        }

        public Task<List<ApiCategory>> GetList() => cacheProvider.GetOrSet("EnglishCategories", async () =>
        {
            var result = await poeTradeClient.Fetch<ApiCategory>("data/stats", useDefaultLanguage: true);
            return result.Result;
        });
    }
}
