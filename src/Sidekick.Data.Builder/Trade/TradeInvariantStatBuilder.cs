using Sidekick.Data.Extensions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Raw;

namespace Sidekick.Data.Builder.Trade;

public class TradeInvariantStatBuilder
(
    IGameLanguageProvider gameLanguageProvider,
    DataProvider dataProvider
)
{
    public async Task Build()
    {
        await BuildForGame(GameType.PathOfExile1);
        await BuildForGame(GameType.PathOfExile2);
    }

    private async Task BuildForGame(GameType game)
    {
        var categories = await dataProvider.Read<RawTradeResult<List<RawTradeStatCategory>>>(game, DataType.TradeRawStats, gameLanguageProvider.InvariantLanguage);
        categories.Result.ForEach(category =>
        {
            category.Entries.ForEach(entry =>
            {
                entry.Text = entry.Text.RemoveSquareBrackets();
            });
        });

        var model = new TradeInvariantStats()
        {
            IgnoreStatIds = GetIgnoreStatIds(categories.Result).ToList(),
            FireWeaponDamageIds = GetFireWeaponDamageIds(categories.Result).ToList(),
            ColdWeaponDamageIds = GetColdWeaponDamageIds(categories.Result).ToList(),
            LightningWeaponDamageIds = GetLightningWeaponDamageIds(categories.Result).ToList(),
            IncursionRoomStatIds = GetIncursionRooms(categories.Result).ToList(),
            LogbookFactionStatIds = GetLogbookFactions(categories.Result).ToList(),
            ClusterJewelSmallPassiveCountStatId = GetClusterPassiveCountId(categories.Result),
            ClusterJewelSmallPassiveGrantStatId = GetClusterGrantId(categories.Result),
            ClusterJewelSmallPassiveGrantOptions = GetClusterJewels(categories.Result),
        };

        await dataProvider.Write(game, DataType.TradeStats, model);
    }

    private IEnumerable<string> GetIgnoreStatIds(List<RawTradeStatCategory> categories)
    {
        foreach (var category in categories)
        {
            if (!IsCategory(category, "pseudo")) continue;

            foreach (var entry in category.Entries)
            {
                if (entry.Text.StartsWith("#% chance for dropped Maps to convert to")) yield return entry.Id;
            }
        }
    }

    private IEnumerable<string> GetFireWeaponDamageIds(List<RawTradeStatCategory> categories)
    {
        foreach (var category in categories)
        {
            if (IsCategory(category, "pseudo")) continue;

            foreach (var entry in category.Entries)
            {
                var text = entry.Text.RemoveSquareBrackets();
                if (text == "Adds # to # Fire Damage") yield return entry.Id;
            }
        }
    }

    private IEnumerable<string> GetColdWeaponDamageIds(List<RawTradeStatCategory> categories)
    {
        foreach (var category in categories)
        {
            if (IsCategory(category, "pseudo")) continue;

            foreach (var entry in category.Entries)
            {
                var text = entry.Text.RemoveSquareBrackets();
                if (text == "Adds # to # Cold Damage") yield return entry.Id;
            }
        }
    }

    private IEnumerable<string> GetLightningWeaponDamageIds(List<RawTradeStatCategory> categories)
    {
        foreach (var category in categories)
        {
            if (IsCategory(category, "pseudo")) continue;

            foreach (var entry in category.Entries)
            {
                var text = entry.Text.RemoveSquareBrackets();
                if (text == "Adds # to # Lightning Damage") yield return entry.Id;
            }
        }
    }

    private IEnumerable<string> GetIncursionRooms(List<RawTradeStatCategory> categories)
    {
        foreach (var category in categories)
        {
            if (!IsCategory(category, "pseudo")) continue;

            foreach (var entry in category.Entries)
            {
                if (entry.Text.StartsWith("Has Room: ")) yield return entry.Id;
            }
        }
    }

    private IEnumerable<string> GetLogbookFactions(List<RawTradeStatCategory> categories)
    {
        foreach (var category in categories)
        {
            if (!IsCategory(category, "pseudo")) continue;

            foreach (var entry in category.Entries)
            {
                if (entry.Text.StartsWith("Has Logbook Faction: ")) yield return entry.Id;
            }
        }
    }

    private string GetClusterPassiveCountId(List<RawTradeStatCategory> categories)
    {
        foreach (var category in categories)
        {
            if (!IsCategory(category, "enchant")) { continue; }

            foreach (var entry in category.Entries)
            {
                if (entry.Text == "Adds # Passive Skills")
                {
                    return entry.Id;
                }
            }
        }

        return string.Empty;
    }

    private string GetClusterGrantId(List<RawTradeStatCategory> categories)
    {
        foreach (var category in categories)
        {
            if (!IsCategory(category, "enchant")) { continue; }

            foreach (var entry in category.Entries)
            {
                if (entry.Text == "Added Small Passive Skills grant: #")
                {
                    return entry.Id;
                }
            }
        }

        return string.Empty;
    }

    private Dictionary<int, string> GetClusterJewels(List<RawTradeStatCategory> categories)
    {
        foreach (var category in categories)
        {
            if (!IsCategory(category, "enchant")) { continue; }

            foreach (var entry in category.Entries)
            {
                if (entry.Text != "Added Small Passive Skills grant: #") continue;
                if (entry.Options == null) continue;

                return entry.Options.Options.ToDictionary(x => x.Id, x => x.Text!);
            }
        }

        return [];
    }

    private static bool IsCategory(RawTradeStatCategory apiCategory, string? key)
    {
        var first = apiCategory.Entries.FirstOrDefault();
        return first?.Id.Split('.')[0] == key;
    }
}
