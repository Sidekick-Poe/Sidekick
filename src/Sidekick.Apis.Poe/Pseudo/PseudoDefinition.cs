using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Modifiers.Models;

namespace Sidekick.Apis.Poe.Pseudo;

public abstract class PseudoDefinition
{
    public void AddModifierIfMatch(ApiModifier entry)
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

    protected abstract List<PseudoPattern> Patterns { get; }

    protected abstract Regex? Exception { get; }

    public string Text => Modifiers.FirstOrDefault()?.Text ?? string.Empty;

    public List<PseudoModifierDefinition> Modifiers { get; set; } = new();
}
