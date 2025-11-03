using System.Collections.Concurrent;
using System.Text.Json;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange;
using Sidekick.Apis.PoeNinja.Items.Models;
using Sidekick.Apis.PoeNinja.Stash;
namespace Sidekick.Apis.PoeNinja.Items;

public class NinjaPageProvider(
    INinjaExchangeProvider ninjaExchangeProvider,
    INinjaStashProvider ninjaStashProvider,
    INinjaItemProvider ninjaItemProvider) : INinjaPageProvider
{
    private static List<NinjaPage> Poe1Pages { get; } =
    [
        new NinjaPage("Currency", "currency", true, true),
        new NinjaPage("Fragment", "fragments", true, true),
        new NinjaPage("Runegraft", "runegrafts", true, true),
        new NinjaPage("AllflameEmber", "allflame-embers", true, true),
        new NinjaPage("Tattoo", "tattoos", true, true),
        new NinjaPage("Omen", "omens", true, true),
        new NinjaPage("DivinationCard", "divination-cards", true, true),
        new NinjaPage("Artifact", "artifacts", true, true),
        new NinjaPage("Oil", "oils", true, true),
        new NinjaPage("Incubator", "incubators", false, true),
        new NinjaPage("UniqueWeapon", "unique-weapons", false, true),
        new NinjaPage("UniqueArmour", "unique-armours", false, true),
        new NinjaPage("UniqueAccessory", "unique-accessories", false, true),
        new NinjaPage("UniqueFlask", "unique-flasks", false, true),
        new NinjaPage("UniqueJewel", "unique-jewels", false, true),
        new NinjaPage("UniqueTincture", "unique-tinctures", false, true),
        new NinjaPage("UniqueRelic", "unique-relics", false, true),
        new NinjaPage("SkillGem", "skill-gems", false, true),
        new NinjaPage("ClusterJewel", "cluster-jewels", false, true),
        new NinjaPage("Map", "maps", false, true),
        new NinjaPage("BlightedMap", "blighted-maps", false, true),
        new NinjaPage("BlightRavagedMap", "blight-ravaged-maps", false, true),
        new NinjaPage("UniqueMap", "unique-maps", false, true),
        new NinjaPage("DeliriumOrb", "delirium-orbs", true, true),
        new NinjaPage("Invitation", "invitations", false, true),
        new NinjaPage("Scarab", "scarabs", true, true),
        new NinjaPage("Memory", "memories", false, true),
        new NinjaPage("BaseType", "base-types", false, true),
        new NinjaPage("Fossil", "fossils", true, true),
        new NinjaPage("Resonator", "resonators", true, true),
        new NinjaPage("Beast", "beasts", false, true),
        new NinjaPage("Essence", "essences", true, true),
        new NinjaPage("Vial", "vials", false, true),
    ];

    private static List<NinjaPage> Poe2Pages { get; } =
    [
        new NinjaPage("Currency", "currency", true, false),
        new NinjaPage("Fragments", "fragments", true, false),
        new NinjaPage("Abyss", "abyssal-bones", true, false),
        new NinjaPage("UncutGems", "uncut-gems", true, false),
        new NinjaPage("LineageSupportGems", "lineage-support-gems", true, false),
        new NinjaPage("Essences", "essences", true, false),
        new NinjaPage("Ultimatum", "soul-cores", true, false),
        new NinjaPage("Talismans", "talismans", true, false),
        new NinjaPage("Runes", "runes", true, false),
        new NinjaPage("Ritual", "omens", true, false),
        new NinjaPage("Expedition", "expedition", true, false),
        new NinjaPage("Delirium", "distilled-emotions", true, false),
        new NinjaPage("Breach", "breach-catalyst", true, false),
    ];

    public async Task Download(string dataFolder)
    {
        await DownloadPages(GameType.PathOfExile, Poe1Pages);
        await DownloadPages(GameType.PathOfExile2, Poe2Pages);
        await ninjaItemProvider.Initialize();

        return;

        async Task DownloadPages(GameType game, List<NinjaPage> pages)
        {
            ConcurrentBag<NinjaPageItem> concurrentItems = [];
            var tasks = pages.Select(x => DownloadPage(game, x, concurrentItems)).ToList();
            await Task.WhenAll(tasks);

            var items = concurrentItems.DistinctBy(x => x.Name).ToList();
            await SaveToDisk(game, items);
        }

        async Task DownloadPage(GameType game, NinjaPage page, ConcurrentBag<NinjaPageItem> items)
        {
            List<Task> tasks = [];

            if (page.SupportsExchange) tasks.Add(DownloadExchange(game, page, items));
            else if (page.SupportsStash) tasks.Add(DownloadStash(game, page, items));

            await Task.WhenAll(tasks);
        }

        async Task DownloadExchange(GameType game, NinjaPage page, ConcurrentBag<NinjaPageItem> items)
        {
            var result = await ninjaExchangeProvider.FetchOverview(game, page.Type);
            foreach (var item in result.Items)
            {
                if (item.Id == null) continue;
                items.Add(new NinjaPageItem(item.Id, page));
            }
        }

        async Task DownloadStash(GameType game, NinjaPage page, ConcurrentBag<NinjaPageItem> items)
        {
            var result = await ninjaStashProvider.FetchOverview(game, page.Type);
            foreach (var item in result.Lines)
            {
                if (item.Name == null) continue;
                items.Add(new NinjaPageItem(item.Name, page));
            }
        }

        async Task SaveToDisk(GameType game, List<NinjaPageItem> items)
        {
            var fileName = GetFileName(game);
            var filePath = Path.Combine(dataFolder, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);// Ensure directory creation

            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(fileStream, items, NinjaClient.JsonSerializerOptions);
        }
    }

    internal static string GetFileName(GameType game)
    {
        return game switch
        {
            GameType.PathOfExile => "poe1.ninja.items.json",
            GameType.PathOfExile2 => "poe2.ninja.items.json",
            _ => throw new Exception(),
        };
    }
}
