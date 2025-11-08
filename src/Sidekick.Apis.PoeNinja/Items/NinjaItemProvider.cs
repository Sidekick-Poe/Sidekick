using System.Text.Json;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Items.Models;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.PoeNinja.Items;

public class NinjaItemProvider(ISettingsService settingsService) : INinjaItemProvider
{
    private List<NinjaExchangeItem> ExchangeItems { get; } = [];

    private List<NinjaStashItem> StashItems { get; } = [];

    public int Priority => 100;

    private GameType Game { get; set; }

    public async Task Initialize()
    {
        Game = await settingsService.GetGame();
        await LoadFile(Game, NinjaPageProvider.ExchangeType, ExchangeItems);
        await LoadFile(Game, NinjaPageProvider.StashType, StashItems);
    }

    private async Task LoadFile<TItem>(GameType game, string type, List<TItem> items)
    {
        var dataFilePath = Path.Combine(AppContext.BaseDirectory, "wwwroot/data/" + NinjaPageProvider.GetFileName(game, type));
        if (!File.Exists(dataFilePath)) return;

        items.Clear();

        await using var fileStream = File.OpenRead(dataFilePath);
        var result = await JsonSerializer.DeserializeAsync<List<TItem>>(fileStream, NinjaClient.JsonSerializerOptions);
        if (result == null) return;

        foreach (var item in result)
        {
            items.Add(item);
        }
    }

    public NinjaExchangeItem? GetExchangeItem(string? invariant)
    {
        if (string.IsNullOrEmpty(invariant)) return null;
        var item = ExchangeItems.FirstOrDefault(x=>x.Id == invariant);
        if (item != null) return item;

        // The PoE1 api doesn't have chaos currency, so we need to add it manually.
        if (Game == GameType.PathOfExile && invariant == "chaos")
        {
            var page = new NinjaPage("Currency", "currency", true, true);
            return new NinjaExchangeItem(invariant, "divine-orb", page);
        }

        return null;
    }

    public NinjaStashItem? GetStashItem(Item item)
    {
        if (item.Properties.Rarity == Rarity.Unique)
        {
            return GetUniqueItem(item.ApiInformation.InvariantName, item.Properties.GetMaximumNumberOfLinks());
        }

        if (item.Properties.ItemClass is ItemClass.ActiveGem or ItemClass.SupportGem)
        {
            return GetGemItem(item.ApiInformation.InvariantType, item.Properties.GemLevel, item.Properties.Quality);
        }

        if (item.Properties.MapTier != 0)
        {
            return GetMapItem(item.ApiInformation.InvariantType, item.Properties.MapTier);
        }

        if (item.Properties.ClusterJewelPassiveCount.HasValue && item.Properties.ClusterJewelPassiveCount != 0)
        {
            return GetClusterItem(item.Properties.ClusterJewelGrantText, item.Properties.ClusterJewelPassiveCount.Value, item.Properties.ItemLevel);
        }

        return GetBaseTypeItem(item.ApiInformation.InvariantType, item.Properties.ItemLevel, item.Properties.Influences);
    }

    public NinjaStashItem? GetUniqueItem(string? name, int links)
    {
        if (name == null) return null;

        if (links < 5) links = 0;

        return StashItems
            .Where(x => x.Name == name)
            .OrderBy(x => x.Links == links || (links == 0 && !x.Links.HasValue) ? 0 : 1)
            .FirstOrDefault();
    }

    public NinjaStashItem? GetGemItem(string? name, int gemLevel, int gemQuality)
    {
        if (name == null) return null;

        if (gemLevel > 7 && gemLevel < 20) gemLevel = 1;

        if (gemQuality < 20) gemQuality = 0;
        else if (gemQuality < 23) gemQuality = 20;
        else gemQuality = 23;

        return StashItems
            .Where(x => x.Name == name)
            .Where(x => x.GemLevel == gemLevel || (gemLevel == 0 && !x.GemLevel.HasValue))
            .Where(x => x.GemQuality == gemQuality || (gemQuality == 0 && !x.GemQuality.HasValue))
            .FirstOrDefault();
    }

    public NinjaStashItem? GetMapItem(string? name, int mapTier)
    {
        if (name == null) return null;

        return StashItems
            .Where(x => x.Name == name)
            .Where(x => x.MapTier == mapTier || (mapTier == 0 && !x.MapTier.HasValue))
            .FirstOrDefault();
    }

    public NinjaStashItem? GetClusterItem(string? grantText, int passiveCount, int itemLevel)
    {
        if (grantText == null) return null;

        if (itemLevel < 50) itemLevel = 1;
        else if (itemLevel < 68) itemLevel = 50;
        else if (itemLevel < 75) itemLevel = 68;
        else if (itemLevel < 84) itemLevel = 75;
        else itemLevel = 84;

        return StashItems
            .Where(x => x.Name == grantText)
            .Where(x => x.Variant == $"{passiveCount} passives")
            .Where(x => x.ItemLevel == itemLevel)
            .FirstOrDefault();
    }

    public NinjaStashItem? GetBaseTypeItem(string? name, int itemLevel, Influences influences)
    {
        if (name == null) return null;

        var variants = GetVariants().ToList();
        if (itemLevel > 86) itemLevel = 86;
        else if (itemLevel < 82) itemLevel = 0;

        return StashItems
            .Where(x => x.Name == name)
            .Where(x => (x.Variant == null && variants.Count == 0) || (x.Variant != null && variants.Contains(x.Variant)))
            .Where(x => x.ItemLevel == itemLevel || (itemLevel == 0 && !x.ItemLevel.HasValue))
            .FirstOrDefault();

        IEnumerable<string> GetVariants()
        {
            List<string> influenceNames = [];
            if (influences.Crusader) influenceNames.Add("Crusader");
            if (influences.Warlord) influenceNames.Add("Warlord");
            if (influences.Hunter) influenceNames.Add("Hunter");
            if (influences.Redeemer) influenceNames.Add("Redeemer");
            if (influences.Shaper) influenceNames.Add("Shaper");
            if (influences.Elder) influenceNames.Add("Elder");

            // Generate all permutations of the influences list
            foreach (var permutation in GetPermutations(influenceNames))
            {
                yield return string.Join("/", permutation);
            }

            yield break;

            IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> enumerable)
            {
                var list = enumerable.ToList();
                if (!list.Any())
                {
                    yield return [];
                    yield break;
                }

                foreach (var element in list)
                {
                    var remainingList = list.Where(x => !x!.Equals(element));
                    foreach (var permutation in GetPermutations(remainingList))
                    {
                        yield return new[]
                        {
                            element,
                        }.Concat(permutation);
                    }
                }
            }
        }
    }
}
