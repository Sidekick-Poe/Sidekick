using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiStats.Models;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public class InvariantStatsProvider
(
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
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

    private void InitializeIgnore(List<ApiCategory> apiCategories)
    {
        IgnoreStatIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            IgnoreStatIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("#% chance for dropped Maps to convert to")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeWeaponDamageIds(List<ApiCategory> apiCategories)
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

    private void InitializeIncursionRooms(List<ApiCategory> apiCategories)
    {
        IncursionRoomStatIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            IncursionRoomStatIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("Has Room: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeLogbookFactions(List<ApiCategory> apiCategories)
    {
        LogbookFactionStatIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            LogbookFactionStatIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("Has Logbook Faction: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeClusterJewel(List<ApiCategory> apiCategories)
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

    private static bool IsCategory(ApiCategory apiCategory, string? key)
    {
        var first = apiCategory.Entries.FirstOrDefault();
        return first?.Id.Split('.')[0] == key;
    }

    public async Task<List<ApiCategory>> GetList()
    {
        var game = await settingsService.GetGame();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantStats";

        var apiCategories = await cacheProvider.GetOrSet(cacheKey,
                                            async () =>
                                            {
                                                var result = await tradeApiClient.FetchData<ApiCategory>(game, gameLanguageProvider.InvariantLanguage, "stats");
                                                return result.Result;
                                            }, (cache) => cache.Any());
        if (apiCategories == null) throw new SidekickException("Could not fetch stats from the trade API.");

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
