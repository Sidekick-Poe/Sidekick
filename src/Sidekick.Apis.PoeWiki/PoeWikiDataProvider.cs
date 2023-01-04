using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.PoeWiki.ApiModels;
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

        public Dictionary<string, string> BlightOilNamesByMetadataIds { get; private set; } = new();

        public async Task Initialize()
        {
            var result = await cacheProvider.GetOrSet("PoeWikiBlightOils", () => poeWikiClient.GetMetadataIdsFromItemNames(oilNames));

            if (result != null)
            {
                BlightOilNamesByMetadataIds = result?.ToDictionary(x => x.MetadataId, x => x.Name);
            }
        }
    }
}
