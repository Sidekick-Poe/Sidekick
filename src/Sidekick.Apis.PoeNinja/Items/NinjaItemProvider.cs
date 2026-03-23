using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Ninja;
namespace Sidekick.Apis.PoeNinja.Items;

public class NinjaItemProvider(
    ISettingsService settingsService,
    DataProvider dataProvider,
    IGameLanguageProvider gameLanguageProvider) : INinjaItemProvider
{
    private List<NinjaExchangeItem> ExchangeItems { get; } = [];

    private List<NinjaStashItem> StashItems { get; } = [];

    public int Priority => 100;

    private GameType Game { get; set; }

    public async Task Initialize()
    {
        Game = await settingsService.GetGame();

        StashItems.Clear();
        var stashItems = await dataProvider.Read<List<NinjaStashItem>>(Game, DataType.NinjaStash, gameLanguageProvider.InvariantLanguage);
        StashItems.AddRange(stashItems);

        ExchangeItems.Clear();
        var exchangeItems = await dataProvider.Read<List<NinjaExchangeItem>>(Game, DataType.NinjaExchange, gameLanguageProvider.InvariantLanguage);
        ExchangeItems.AddRange(exchangeItems);
    }

    public NinjaExchangeItem? GetExchangeItem(ItemDefinition item)
    {
        if (string.IsNullOrEmpty(item.BaseItem?.Name)) return null;
        var ninjaItem = ExchangeItems.FirstOrDefault(x => x.Id == item.BaseItem.Name);
        if (ninjaItem != null) return ninjaItem;

        // The PoE1 api doesn't have chaos currency, so we need to add it manually.
        if (Game == GameType.PathOfExile1 && item.BaseItem.Name == "Chaos Orb")
        {
            var page = new NinjaPage("Currency", "currency", true, true);
            return new NinjaExchangeItem("chaos", "divine-orb", page);
        }

        return null;
    }

    public NinjaStashItem? GetStashItem(Item item)
    {
        if (item.Properties.Rarity == Rarity.Unique)
        {
            return GetUniqueItem(item.Invariant, item.Properties.GetMaximumNumberOfLinks());
        }

        if (item.Properties.ItemClass is ItemClass.ActiveGem or ItemClass.SupportGem)
        {
            return GetGemItem(item.Invariant, item.Properties.GemLevel, item.Properties.Quality, item.Properties.Corrupted);
        }

        if (item.Properties.MapTier != 0)
        {
            return GetMapItem(item.Invariant, item.Properties.MapTier);
        }

        if (item.Properties.ClusterJewelPassiveCount.HasValue && item.Properties.ClusterJewelPassiveCount != 0)
        {
            return GetClusterItem(item.Properties.ClusterJewelGrantText, item.Properties.ClusterJewelPassiveCount.Value, item.Properties.ItemLevel);
        }

        return GetBaseTypeItem(item.Invariant, item.Properties.ItemLevel, item.Properties.Influences);
    }

    public NinjaStashItem? GetUniqueItem(ItemDefinition item, int links)
    {
        if (string.IsNullOrEmpty(item.UniqueItem?.Name)) return null;

        if (links < 5) links = 0;

        return StashItems
            .Where(x => x.Name == item.UniqueItem.Name)
            .OrderBy(x => x.Links == links || (links == 0 && !x.Links.HasValue) ? 0 : 1)
            .FirstOrDefault();
    }

    public NinjaStashItem? GetGemItem(ItemDefinition item, int gemLevel, int gemQuality, bool corrupted)
    {
        var name = item.TradeItem?.Text;
        name ??= item.BaseItem?.Name;
        if (string.IsNullOrEmpty(name)) return null;

        if (gemLevel > 7 && gemLevel < 20) gemLevel = 1;

        if (gemQuality < 20) gemQuality = 0;
        else if (gemQuality < 23) gemQuality = 20;
        else gemQuality = 23;

        var items = StashItems
            .Where(x => x.Name == name)
            .Where(x => x.GemLevel == gemLevel || (gemLevel == 0 && !x.GemLevel.HasValue))
            .Where(x => x.GemQuality == gemQuality || (gemQuality == 0 && !x.GemQuality.HasValue))
            .Where(x => x.Corrupted == corrupted || (!corrupted && !x.Corrupted.HasValue));

        return items.FirstOrDefault();
    }

    public NinjaStashItem? GetMapItem(ItemDefinition item, int mapTier)
    {
        if (string.IsNullOrEmpty(item.BaseItem?.Name)) return null;

        var name = item.BaseItem.Name;
        if (name == "Blighted Map") name = $"Blighted Map (Tier {mapTier})";
        if (name == "Blight-ravaged Map") name = $"Blight-ravaged Map (Tier {mapTier})";

        var items = StashItems.Where(x => x.Name == name);
        return items.FirstOrDefault();
    }

    public NinjaStashItem? GetClusterItem(string? grantText, int passiveCount, int itemLevel)
    {
        if (grantText == null) return null;

        if (itemLevel < 50) itemLevel = 1;
        else if (itemLevel < 68) itemLevel = 50;
        else if (itemLevel < 75) itemLevel = 68;
        else if (itemLevel < 84) itemLevel = 75;
        else itemLevel = 84;

        var items = StashItems
            .Where(x => x.Name == grantText)
            .Where(x => x.Variant == $"{passiveCount} passives")
            .Where(x => x.ItemLevel == itemLevel);

        return items.FirstOrDefault();
    }

    public NinjaStashItem? GetBaseTypeItem(ItemDefinition item, int itemLevel, Influences influences)
    {
        if (string.IsNullOrEmpty(item.BaseItem?.Name)) return null;

        var variants = GetVariants().ToList();
        if (itemLevel > 86) itemLevel = 86;
        else if (itemLevel < 82) itemLevel = 0;

        var items = StashItems
            .Where(x => x.Name == item.BaseItem.Name)
            .Where(x => (x.Variant == null && variants.Count == 0) || (x.Variant != null && variants.Contains(x.Variant)))
            .Where(x => x.ItemLevel == itemLevel || (itemLevel == 0 && !x.ItemLevel.HasValue));

        return items.FirstOrDefault();

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
                var values = permutation.ToList();
                if (values.Count != 0) yield return string.Join("/", values);
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
