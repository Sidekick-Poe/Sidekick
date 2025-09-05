using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Modifiers.Models;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
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
    public Dictionary<ModifierCategory, List<ApiModifier>> Categories { get; } = [];

    public List<string> IgnoreModifierIds { get; } = [];

    public List<string> IncursionRoomModifierIds { get; } = [];

    public List<string> LogbookFactionModifierIds { get; } = [];

    public List<string> FireWeaponDamageIds { get; } = [];

    public List<string> ColdWeaponDamageIds { get; } = [];

    public List<string> LightningWeaponDamageIds { get; } = [];

    public string ClusterJewelSmallPassiveCountModifierId { get; private set; } = string.Empty;

    public string ClusterJewelSmallPassiveGrantModifierId { get; private set; } = string.Empty;

    public Dictionary<int, string> ClusterJewelSmallPassiveGrantOptions { get; private set; } = [];

    public Dictionary<Type, string> PseudoModifierIds { get; } = [];

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantModifiers";

        var apiCategories = await cacheProvider.GetOrSet(cacheKey, async () => await tradeApiClient.FetchData<ApiCategory>(game, gameLanguageProvider.InvariantLanguage, "stats"), (cache) => cache.Result.Any());

        foreach (var apiCategory in apiCategories.Result)
        {
            var modifierCategory = apiCategory.Entries[0].Id.GetModifierCategory();

            apiCategory.Entries.ForEach(entry =>
            {
                entry.Text = ModifierProvider.RemoveSquareBrackets(entry.Text);
            });
            if (!Categories.TryAdd(modifierCategory, apiCategory.Entries))
            {
                Categories[modifierCategory].AddRange(apiCategory.Entries);
            }
        }

        InitializeIgnore();
        InitializeIncursionRooms();
        InitializeLogbookFactions();
        InitializeClusterJewel();
        InitializeWeaponDamageIds();
        InitializePseudoModifierIds();
    }

    private void InitializeIgnore()
    {
        IgnoreModifierIds.Clear();
        foreach (var apiCategory in Categories)
        {
            if (apiCategory.Key != ModifierCategory.Pseudo) { continue; }

            IgnoreModifierIds.AddRange(apiCategory.Value.Where(x => x.Text.StartsWith("#% chance for dropped Maps to convert to")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeWeaponDamageIds()
    {
        FireWeaponDamageIds.Clear();
        ColdWeaponDamageIds.Clear();
        LightningWeaponDamageIds.Clear();
        foreach (var apiCategory in Categories)
        {
            if (apiCategory.Key == ModifierCategory.Pseudo) { continue; }

            foreach (var apiModifier in apiCategory.Value)
            {
                var text = ModifierProvider.RemoveSquareBrackets(apiModifier.Text);

                if (text == "Adds # to # Fire Damage") FireWeaponDamageIds.Add(apiModifier.Id);
                if (text == "Adds # to # Cold Damage") ColdWeaponDamageIds.Add(apiModifier.Id);
                if (text == "Adds # to # Lightning Damage") LightningWeaponDamageIds.Add(apiModifier.Id);
            }
        }
    }

    private void InitializeIncursionRooms()
    {
        IncursionRoomModifierIds.Clear();
        foreach (var apiCategory in Categories)
        {
            if (apiCategory.Key != ModifierCategory.Pseudo) { continue; }

            IncursionRoomModifierIds.AddRange(apiCategory.Value.Where(x => x.Text.StartsWith("Has Room: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeLogbookFactions()
    {
        LogbookFactionModifierIds.Clear();
        foreach (var apiCategory in Categories)
        {
            if (apiCategory.Key != ModifierCategory.Pseudo) { continue; }

            LogbookFactionModifierIds.AddRange(apiCategory.Value.Where(x => x.Text.StartsWith("Has Logbook Faction: ")).Select(x => x.Id).ToList());
        }
    }

    private void InitializeClusterJewel()
    {
        foreach (var apiCategory in Categories)
        {
            if (apiCategory.Key != ModifierCategory.Enchant) { continue; }

            foreach (var apiModifier in apiCategory.Value)
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

    private void InitializePseudoModifierIds()
    {
        PseudoModifierIds.Clear();

        foreach (var apiCategory in Categories)
        {
            if (apiCategory.Key != ModifierCategory.Explicit) { continue; }

            foreach (var apiModifier in apiCategory.Value)
            {
                switch (apiModifier.Text)
                {
                    case "#% to Chaos Resistance": PseudoModifierIds.TryAdd(typeof(ChaosResistancesDefinition), apiModifier.Id); break;
                    case "#% to all Elemental Resistances": PseudoModifierIds.TryAdd(typeof(ElementalResistancesDefinition), apiModifier.Id); break;
                    case "# to Dexterity": PseudoModifierIds.TryAdd(typeof(DexterityDefinition), apiModifier.Id); break;
                    case "# to Strength": PseudoModifierIds.TryAdd(typeof(StrengthDefinition), apiModifier.Id); break;
                    case "# to Intelligence": PseudoModifierIds.TryAdd(typeof(IntelligenceDefinition), apiModifier.Id); break;
                    case "# to maximum Life": PseudoModifierIds.TryAdd(typeof(LifeDefinition), apiModifier.Id); break;
                    case "# to maximum Mana": PseudoModifierIds.TryAdd(typeof(ManaDefinition), apiModifier.Id); break;
                }
            }
        }
    }
}
