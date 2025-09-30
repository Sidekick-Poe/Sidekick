using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Modifiers.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Modifiers;

public class InvariantModifierProvider
(
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IInvariantModifierProvider
{
    public List<string> IgnoreModifierIds { get; } = [];

    public List<string> IncursionRoomModifierIds { get; } = [];

    public List<string> LogbookFactionModifierIds { get; } = [];

    public List<string> FireWeaponDamageIds { get; } = [];

    public List<string> ColdWeaponDamageIds { get; } = [];

    public List<string> LightningWeaponDamageIds { get; } = [];

    public string ClusterJewelSmallPassiveCountModifierId { get; private set; } = string.Empty;

    public string ClusterJewelSmallPassiveGrantModifierId { get; private set; } = string.Empty;

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
        IgnoreModifierIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            IgnoreModifierIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("#% chance for dropped Maps to convert to")).Select(x => x.Id).ToList());
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

            foreach (var apiModifier in apiCategory.Entries)
            {
                var text = apiModifier.Text.RemoveSquareBrackets();

                if (text == "Adds # to # Fire Damage") FireWeaponDamageIds.Add(apiModifier.Id);
                if (text == "Adds # to # Cold Damage") ColdWeaponDamageIds.Add(apiModifier.Id);
                if (text == "Adds # to # Lightning Damage") LightningWeaponDamageIds.Add(apiModifier.Id);
            }
        }
    }

    private void InitializeIncursionRooms(List<ApiCategory> apiCategories)
    {
        IncursionRoomModifierIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            IncursionRoomModifierIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("Has Room: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeLogbookFactions(List<ApiCategory> apiCategories)
    {
        LogbookFactionModifierIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "pseudo")) { continue; }

            LogbookFactionModifierIds.AddRange(apiCategory.Entries.Where(x => x.Text.StartsWith("Has Logbook Faction: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeClusterJewel(List<ApiCategory> apiCategories)
    {
        foreach (var apiCategory in apiCategories)
        {
            if (!IsCategory(apiCategory, "enchant")) { continue; }

            foreach (var apiModifier in apiCategory.Entries)
            {
                if (apiModifier.Text == "Adds # Passive Skills")
                {
                    ClusterJewelSmallPassiveCountModifierId = apiModifier.Id;
                }

                if (apiModifier.Text != "Added Small Passive Skills grant: #")
                {
                    continue;
                }

                ClusterJewelSmallPassiveGrantModifierId = apiModifier.Id;
                if (apiModifier.Option == null)
                {
                    return;
                }

                ClusterJewelSmallPassiveGrantOptions = apiModifier.Option.Options.ToDictionary(x => x.Id, x => x.Text!);
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
        var cacheKey = $"{game.GetValueAttribute()}_InvariantModifiers";

        var apiCategories = await cacheProvider.GetOrSet(cacheKey,
                                            async () =>
                                            {
                                                var result = await tradeApiClient.FetchData<ApiCategory>(game, gameLanguageProvider.InvariantLanguage, "stats");
                                                return result.Result;
                                            }, (cache) => cache.Any());
        if (apiCategories == null) throw new SidekickException("Could not fetch modifiers from the trade API.");

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
