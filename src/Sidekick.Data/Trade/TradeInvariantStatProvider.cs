using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Models.Raw;

namespace Sidekick.Data.Trade;

public class TradeInvariantStatProvider
(
    TradeDataProvider tradeDataProvider,
    IGameLanguageProvider gameLanguageProvider
) : IInitializableService
{
    public List<string> IgnoreStatIds { get; } = [];

    public List<string> IncursionRoomStatIds { get; } = [];

    public List<string> LogbookFactionStatIds { get; } = [];

    public List<string> FireWeaponDamageIds { get; } = [];

    public List<string> ColdWeaponDamageIds { get; } = [];

    public List<string> LightningWeaponDamageIds { get; } = [];

    public string ClusterJewelSmallPassiveCountStatId { get; private set; } = string.Empty;

    public string ClusterJewelSmallPassiveGrantStatId { get; private set; } = string.Empty;

    public Dictionary<int, string> ClusterJewelSmallPassiveGrantOptions { get; private set; } = [];

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var categories = await tradeDataProvider.GetRawStats(GameType.PathOfExile1, gameLanguageProvider.InvariantLanguage.Code);
        categories.AddRange(await tradeDataProvider.GetRawStats(GameType.PathOfExile2, gameLanguageProvider.InvariantLanguage.Code));

        categories.ForEach(category =>
        {
            category.Entries.ForEach(entry =>
            {
                entry.Text = entry.Text.RemoveSquareBrackets();
            });
        });

        InitializeIgnore(categories);
        InitializeIncursionRooms(categories);
        InitializeLogbookFactions(categories);
        InitializeClusterJewel(categories);
        InitializeWeaponDamageIds(categories);
    }

    private void InitializeIgnore(List<RawTradeStatCategory> categories)
    {
        IgnoreStatIds.Clear();
        foreach (var apiCategory in categories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            IgnoreStatIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("#% chance for dropped Maps to convert to")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeWeaponDamageIds(List<RawTradeStatCategory> categories)
    {
        FireWeaponDamageIds.Clear();
        ColdWeaponDamageIds.Clear();
        LightningWeaponDamageIds.Clear();
        foreach (var apiCategory in categories)
        {
            if (IsCategory(apiCategory, "pseudo")) { continue; }

            foreach (var apiStat in apiCategory.Entries)
            {
                var text = apiStat.Text.RemoveSquareBrackets();

                if (text == "Adds # to # Fire Damage") FireWeaponDamageIds.Add(apiStat.Id);
                if (text == "Adds # to # Cold Damage") ColdWeaponDamageIds.Add(apiStat.Id);
                if (text == "Adds # to # Lightning Damage") LightningWeaponDamageIds.Add(apiStat.Id);
            }
        }
    }

    private void InitializeIncursionRooms(List<RawTradeStatCategory> categories)
    {
        IncursionRoomStatIds.Clear();
        foreach (var apiCategory in categories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            IncursionRoomStatIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("Has Room: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeLogbookFactions(List<RawTradeStatCategory> categories)
    {
        LogbookFactionStatIds.Clear();
        foreach (var apiCategory in categories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            LogbookFactionStatIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("Has Logbook Faction: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeClusterJewel(List<RawTradeStatCategory> categories)
    {
        foreach (var apiCategory in categories)
        {
            if (!IsCategory(apiCategory, "enchant")) { continue; }

            foreach (var apiStat in apiCategory.Entries)
            {
                if (apiStat.Text == "Adds # Passive Skills")
                {
                    ClusterJewelSmallPassiveCountStatId = apiStat.Id;
                }

                if (apiStat.Text != "Added Small Passive Skills grant: #")
                {
                    continue;
                }

                ClusterJewelSmallPassiveGrantStatId = apiStat.Id;
                if (apiStat.Option == null)
                {
                    return;
                }

                ClusterJewelSmallPassiveGrantOptions = apiStat.Option.Options.ToDictionary(x => x.Id, x => x.Text!);
            }
        }
    }

    private static bool IsCategory(RawTradeStatCategory apiCategory, string? key)
    {
        var first = apiCategory.Entries.FirstOrDefault();
        return first?.Id.Split('.')[0] == key;
    }
}
