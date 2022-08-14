using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.PoeWiki.ApiModels;
using Sidekick.Apis.PoeWiki.Extensions;
using Sidekick.Apis.PoeWiki.Models;
using Sidekick.Common.Browser;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
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
        private readonly HttpClient client;
        private readonly ILogger<PoeWikiClient> logger;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly IBrowserProvider browserProvider;
        private readonly ISettings settings;

        private const string PoeWiki_BaseUri = "https://www.poewiki.net/";
        private const string PoeWiki_SubUrl = "w/index.php?search=";

        public bool IsEnabled { get; }

        public PoeWikiClient(ILogger<PoeWikiClient> logger,
                             IHttpClientFactory httpClientFactory,
                             IGameLanguageProvider gameLanguageProvider,
                             IBrowserProvider browserProvider,
                             ISettings settings)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://www.poewiki.net/w/api.php");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
            client.Timeout = TimeSpan.FromSeconds(30);
            options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            this.logger = logger;
            this.gameLanguageProvider = gameLanguageProvider;
            this.browserProvider = browserProvider;
            this.settings = settings;

            IsEnabled = this.gameLanguageProvider.IsEnglish() && this.settings.PoeWikiMap_Enable;
        }

        private async Task<MapResult> GetMapResult(Item item)
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

                    new("join_on", @"items._pageID=maps._pageID,maps.area_id=areas.id"),

                    new("fields", @"items.name,maps.area_id,areas.boss_monster_ids,items.drop_monsters"),

                    new("group_by", "items.name"),

                    new("where", @$"items.name=""{mapName}"""),
                });

                var response = await client.GetAsync(query.ToString());
                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<CargoQueryResult<MapResult>>(content, options);

                return result.CargoQuery.Select(x => x.Title).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error while trying to get map from poewiki.net.");
            }

            return null;
        }

        private async Task<List<BossResult>> GetBossesResult(MapResult mapResult)
        {
            if (mapResult.BossMonsterIds == null) return null;

            try
            {
                var query = new QueryBuilder(new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "500"),

                    new("tables", "monsters"),

                    new("fields", @"monsters.name,monsters.metadata_id"),

                    new("where", @$"monsters.metadata_id IN ({mapResult.BossMonsterIds.ToQueryString()})"),
                });

                var response = await client.GetAsync(query.ToString());
                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<CargoQueryResult<BossResult>>(content, options);

                return result.CargoQuery.Select(x => x.Title).ToList();
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error while trying to get bosses from poewiki.net.");
            }

            return null;
        }

        private async Task<List<ItemResult>> GetItemsResult(MapResult mapResult)
        {
            try
            {
                var query = new QueryBuilder(new List<KeyValuePair<string, string>>
                {
                    new("action", "cargoquery"),
                    new("format", "json"),
                    new("limit", "500"),

                    new("tables", "items"),

                    new("fields", @"items.drop_areas,items.flavour_text,items.drop_text,items.class_id,items.drop_monsters,items.name,items.drop_enabled"),

                    new("where", @$"items.drop_areas HOLDS '{mapResult.AreaId}'"),
                });

                var response = await client.GetAsync(query.ToString());
                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<CargoQueryResult<ItemResult>>(content, options);

                return result.CargoQuery.Select(x => x.Title).ToList();
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error while trying to get items from poewiki.net.");
            }

            return null;
        }

        public async Task<Map> GetMap(Item item)
        {
            // Only maps.
            if (item.Metadata.Category != Category.Map) return null;

            var mapResult = await GetMapResult(item);

            if (mapResult == null) return null;

            // Fetch bosses.
            var bossesResult = await GetBossesResult(mapResult) ?? new();

            // Fetch dropped items.
            var itemsResult = await GetItemsResult(mapResult) ?? new();

            var map = new Map(mapResult, bossesResult, itemsResult);

            return map;
        }

        public void OpenUri(Map map)
        {
            var wikiLink = PoeWiki_SubUrl + map.Name.Replace(" ", "+");
            var uri = new Uri(PoeWiki_BaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }

        public void OpenUri(ItemDrop itemDrop)
        {
            var wikiLink = PoeWiki_SubUrl + itemDrop.Name.Replace(" ", "+");
            var uri = new Uri(PoeWiki_BaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }

        public void OpenUri(Boss boss)
        {
            var wikiLink = "wiki/Monster:" + boss.Id.Replace("_", "~");
            var uri = new Uri(PoeWiki_BaseUri + wikiLink);

            browserProvider.OpenUri(uri);
        }
    }
}
