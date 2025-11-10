using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Modifiers;

public class ModifierFilter
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

    public double NormalizeValue { get; set; }

    public FilterType FilterType { get; private set; }

    public void ChangeFilterType(FilterType value)
    {
        switch (value)
        {
            case FilterType.Minimum:
                NormalizeMinValue();
                Max = null;
                break;

            case FilterType.Maximum:
                NormalizeMaxValue();
                Min = null;
                break;

            case FilterType.Equals: SetExactValue(); break;

            case FilterType.Range:
                NormalizeMinValue();
                NormalizeMaxValue();
                break;
        }

        FilterType = value;
    }

    /// <summary>
    /// Normalize the Min value with NormalizeValue.
    /// </summary>
    private void NormalizeMinValue()
    {
        if (Line.OptionValue != null || !Line.Values.Any())
        {
            return;
        }

        var value = Line.AverageValue;
        if (Line.Negative) value *= -1;
        if (value > 0)
        {
            Min = (int)Math.Max((1 - NormalizeValue) * value, 0);
        }
        else
        {
            Min = (int)Math.Min((1 + NormalizeValue) * value, 0);
        }
    }

    /// <summary>
    /// Normalize the Max value, +1 and/or NormalizeValue.
    /// </summary>
    private void NormalizeMaxValue()
    {
        if (Line.OptionValue != null || !Line.Values.Any())
        {
            return;
        }

        var value = Line.AverageValue;
        if (Line.Negative) value *= -1;
        if (value > 0)
        {
            Max = (int)Math.Max(Math.Max(value + 1, (1 + NormalizeValue) * value), 0);
        }
        else
        {
            Max = (int)Math.Min(Math.Max(value + 1, (1 - NormalizeValue) * value), 0);
        }
    }

    /// <summary>
    /// Sets the filter to be the exact value.
    /// </summary>
    private void SetExactValue()
    {
        if (Line.OptionValue != null || !Line.Values.Any())
        {
            return;
        }

        var value = Line.AverageValue;
        if (Line.Negative) value *= -1;
        Min = (int)value;
        Max = (int)value;
    }
}
