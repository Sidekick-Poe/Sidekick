using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Data.Builder.Ninja.Models;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Languages;
using Sidekick.Data.Leagues;
using Sidekick.Data.Trade;

namespace Sidekick.Data.Builder.Ninja;

public class NinjaDownloader(
    ILogger<NinjaDownloader> logger,
    IOptions<SidekickConfiguration> configuration,
    RawDataProvider rawDataProvider,
    DataProvider dataProvider,
    IGameLanguageProvider languageProvider)
{
    private record NinjaPage(
        string Type,
        string Url,
        bool SupportsExchange,
        bool SupportsStash);

    private static readonly List<NinjaPage> Poe1Pages =
    [
        new("Currency", "currency", true, true),
        new("Fragment", "fragments", true, true),
        new("Wombgift", "wombgifts", false, true),
        new("Runegraft", "runegrafts", true, true),
        new("AllflameEmber", "allflame-embers", true, true),
        new("Tattoo", "tattoos", true, true),
        new("Omen", "omens", true, true),
        new("DjinnCoin", "djinn-coins", true, true),
        new("DivinationCard", "divination-cards", true, true),
        new("Artifact", "artifacts", true, true),
        new("Oil", "oils", true, true),
        new("Incubator", "incubators", false, true),
        new("UniqueWeapon", "unique-weapons", false, true),
        new("UniqueArmour", "unique-armours", false, true),
        new("UniqueAccessory", "unique-accessories", false, true),
        new("UniqueFlask", "unique-flasks", false, true),
        new("UniqueJewel", "unique-jewels", false, true),
        new("ForbiddenJewel", "forbidden-jewels", false, true),
        new("ShrineBelt", "shrine-belts", false, true),
        new("UniqueTincture", "unique-tinctures", false, true),
        new("UniqueRelic", "unique-relics", false, true),
        new("SkillGem", "skill-gems", false, true),
        new("ClusterJewel", "cluster-jewels", false, true),
        new("Map", "maps", false, true),
        new("BlightedMap", "blighted-maps", false, true),
        new("BlightRavagedMap", "blight-ravaged-maps", false, true),
        new("UniqueMap", "unique-maps", false, true),
        new("ValdoMap", "valdo-maps", false, true),
        new("DeliriumOrb", "delirium-orbs", true, true),
        new("Invitation", "invitations", false, true),
        new("Scarab", "scarabs", true, true),
        new("Astrolabe", "astrolabes", true, false),
        new("Memory", "memories", false, true),
        new("IncursionTemple", "temples", false, true),
        new("BaseType", "base-types", false, true),
        new("Fossil", "fossils", true, true),
        new("Resonator", "resonators", true, true),
        new("Beast", "beasts", false, true),
        new("Essence", "essences", true, true),
        new("Vial", "vials", false, true)
    ];

    private static readonly List<NinjaPage> Poe2Pages =
    [
        new("Currency", "currency", true, false),
        new("Fragments", "fragments", true, false),
        new("Abyss", "abyssal-bones", true, false),
        new("UncutGems", "uncut-gems", true, false),
        new("LineageSupportGems", "lineage-support-gems", true, false),
        new("Essences", "essences", true, false),
        new("Ultimatum", "soul-cores", true, false),
        new("Talismans", "talismans", true, false),
        new("Runes", "runes", true, false),
        new("Ritual", "omens", true, false),
        new("Expedition", "expedition", true, false),
        new("Delirium", "distilled-emotions", true, false),
        new("Breach", "breach-catalyst", true, false)
    ];

    public async Task Download()
    {
        try
        {
            var poe1Leagues = await dataProvider.Read<List<League>>(GameType.PathOfExile1, DataType.Leagues, languageProvider.InvariantLanguage);
            await DownloadForGame(GameType.PathOfExile1,
                                  poe1Leagues.First().Id ?? throw new ArgumentException("No leagues found for Poe1"));

            var poe2Leagues = await dataProvider.Read<List<League>>(GameType.PathOfExile2, DataType.Leagues, languageProvider.InvariantLanguage);
            await DownloadForGame(GameType.PathOfExile2,
                                  poe2Leagues.First().Id ?? throw new ArgumentException("No leagues found for Poe2"));
        }
        catch (Exception ex)
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                throw;
            }

            logger.LogError(ex, "Failed to download ninja data.");
        }

        return;

        async Task DownloadForGame(GameType game, string league)
        {
            await DownloadExchange(game, league);
            await DownloadStash(game, league);
        }

        async Task DownloadExchange(GameType game, string league)
        {
            var pages = game == GameType.PathOfExile1 ? Poe1Pages : Poe2Pages;

            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Sidekick", "1.0"));
            http.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");

            foreach (var page in pages)
            {
                if (!page.SupportsExchange) continue;

                var url =
                    $"https://poe.ninja/{game.GetValueAttribute()}/api/economy/exchange/current/overview?league={league.Replace(' ', '+')}&type={page.Type}";
                await DownloadToFile(http, url, game, $"{page.Type}.json");
            }
        }

        async Task DownloadStash(GameType game, string league)
        {
            var pages = game == GameType.PathOfExile1 ? Poe1Pages : Poe2Pages;

            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Sidekick", "1.0"));
            http.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");

            foreach (var page in pages)
            {
                if (!page.SupportsStash || page.SupportsExchange) continue;

                var url =
                    $"https://poe.ninja/{game.GetValueAttribute()}/api/economy/stash/current/item/overview?league={league.Replace(' ', '+')}&type={page.Type}";
                await DownloadToFile(http, url, game, $"{page.Type}.json");
            }
        }

    }

    private async Task DownloadToFile(HttpClient http, string url, GameType game, string fileName)
    {
        try
        {
            logger.LogInformation($"GET {url}");
            using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await rawDataProvider.Write($"{game.GetValueAttribute()}/raw/ninja/{fileName}", await response.Content.ReadAsStreamAsync());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed for {url}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<NinjaItemDefinition>> GetDefinitions(GameType game)
    {
        var definitions = new List<NinjaItemDefinition>();
        definitions.AddRange(await ReadExchange());
        definitions.AddRange(await ReadStash());
        return definitions;

        async Task<List<NinjaItemDefinition>> ReadExchange()
        {
            var items = new List<NinjaItemDefinition>();
            var pages = game == GameType.PathOfExile1 ? Poe1Pages : Poe2Pages;

            foreach (var page in pages)
            {
                if (!page.SupportsExchange) continue;

                var result = await rawDataProvider.Read<NinjaExchangeOverview>($"{game.GetValueAttribute()}/raw/ninja/{page.Type}.json");
                items.AddRange(from it in result.Items
                               where !string.IsNullOrWhiteSpace(it.Id)
                               select new NinjaItemDefinition()
                               {
                                   Type = page.Type,
                                   Url = page.Url,
                                   Exchange = new NinjaExchangeDefinition()
                                   {
                                       Id = it.Id,
                                       DetailsId = it.DetailsId,
                                   },
                               });
            }

            return items;
        }

        async Task<List<NinjaItemDefinition>> ReadStash()
        {
            var items = new List<NinjaItemDefinition>();
            var pages = game == GameType.PathOfExile1 ? Poe1Pages : Poe2Pages;
            var tradeStatDefinitions = await dataProvider.Read<List<TradeStatDefinition>>(game, DataType.TradeStats, languageProvider.InvariantLanguage);

            foreach (var page in pages)
            {
                if (!page.SupportsStash || page.SupportsExchange) continue;

                var result = await rawDataProvider.Read<NinjaStashOverview>($"{game.GetValueAttribute()}/raw/ninja/{page.Type}.json");
                items.AddRange(from it in result.Lines
                               where !string.IsNullOrWhiteSpace(it.Name)
                               select new NinjaItemDefinition()
                               {
                                   Type = page.Type,
                                   Url = page.Url,
                                   Stash = new NinjaStashDefinition()
                                   {
                                       DetailsId = it.DetailsId,
                                       Name = it.Name,
                                       BaseType = it.BaseType,
                                       Corrupted = it.Corrupted,
                                       Foulborn = (it.Name?.StartsWith("Foulborn") ?? false) ? true : null,
                                       GemLevel = it.GemLevel,
                                       GemQuality = it.GemQuality,
                                       Links = it.Links,
                                       ItemLevel = it.LevelRequired,
                                       Variant = it.Variant,
                                       Stats = GetTradeStats(it),
                                   },
                               });
            }

            return items;

            List<NinjaStashStatDefinition>? GetTradeStats(NinjaStashLine it)
            {
                var stats = it.TradeInfo?.ConvertAll(x => new NinjaStashStatDefinition
                {
                    Value = x.Min == x.Max ? x.Min : throw new SidekickException("Unsupported ninja value detected. Typically min and max always match. The item is {0}", it.Name ?? string.Empty),
                    Id = x.Option != null ? $"{x.Mod}#{x.Option}" : x.Mod,
                }) ?? [];

                if (it.MutatedModifiers != null)
                {
                    var numberRegex = new Regex(@"\d+");

                    foreach (var mutatedMod in it.MutatedModifiers)
                    {
                        if (string.IsNullOrEmpty(mutatedMod.Text) || mutatedMod.Optional) continue;
                        var text = numberRegex.Replace(mutatedMod.Text, "#");
                        var tradeMutatedStat = tradeStatDefinitions.FirstOrDefault(x => x.Text == text);
                        if (tradeMutatedStat == null) continue;
                        stats.Add(new NinjaStashStatDefinition
                        {
                            Id = tradeMutatedStat.Id,
                        });
                    }
                }

                return stats.Count == 0 ? null : stats.DistinctBy(x => x.Id).ToList();
            }
        }
    }
}
