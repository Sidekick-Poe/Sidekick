using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Common.Settings;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public class InvariantStatsProvider
(
    TradeDataProvider tradeDataProvider,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IInvariantStatsProvider
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
        var result = await GetList();
        InitializeIgnore(result);
        InitializeIncursionRooms(result);
        InitializeLogbookFactions(result);
        InitializeClusterJewel(result);
        InitializeWeaponDamageIds(result);
    }

    private void InitializeIgnore(List<TradeStatCategory> apiCategories)
    {
        IgnoreStatIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            IgnoreStatIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("#% chance for dropped Maps to convert to")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeWeaponDamageIds(List<TradeStatCategory> apiCategories)
    {
        FireWeaponDamageIds.Clear();
        ColdWeaponDamageIds.Clear();
        LightningWeaponDamageIds.Clear();
        foreach (var apiCategory in apiCategories)
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

    private void InitializeIncursionRooms(List<TradeStatCategory> apiCategories)
    {
        IncursionRoomStatIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            IncursionRoomStatIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("Has Room: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeLogbookFactions(List<TradeStatCategory> apiCategories)
    {
        LogbookFactionStatIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            LogbookFactionStatIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("Has Logbook Faction: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeClusterJewel(List<TradeStatCategory> apiCategories)
    {
        foreach (var apiCategory in apiCategories)
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

    private static bool IsCategory(TradeStatCategory apiCategory, string? key)
    {
        var first = apiCategory.Entries.FirstOrDefault();
        return first?.Id.Split('.')[0] == key;
    }

    public async Task<List<TradeStatCategory>> GetList()
    {
        var game = await settingsService.GetGame();
        var apiCategories = await tradeDataProvider.GetStats(game, gameLanguageProvider.InvariantLanguage.Code);

        apiCategories.ForEach(category =>
        {
            category.Entries.ForEach(entry =>
            {
                entry.Text = entry.Text.RemoveSquareBrackets();
            });
        });

        return apiCategories;
    }
}
