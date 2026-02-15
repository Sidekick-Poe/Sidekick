using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Data.Files;
using Sidekick.Data.Options;

namespace Sidekick.Data.Ninja;

internal class NinjaDownloader(
    ILogger<NinjaDownloader> logger,
    DataFileWriter dataFileWriter,
    IOptions<DataOptions> options)
{
    private static readonly List<NinjaPage> Poe1Pages =
    [
        new("Currency", "currency", true, true),
        new("Fragment", "fragments", true, true),
        new("Runegraft", "runegrafts", true, true),
        new("AllflameEmber", "allflame-embers", true, true),
        new("Tattoo", "tattoos", true, true),
        new("Omen", "omens", true, true),
        new("DivinationCard", "divination-cards", true, true),
        new("Artifact", "artifacts", true, true),
        new("Oil", "oils", true, true),
        new("Incubator", "incubators", false, true),
        new("UniqueWeapon", "unique-weapons", false, true),
        new("UniqueArmour", "unique-armours", false, true),
        new("UniqueAccessory", "unique-accessories", false, true),
        new("UniqueFlask", "unique-flasks", false, true),
        new("UniqueJewel", "unique-jewels", false, true),
        new("UniqueTincture", "unique-tinctures", false, true),
        new("UniqueRelic", "unique-relics", false, true),
        new("SkillGem", "skill-gems", false, true),
        new("ClusterJewel", "cluster-jewels", false, true),
        new("Map", "maps", false, true),
        new("BlightedMap", "blighted-maps", false, true),
        new("BlightRavagedMap", "blight-ravaged-maps", false, true),
        new("UniqueMap", "unique-maps", false, true),
        new("DeliriumOrb", "delirium-orbs", true, true),
        new("Invitation", "invitations", false, true),
        new("Scarab", "scarabs", true, true),
        new("Memory", "memories", false, true),
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

    private static string GetFileName(string type) => $"{type}.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task DownloadAll()
    {
        if (string.IsNullOrWhiteSpace(options.Value.DataFolder))
        {
            throw new ArgumentException("Data folder cannot be null for download.");
        }

        if (string.IsNullOrWhiteSpace(options.Value.Poe1League) &&
            string.IsNullOrWhiteSpace(options.Value.Poe2League))
        {
            throw new ArgumentException("At least one of --poe1 or --poe2 must be provided for download-ninja.");
        }

        if (!string.IsNullOrWhiteSpace(options.Value.Poe1League))
        {
            await DownloadForGame(game: "poe1", options.Value.Poe1League);
        }

        if (!string.IsNullOrWhiteSpace(options.Value.Poe2League))
        {
            await DownloadForGame(game: "poe2", options.Value.Poe2League);
        }

        return;

        async Task DownloadForGame(string game, string league)
        {
            await DownloadExchange(game, league);
            await DownloadStash(game, league);
        }

        async Task DownloadExchange(string game, string league)
        {
            var exchangeItems = new List<NinjaExchangeItem>();
            var exchangePages = game == "poe1" ? Poe1Pages : Poe2Pages;

            await Task.WhenAll(exchangePages.Select(async page =>
            {
                if (!page.SupportsExchange) return;

                var url =
                    $"https://poe.ninja/{game}/api/economy/exchange/current/overview?league={league.Replace(' ', '+')}&type={page.Type}";
                try
                {
                    using var http = new HttpClient();
                    http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Sidekick", "1.0"));
                    http.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");

                    logger.LogInformation($"GET {url}");
                    using var response = await http.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    await using var s = await response.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<ApiExchangeOverview>(s, JsonOptions);
                    if (result?.Items != null)
                    {
                        lock (exchangeItems)
                        {
                            foreach (var it in result.Items)
                            {
                                if (string.IsNullOrWhiteSpace(it.Id)) continue;
                                exchangeItems.Add(new NinjaExchangeItem(it.Id!, it.DetailsId, page));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed exchange {url}");
                }
            }));

            await dataFileWriter.Write(game, "ninja", GetFileName("exchange"), exchangeItems);
        }

        async Task DownloadStash(string game, string league)
        {
            var stashItems = new List<NinjaStashItem>();
            var stashPages = game == "poe1" ? Poe1Pages : Poe2Pages;

            await Task.WhenAll(stashPages.Select(async page =>
            {
                if (!page.SupportsStash || page.SupportsExchange) return;

                var url =
                    $"https://poe.ninja/{game}/api/economy/stash/current/item/overview?league={league.Replace(' ', '+')}&type={page.Type}";
                try
                {
                    using var http = new HttpClient();
                    http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Sidekick", "1.0"));
                    http.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");

                    logger.LogInformation($"GET {url}");
                    using var response = await http.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    await using var s = await response.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<ApiStashOverview>(s, JsonOptions);
                    if (result?.Lines != null)
                    {
                        lock (stashItems)
                        {
                            foreach (var it in result.Lines)
                            {
                                if (string.IsNullOrWhiteSpace(it.Name)) continue;
                                stashItems.Add(new NinjaStashItem(
                                    it.Name!, it.DetailsId, it.Corrupted, it.GemLevel, it.GemQuality,
                                    it.MapTier, it.Links, it.LevelRequired, it.Variant, page));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed stash {url}");
                }
            }));

            await dataFileWriter.Write(game, "ninja", GetFileName("stash"), stashItems);
        }
    }

    private sealed record ApiExchangeOverview(ApiOverviewCore? Core, List<ApiExchangeItem> Items);

    private sealed record ApiOverviewCore(string? Primary);

    private sealed record ApiExchangeItem(string? Id, string? DetailsId);

    private sealed record ApiStashOverview(List<ApiStashLine> Lines);

    private sealed record ApiStashLine(
        string? Name,
        string? DetailsId,
        bool? Corrupted,
        int? GemLevel,
        int? GemQuality,
        int? MapTier,
        int? Links,
        int? LevelRequired,
        string? Variant);
}