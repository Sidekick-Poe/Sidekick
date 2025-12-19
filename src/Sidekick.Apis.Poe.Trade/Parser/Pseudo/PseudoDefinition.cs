using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Modifiers.Models;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public abstract class PseudoDefinition
{
    private static readonly Regex ParseHashPattern = new("\\#");

    protected abstract bool Enabled { get; }

    protected abstract string ModifierId { get; }

    protected abstract List<PseudoPattern> Patterns { get; }

    /// <summary>
    /// Represents a regular expression pattern used to exclude certain modifier texts
    /// during the processing of pseudo-modifier definitions in the Path of Exile API.
    /// </summary>
    /// <remarks>
    /// This property defines a regular expression that matches modifier texts
    /// which should be excluded from further processing. Each derived class provides
    /// a specific implementation of this property. It is utilized within the
    /// initialization and parsing processes to filter out unwanted modifier entries.
    /// </remarks>
    protected abstract Regex? Exception { get; }

    private string? Text { get; set; }

    private List<PseudoModifierDefinition> Modifiers { get; set; } = new();

    internal void InitializeDefinition(List<ApiCategory> apiCategories, List<ModifierDefinition>? localizedPseudoModifiers)
    {
        if (!Enabled) return;

        foreach (var apiModifier in apiCategories.SelectMany(apiCategory => apiCategory.Entries))
        {
            if (Exception != null && Exception.IsMatch(apiModifier.Text)) continue;

            foreach (var pattern in Patterns)
            {
                if (apiModifier.Id == null || apiModifier.Type == null || apiModifier.Text == null || !pattern.Pattern.IsMatch(apiModifier.Text)) continue;

                Modifiers.Add(new PseudoModifierDefinition(apiModifier.Id, apiModifier.Type, apiModifier.Text, pattern.Multiplier));
            }
        }

        Modifiers = Modifiers.OrderBy(x => x.Type switch
            {
                "pseudo" => 0,
                "explicit" => 1,
                "implicit" => 2,
                "crafted" => 3,
                "enchant" => 4,
                "fractured" => 5,
                "veiled" => 6,
                "desecrated" => 7,
                _ => 8,
            })
            .ThenBy(x => x.Text)
            .ToList();

        Text = localizedPseudoModifiers?.FirstOrDefault(x => x.ApiId == ModifierId)?.ApiText ?? null;
    }

    internal PseudoModifier? Parse(List<Modifier> itemModifierLines)
    {
        if (!Enabled || !HasPseudoMods(itemModifierLines))
        {
            return null;
        }

        var result = new PseudoModifier()
        {
            ModifierId = ModifierId,
            Text = Text ?? string.Empty,
        };

        foreach (var itemModifierLine in itemModifierLines)
        {
            foreach (var definitionModifier in Modifiers)
            {
                if (itemModifierLine.ApiInformation.All(itemModifier => definitionModifier.Id != itemModifier.ApiId) || itemModifierLine.AverageValue == 0)
                {
                    continue;
                }

                result.Value += itemModifierLine.AverageValue * definitionModifier.Multiplier;
                break;
            }
        }

        result.Value = (int)result.Value;
        result.Text = ParseHashPattern
            .Replace(result.Text, ((int)result.Value).ToString(), 1)
            .Replace("+-", "-");
        return result;
    }

    private bool HasPseudoMods(List<Modifier> itemModifierLines)
    {
        foreach (var definitionModifier in Modifiers)
        {
            foreach (var itemModifierLine in itemModifierLines)
            {
                foreach (var modifier in itemModifierLine.ApiInformation)
                {
                    if (modifier.ApiId == definitionModifier.Id) return true;
                }
            }
        }

        return false;
    }
}
