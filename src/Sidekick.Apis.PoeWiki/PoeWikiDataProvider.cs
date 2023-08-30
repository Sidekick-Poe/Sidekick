using Sidekick.Common.Cache;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.PoeWiki
{
    public class PoeWikiDataProvider : IPoeWikiDataProvider
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeWikiClient poeWikiClient;

        private readonly List<string> oilNames;

        public PoeWikiDataProvider(ICacheProvider cacheProvider,
                                   IPoeWikiClient poeWikiClient)
        {
            this.cacheProvider = cacheProvider;
            this.poeWikiClient = poeWikiClient;

            oilNames = new List<string>() {
                "Clear Oil",
                "Sepia Oil",
                "Amber Oil",
                "Verdant Oil",
                "Teal Oil",
                "Azure Oil",
                "Indigo Oil",
                "Violet Oil",
                "Crimson Oil",
                "Black Oil",
                "Opalescent Oil",
                "Silver Oil",
                "Golden Oil",
            };
        }

        public Dictionary<string, string> BlightOilNamesByMetadataIds { get; private set; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Low;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            var result = await cacheProvider.GetOrSet("PoeWikiBlightOils", async () =>
            {
                var result = await poeWikiClient.GetMetadataIdsFromItemNames(oilNames);
                if (result == null)
                {
                    return new();
                }

                return result;
            });

            if (result != null)
            {
                BlightOilNamesByMetadataIds = result.ToDictionary(x => x.MetadataId ?? string.Empty, x => x.Name ?? string.Empty);
            }
        }
    }
}
