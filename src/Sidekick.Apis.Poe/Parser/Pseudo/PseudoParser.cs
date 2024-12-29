using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Pseudo.Definitions;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Pseudo;

public class PseudoParser
(
    IInvariantModifierProvider invariantModifierProvider,
    ISettingsService settingsService
) : IPseudoParser
{
    private readonly Regex parseHashPattern = new("\\#");

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
        foreach (var category in categories)
        {
            foreach (var entry in category.Entries)
            {
                foreach (var definition in Definitions)
                {
                    definition.AddModifierIfMatch(entry);
                }
            }
        }

        foreach (var definition in Definitions)
        {
            definition.Modifiers = definition.Modifiers.OrderBy(x => x.Type switch
                {
                    "pseudo" => 0,
                    "explicit" => 1,
                    "implicit" => 2,
                    "crafted" => 3,
                    "enchant" => 4,
                    "fractured" => 5,
                    "veiled" => 6,
                    _ => 7,
                })
                .ThenBy(x => x.Text)
                .ToList();
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

        results.ForEach(x =>
        {
            x.Value = (int)x.Value;
            x.Text = parseHashPattern.Replace(x.Text, ((int)x.Value).ToString(), 1);
        });

        return results;
    }
}
