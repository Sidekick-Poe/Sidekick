using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.PoeWiki.Api;
using Sidekick.Apis.PoeWiki.Extensions;
using Sidekick.Apis.PoeWiki.Models;
using Sidekick.Common.Browser;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.PoeWiki
{
    /// <summary>
    /// PoeWiki.net API.
    /// https://www.poewiki.net/wiki/Path_of_Exile_Wiki:Data_query_API
    /// </summary>
    public class PoeWikiClient
    (
        ILogger<PoeWikiClient> logger,
        IHttpClientFactory httpClientFactory,
        IBrowserProvider browserProvider,
        ICacheProvider cacheProvider
    ) : IPoeWikiClient
    {
        private readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private const string PoeWikiBaseUri = "https://www.poewiki.net/";
        private const string PoeWikiSubUrl = "w/index.php?search=";

        private static readonly List<string> oilNames =
        [
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
        ];

        private HttpClient GetHttpClient()
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://www.poewiki.net/w/api.php");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
            client.Timeout = TimeSpan.FromSeconds(30);
            return client;
        }

        public Dictionary<string, string> BlightOilNamesByMetadataIds { get; private set; } = new();

        /// <inheritdoc/>
        public int Priority => 0;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            var result = await cacheProvider.GetOrSet("PoeWikiBlightOils",
                                                      async () =>
                                                      {
                                                          var result = await GetMetadataIdsFromItemNames(oilNames);
                                                          if (result == null)
                                                          {
                                                              return new();
                                                          }

                                                          return result;
                                                      }, (cache) => cache.Any());

            BlightOilNamesByMetadataIds = result.ToDictionary(x => x.MetadataId ?? string.Empty, x => x.Name ?? string.Empty);
        }

        private async Task<Uri?> GetMapScreenshotUri(string mapType)
        {
            try
            {
                var query = new List<KeyValuePair<string, string>>
                {
                    new("action", "query"),
                    new("format", "json"),
                    new("titles", $"File:{mapType} area screenshot.png|File:{mapType} area screenshot.jpg"),
                    new("prop", "imageinfo"),
                    new("iiprop", "url"),
                };

                using var client = GetHttpClient();
                var response = await client.GetAsync(QueryStringHelper.ToQueryString(query));
                var content = await response.Content.ReadAsStreamAsync();
                var json = await JsonNode.ParseAsync(content);

                var screenshotUrl = json!["query"]!["pages"]!.AsObject().First(x => x.Key != "-1").Value!["imageinfo"]?.AsArray().First()!["url"]?.AsValue().GetValue<string>()!;

                return new Uri(screenshotUrl);
            }
            catch (Exception)
            {
                // Not having a map screenshot is not exception worthy.
            }

            return null;
        }

        private async Task<MapResult?> GetMapResult(string mapType)
        {
            try
            {
                var query = new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "1"),
                    new("tables", "maps,items,areas"),
                    new("join_on", "items._pageID=maps._pageID,maps.area_id=areas.id"),
                    new("fields", "items.name,maps.area_id,areas.boss_monster_ids,items.drop_monsters,areas.area_type_tags"),
                    new("group_by", "items.name"),
                    new("where", @$"items.name=""{mapType}"" AND items.drop_enabled = true"),
                };

                using var client = GetHttpClient();
                var response = await client.GetAsync(QueryStringHelper.ToQueryString(query));
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
            try
            {
                var query = new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "500"),
                    new("tables", "monsters"),
                    new("fields", "monsters.name,monsters.metadata_id"),
                    new("where", @$"monsters.metadata_id IN ({mapResult.BossMonsterIds.ToQueryString()})"),
                };

                using var client = GetHttpClient();
                var response = await client.GetAsync(QueryStringHelper.ToQueryString(query));
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
                var query = new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("fields", "items.name,items.description,items.flavour_text,items.drop_level"),
                    new("tables", "items"),
                    new("where", @$"items.drop_areas HOLDS '{mapResult.AreaId}' AND items.is_in_game = true AND items.drop_enabled = true"),
                    new("order by", "items.drop_level DESC"),
                    new("limit", "500"),
                };

                using var client = GetHttpClient();
                var response = await client.GetAsync(QueryStringHelper.ToQueryString(query));
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

                var query = new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "500"),
                    new("tables", "blight_crafting_recipes,blight_crafting_recipes_items,mods,passive_skills"),
                    new("join_on", "blight_crafting_recipes_items.recipe_id=blight_crafting_recipes.id,blight_crafting_recipes.modifier_id=mods.id,blight_crafting_recipes.passive_id=passive_skills.id"),
                    new("fields", "blight_crafting_recipes_items.item_id"),
                    new("where", @$"passive_skills.name='{enchantmentText}' OR mods.stat_text='{enchantmentText}'"),
                };

                using var client = GetHttpClient();
                var response = await client.GetAsync(QueryStringHelper.ToQueryString(query));
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
                var query = new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "500"),
                    new("tables", "items"),
                    new("fields", "items.name,items.metadata_id"),
                    new("where", @$"items.name IN ({itemNames.ToQueryString()})"),
                };

                using var client = GetHttpClient();
                var response = await client.GetAsync(QueryStringHelper.ToQueryString(query));
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

        public async Task<Map?> GetMap(string? mapType)
        {
            if (string.IsNullOrEmpty(mapType))
            {
                return null;
            }

            var mapResult = await GetMapResult(mapType);
            if (mapResult == null)
            {
                return null;
            }

            // Fetch bosses.
            var bossesResult = await GetBossesResult(mapResult) ?? new();

            // Fetch dropped items.
            var itemsResult = await GetItemsResult(mapResult) ?? new();

            // Fetch map screenshot.
            var mapScreenshotUri = await GetMapScreenshotUri(mapType);

            var map = new Map(mapResult, bossesResult, itemsResult, mapScreenshotUri);

            return map;
        }

        public void OpenUri(Map map)
        {
            var wikiLink = PoeWikiSubUrl + map.Name?.Replace(" ", "+");
            var uri = new Uri(PoeWikiBaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }

        public void OpenUri(ItemDrop itemDrop)
        {
            var wikiLink = PoeWikiSubUrl + itemDrop.Name?.Replace(" ", "+");
            var uri = new Uri(PoeWikiBaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }

        public void OpenUri(Boss boss)
        {
            var wikiLink = "wiki/Monster:" + boss.Id?.Replace("_", "~");
            var uri = new Uri(PoeWikiBaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }
    }
}
