using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser.Pseudo;

public abstract class PseudoDefinition
{
    protected abstract bool Enabled { get; }

    protected abstract string? ModifierId { get; }

    protected abstract List<PseudoPattern> Patterns { get; }

    protected abstract Regex? Exception { get; }

    public string Text => Modifiers.FirstOrDefault()?.Text ?? string.Empty;

    public List<PseudoModifierDefinition> Modifiers { get; set; } = new();

    internal void AddModifierIfMatch(ApiModifier entry)
    {
        if (!Enabled)
        {
            return;
        }

        if (Exception != null && Exception.IsMatch(entry.Text))
        {
            return;
        }

        foreach (var pattern in Patterns)
        {
            if (entry.Id == null || entry.Type == null || entry.Text == null)
            {
                continue;
            }

            if (pattern.Pattern.IsMatch(entry.Text))
            {
                Modifiers.Add(new PseudoModifierDefinition(entry.Id, entry.Type, entry.Text, pattern.Multiplier));
            }
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
            PseudoModifierId = ModifierId,
            Text = Text,
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
                if (itemModifierLine.Modifiers.All(itemModifier => definitionModifier.Id != itemModifier.Id))
                {
                    continue;
                }

                result.Value += itemModifierLine.Values.Average() * definitionModifier.Multiplier;
                break;
            }
        }

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
                    if (modifier.Id == definitionModifier.Id) return true;
                }
            }
        }

        return false;
    }
}
