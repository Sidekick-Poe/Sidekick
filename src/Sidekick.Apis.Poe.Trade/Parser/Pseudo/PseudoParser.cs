using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public class PseudoParser
(
    IInvariantModifierProvider invariantModifierProvider,
    IModifierProvider modifierProvider,
    ISettingsService settingsService
) : IPseudoParser
{
    private List<PseudoDefinition> Definitions { get; } = new();

    /// <inheritdoc/>
    public int Priority => 300;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions.Clear();
        Definitions.AddRange([
            new ElementalResistancesDefinition(),
            new ChaosResistancesDefinition(),
            new StrengthDefinition(),
            new IntelligenceDefinition(),
            new DexterityDefinition(),
            new LifeDefinition(game),
            new ManaDefinition(game),
        ]);

        var categories = await invariantModifierProvider.GetList();
        categories.RemoveAll(x => x.Entries.FirstOrDefault()?.Id.StartsWith("pseudo") == true);

        var localizedPseudoModifiers = modifierProvider.Definitions.GetValueOrDefault(ModifierCategory.Pseudo);

        foreach (var definition in Definitions)
        {
            definition.InitializeDefinition(categories, localizedPseudoModifiers);
        }
    }

    public void Parse(Item item)
    {
        item.PseudoModifiers.Clear();
        foreach (var definition in Definitions)
        {
            var result = definition.Parse(item.ModifierLines);
            if (result != null && !string.IsNullOrEmpty(result.Text)) item.PseudoModifiers.Add(result);
        }
    }

    public List<PseudoFilter> GetFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.Header.Category == Category.DivinationCard || item.Header.Category == Category.Gem || item.Header.Category == Category.ItemisedMonster || item.Header.Category == Category.Leaguestone || item.Header.Category == Category.Unknown || item.Header.Category == Category.Currency)
        {
            return [];
        }

        var result = new List<PseudoFilter>();
        foreach (var modifier in item.PseudoModifiers)
        {
            result.Add(new PseudoFilter()
            {
                PseudoModifier = modifier,
                Checked = false,
            });
        }

        return result;
    }
}
