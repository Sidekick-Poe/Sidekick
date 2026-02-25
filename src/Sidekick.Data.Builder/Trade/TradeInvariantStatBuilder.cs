using Sidekick.Data.Extensions;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models;
using Sidekick.Data.Trade.Models.Raw;

namespace Sidekick.Data.Builder.Trade;

public class TradeInvariantStatBuilder
(
    TradeDataProvider tradeDataProvider,
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
        var categories = await tradeDataProvider.GetRawStats(game, gameLanguageProvider.InvariantLanguage.Code);
        categories.ForEach(category =>
        {
            category.Entries.ForEach(entry =>
            {
                entry.Text = entry.Text.RemoveSquareBrackets();
            });
        });

        var model = new TradeInvariantStats()
        {
            IgnoreStatIds = GetIgnoreStatIds(categories).ToList(),
            FireWeaponDamageIds = GetFireWeaponDamageIds(categories).ToList(),
            ColdWeaponDamageIds = GetColdWeaponDamageIds(categories).ToList(),
            LightningWeaponDamageIds = GetLightningWeaponDamageIds(categories).ToList(),
            IncursionRoomStatIds = GetIncursionRooms(categories).ToList(),
            LogbookFactionStatIds = GetLogbookFactions(categories).ToList(),
            ClusterJewelSmallPassiveCountStatId = GetClusterPassiveCountId(categories),
            ClusterJewelSmallPassiveGrantStatId = GetClusterGrantId(categories),
            ClusterJewelSmallPassiveGrantOptions = GetClusterJewels(categories),
        };

        await dataProvider.Write(game, $"trade/stats.invariant.json", model);
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
