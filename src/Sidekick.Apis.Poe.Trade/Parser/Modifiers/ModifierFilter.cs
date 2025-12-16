using Sidekick.Apis.Poe.Items;

using Sidekick.Apis.Poe.Trade.Parser.Filters;
namespace Sidekick.Apis.Poe.Trade.Parser.Modifiers;

public class ModifierFilter : IAutoSelectableFilter
{
    public ModifierFilter(Modifier line)
    {
        Line = line;
        Checked = line.ApiInformation.FirstOrDefault()?.Category == ModifierCategory.Fractured;

        var categories = line.ApiInformation.Select(x => x.Category).Distinct().ToList();
        if (categories.Any(x => x is ModifierCategory.Fractured or ModifierCategory.Desecrated or ModifierCategory.Crafted))
        {
            UsePrimaryCategory = true;
            PrimaryCategory = categories.FirstOrDefault(x => x is ModifierCategory.Fractured or ModifierCategory.Desecrated or ModifierCategory.Crafted);
            SecondaryCategory = categories.FirstOrDefault(x => x == ModifierCategory.Explicit);
        }
        else
        {
            UsePrimaryCategory = false;
            PrimaryCategory = Line.ApiInformation.FirstOrDefault()?.Category ?? ModifierCategory.Undefined;
            SecondaryCategory = ModifierCategory.Undefined;
        }
    }

    public Modifier Line { get; }

    public bool? Checked { get; set; }

    public bool UsePrimaryCategory { get; set; }

    public ModifierCategory PrimaryCategory { get; private init; }

    public ModifierCategory SecondaryCategory { get; private init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public string Text => Line.Text;
}
