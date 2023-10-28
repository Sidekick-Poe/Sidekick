using System.Text.Json;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.PoeWiki.Api;
using Sidekick.Apis.PoeWiki.Extensions;
using Sidekick.Apis.PoeWiki.Models;
using Sidekick.Common.Browser;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoeWiki
{
    /// <summary>
    /// PoeWiki.net API.
    /// https://www.poewiki.net/wiki/Path_of_Exile_Wiki:Data_query_API
    /// </summary>
    public class PoeWikiClient : IPoeWikiClient
    {
        private readonly JsonSerializerOptions options;
        private readonly ILogger<PoeWikiClient> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly IBrowserProvider browserProvider;
        private readonly ICacheProvider cacheProvider;
        private readonly ISettings settings;

        private const string PoeWiki_BaseUri = "https://www.poewiki.net/";
        private const string PoeWiki_SubUrl = "w/index.php?search=";

        private static readonly List<string> oilNames = new List<string>() {
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

        public PoeWikiClient(ILogger<PoeWikiClient> logger,
                             IHttpClientFactory httpClientFactory,
                             IGameLanguageProvider gameLanguageProvider,
                             IBrowserProvider browserProvider,
                             ICacheProvider cacheProvider,
                             ISettings settings)
        {
            options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.gameLanguageProvider = gameLanguageProvider;
            this.browserProvider = browserProvider;
            this.cacheProvider = cacheProvider;
            this.settings = settings;
        }

        private HttpClient GetHttpClient()
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://www.poewiki.net/w/api.php");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
            client.Timeout = TimeSpan.FromSeconds(30);
            return client;
        }

        public bool IsEnabled => gameLanguageProvider.IsEnglish() && settings.PoeWikiData_Enable;

        public Dictionary<string, string> BlightOilNamesByMetadataIds { get; private set; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Low;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            var result = await cacheProvider.GetOrSet("PoeWikiBlightOils", async () =>
            {
                var result = await GetMetadataIdsFromItemNames(oilNames);
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

        private async Task<MapResult?> GetMapResult(Item item)
        {
            try
            {
                var mapName = item.Metadata.Name ?? item.Metadata.Type;
                var query = new QueryBuilder(new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "1"),
                    new("tables", "maps,items,areas"),
                    new("join_on", "items._pageID=maps._pageID,maps.area_id=areas.id"),
                    new("fields", "items.name,maps.area_id,areas.boss_monster_ids,items.drop_monsters"),
                    new("group_by", "items.name"),
                    new("where", @$"items.name=""{mapName}"""),
                });

                using var client = GetHttpClient();
                var response = await client.GetAsync(query.ToString());
                var contentString = await response.Content.ReadAsStringAsync();
                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<CargoQueryResult<MapResult>>(content, options);
                return result?.CargoQuery.Select(x => x.Title).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "[PoeWiki] Error while trying to get map from poewiki.net.");
            }

            return null;
        }

        private async Task<List<BossResult>?> GetBossesResult(MapResult mapResult)
        {
            if (mapResult.BossMonsterIds == null)
            {
                return null;
            }

            try
            {
                var query = new QueryBuilder(new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "500"),
                    new("tables", "monsters"),
                    new("fields", "monsters.name,monsters.metadata_id"),
                    new("where", @$"monsters.metadata_id IN ({mapResult.BossMonsterIds.ToQueryString()})"),
                });

                using var client = GetHttpClient();
                var response = await client.GetAsync(query.ToString());
                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<CargoQueryResult<BossResult>>(content, options);
                if (result == null)
                {
                    return null;
                }

                var list = new List<BossResult>();
                foreach (var queryResult in result.CargoQuery)
                {
                    if (queryResult.Title == null)
                    {
                        continue;
                    }

                    list.Add(queryResult.Title);
                }

                return list;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "[PoeWiki] Error while trying to get bosses from poewiki.net.");
            }

            return null;
        }

        private async Task<List<ItemResult>?> GetItemsResult(MapResult mapResult)
        {
            try
            {
                var query = new QueryBuilder(new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("fields", "items.name,items.description,items.flavour_text,items.drop_level"),
                    new("tables", "items"),
                    new("where", @$"items.drop_areas HOLDS '{mapResult.AreaId}' AND items.is_in_game = true AND items.drop_enabled = true"),
                    new("order by", "items.drop_level DESC"),
                    new("limit", "500"),
                });

                using var client = GetHttpClient();
                var response = await client.GetAsync(query.ToString());
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<CargoQueryResult<ItemResult>>(content, options);
                if (result == null)
                {
                    return null;
                }

                var list = new List<ItemResult>();
                foreach (var queryResult in result.CargoQuery)
                {
                    if (queryResult.Title == null)
                    {
                        continue;
                    }

                    list.Add(queryResult.Title);
                }

                return list;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "[PoeWiki] Error while trying to get items from poewiki.net.");
            }

            return null;
        }

        public async Task<List<string>?> GetOilsMetadataIdsFromEnchantment(ModifierLine modifierLine)
        {
            try
            {
                var enchantmentText = modifierLine.Text.Replace("Allocates ", string.Empty);

                var query = new QueryBuilder(new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "500"),
                    new("tables", "blight_crafting_recipes,blight_crafting_recipes_items,mods,passive_skills"),
                    new("join_on", "blight_crafting_recipes_items.recipe_id=blight_crafting_recipes.id,blight_crafting_recipes.modifier_id=mods.id,blight_crafting_recipes.passive_id=passive_skills.id"),
                    new("fields", "blight_crafting_recipes_items.item_id"),
                    new("where", @$"passive_skills.name='{enchantmentText}' OR mods.stat_text='{enchantmentText}'"),
                });

                using var client = GetHttpClient();
                var response = await client.GetAsync(query.ToString());
                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<CargoQueryResult<ItemIdResult>>(content, options);
                if (result == null)
                {
                    return null;
                }

                var list = new List<string>();
                foreach (var queryResult in result.CargoQuery)
                {
                    if (queryResult.Title == null || queryResult.Title.ItemId == null)
                    {
                        continue;
                    }

                    list.Add(queryResult.Title.ItemId);
                }

                return list;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "[PoeWiki] Error while trying to get oils from enchantment from poewiki.net.");
            }

            return null;
        }

        private async Task<List<ItemNameMetadataIdResult>?> GetMetadataIdsFromItemNames(List<string> itemNames)
        {
            try
            {
                var query = new QueryBuilder(new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "500"),

                    new("tables", "items"),

                    new("fields", "items.name,items.metadata_id"),

                    new("where", @$"items.name IN ({itemNames.ToQueryString()})"),
                });

                using var client = GetHttpClient();
                var response = await client.GetAsync(query.ToString());
                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<CargoQueryResult<ItemNameMetadataIdResult>>(content, options);
                if (result == null)
                {
                    return null;
                }

                var list = new List<ItemNameMetadataIdResult>();
                foreach (var queryResult in result.CargoQuery)
                {
                    if (queryResult.Title == null)
                    {
                        continue;
                    }

                    list.Add(queryResult.Title);
                }

                return list;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "[PoeWiki] Error while trying to get item metadata ids from poewiki.net.");
            }

            return null;
        }

        public async Task<Map?> GetMap(Item item)
        {
            // Only maps.
            if (item.Metadata.Category != Category.Map)
            {
                return null;
            }

            var mapResult = await GetMapResult(item);

            if (mapResult == null)
            {
                return null;
            }

            // Fetch bosses.
            var bossesResult = await GetBossesResult(mapResult) ?? new();

            // Fetch dropped items.
            var itemsResult = await GetItemsResult(mapResult) ?? new();

            var map = new Map(mapResult, bossesResult, itemsResult);

            return map;
        }

        public void OpenUri(Map map)
        {
            var wikiLink = PoeWiki_SubUrl + map.Name?.Replace(" ", "+");
            var uri = new Uri(PoeWiki_BaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }

        public void OpenUri(ItemDrop itemDrop)
        {
            var wikiLink = PoeWiki_SubUrl + itemDrop.Name?.Replace(" ", "+");
            var uri = new Uri(PoeWiki_BaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }

        public void OpenUri(Boss boss)
        {
            var wikiLink = "wiki/Monster:" + boss.Id?.Replace("_", "~");
            var uri = new Uri(PoeWiki_BaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }
    }
}
