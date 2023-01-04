using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Languages;

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

        /// <summary>
        /// Gets the metadata ids of the blight oils from PoeWiki using their common names.
        /// </summary>
        public async Task<Dictionary<string, string>> GetBlightOils()
        {
            var result = await cacheProvider.GetOrSet("PoeWikiBlightOils", () => poeWikiClient.GetMetadataIdsFromItemNames(oilNames));

            return result.ToDictionary(x => x.MetadataId, x => x.Name);
        }

        public async Task Initialize()
        {
            await GetBlightOils();
        }
    }
}
