using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser.Pseudo;

public abstract class PseudoDefinition
{
    internal void AddModifierIfMatch(ApiModifier entry)
    {
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
        var hasPseudo = false;
        foreach (var definitionModifier in Modifiers)
        {
            foreach (var itemModifierLine in itemModifierLines)
            {
                foreach (var modifier in itemModifierLine.Modifiers)
                {
                    hasPseudo = hasPseudo || modifier.Id == definitionModifier.Id;
                    if (hasPseudo) break;
                }

                if (hasPseudo) break;
            }

            if (hasPseudo) break;
        }

        if (!hasPseudo)
        {
            return null;
        }

        var result = new PseudoModifier()
        {
            Text = Text,
        };

        foreach (var definitionModifier in Modifiers)
        {
            result.Modifiers.Add(definitionModifier.Id, definitionModifier.Multiplier);
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

    protected abstract List<PseudoPattern> Patterns { get; }

    protected abstract Regex? Exception { get; }

    public string Text => Modifiers.FirstOrDefault()?.Text ?? string.Empty;

    public List<PseudoModifierDefinition> Modifiers { get; set; } = new();
}
