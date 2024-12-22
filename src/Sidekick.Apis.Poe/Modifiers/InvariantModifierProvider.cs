using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Modifiers;

public class InvariantModifierProvider
(
    ICacheProvider cacheProvider,
    IPoeTradeClient poeTradeClient,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IInvariantModifierProvider
{
    public List<string> IncursionRoomModifierIds { get; } = [];

    public List<string> LogbookFactionModifierIds { get; } = [];

    public List<string> FireWeaponDamageIds { get; } = [];

    public List<string> ColdWeaponDamageIds { get; } = [];

    public List<string> LightningWeaponDamageIds { get; } = [];

    public string ClusterJewelSmallPassiveCountModifierId { get; private set; } = null!;

    public string ClusterJewelSmallPassiveGrantModifierId { get; private set; } = null!;

    public Dictionary<int, string> ClusterJewelSmallPassiveGrantOptions { get; private set; } = null!;

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var result = await GetList();
        InitializeIncursionRooms(result);
        InitializeLogbookFactions(result);
        InitializeClusterJewel(result);
        InitializeWeaponDamageIds(result);
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
                var text = ModifierProvider.RemoveSquareBrackets(apiModifier.Text);

                if (text == "Adds # to # Fire Damage") FireWeaponDamageIds.Add(apiModifier.Id);
                if (text == "Adds # to # Cold Damage") ColdWeaponDamageIds.Add(apiModifier.Id);
                if (text == "Adds # to # Lightning Damage") LightningWeaponDamageIds.Add(apiModifier.Id);
            }
        }
    }

    private bool IsCategory(ApiCategory apiCategory, string? key)
    {
        var first = apiCategory.Entries.FirstOrDefault();
        return first?.Id.Split('.')[0] == key;
    }

    public async Task<List<ApiCategory>> GetList()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantModifiers";

        return await cacheProvider.GetOrSet(cacheKey,
                                            async () =>
                                            {
                                                var result = await poeTradeClient.Fetch<ApiCategory>(game, gameLanguageProvider.InvariantLanguage, "data/stats");
                                                return result.Result;
                                            }, (cache) => cache.Any());
    }
}
