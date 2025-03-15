using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser.Pseudo;

public abstract class PseudoDefinition
{
    private static readonly Regex parseHashPattern = new("\\#");

    protected abstract bool Enabled { get; }

    protected abstract string? ModifierId { get; }

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

    public List<PseudoModifierDefinition> Modifiers { get; private set; } = new();

    internal void InitializeDefinition(List<ApiCategory> apiCategories, List<ModifierDefinition>? localizedPseudoModifiers)
    {
        foreach (var apiModifier in apiCategories.SelectMany(apiCategory => apiCategory.Entries))
        {
            if (!Enabled)
            {
                return;
            }

            if (Exception != null && apiModifier.Text is not null && Exception.IsMatch(apiModifier.Text))
            {
                continue;
            }

            foreach (var pattern in Patterns)
            {
                if (apiModifier.Id == null || apiModifier.Type == null || apiModifier.Text == null)
                {
                    continue;
                }

                if (pattern.Pattern.IsMatch(apiModifier.Text))
                {
                    Modifiers.Add(new PseudoModifierDefinition(apiModifier.Id, apiModifier.Type, apiModifier.Text, pattern.Multiplier));
                }
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
                _ => 7,
            })
            .ThenBy(x => x.Text)
            .ToList();

        if (localizedPseudoModifiers != null && !string.IsNullOrEmpty(ModifierId))
        {
            Text = localizedPseudoModifiers.FirstOrDefault(x => x.ApiId == ModifierId)?.ApiText ?? "";
        }

        if (string.IsNullOrEmpty(Text))
        {
            Text = string.Join(", ", Modifiers.Select(x => x.Text).Distinct().ToList());
        }
    }

    internal PseudoModifier? Parse(List<ModifierLine> itemModifierLines)
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

        if (string.IsNullOrEmpty(ModifierId))
        {
            foreach (var definitionModifier in Modifiers)
            {
                result.WeightedSumModifiers.Add(definitionModifier.Id, definitionModifier.Multiplier);
            }
        }

        foreach (var itemModifierLine in itemModifierLines)
        {
            foreach (var definitionModifier in Modifiers)
            {
                if (itemModifierLine.Modifiers.All(itemModifier => definitionModifier.Id != itemModifier.ApiId) || itemModifierLine.Values.Count == 0)
                {
                    continue;
                }

                result.Value += itemModifierLine.Values.Average() * definitionModifier.Multiplier;
                break;
            }
        }

        result.Value = (int)result.Value;
        result.Text = parseHashPattern.Replace(result.Text, ((int)result.Value).ToString(), 1);
        return result;
    }

    private bool HasPseudoMods(List<ModifierLine> itemModifierLines)
    {
        foreach (var definitionModifier in Modifiers)
        {
            foreach (var itemModifierLine in itemModifierLines)
            {
                foreach (var modifier in itemModifierLine.Modifiers)
                {
                    if (modifier.ApiId == definitionModifier.Id) return true;
                }
            }
        }

        return false;
    }
}
