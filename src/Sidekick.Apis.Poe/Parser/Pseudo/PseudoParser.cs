using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Pseudo.Definitions;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Pseudo;

public class PseudoParser
(
    IInvariantModifierProvider invariantModifierProvider,
    IModifierProvider modifierProvider,
    ISettingsService settingsService
) : IPseudoParser
{
    private List<PseudoDefinition> Definitions { get; } = new();

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();

        Definitions.Clear();
        Definitions.AddRange([
            new ElementalResistancesDefinition(game),
            new ChaosResistancesDefinition(game),
            new StrengthDefinition(game),
            new IntelligenceDefinition(game),
            new DexterityDefinition(game),
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

    public List<PseudoModifier> Parse(List<ModifierLine> lines)
    {
        var results = new List<PseudoModifier>();

        foreach (var definition in Definitions)
        {
            var result = definition.Parse(lines);
            if (result != null) results.Add(result);
        }

        return results;
    }
}
