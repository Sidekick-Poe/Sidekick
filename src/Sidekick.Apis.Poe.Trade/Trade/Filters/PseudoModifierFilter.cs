using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Trade.Filters;

public class PseudoModifierFilter : ITradeFilter
{
    public required PseudoModifier PseudoModifier { get; set; }

    public bool? @Checked { get; set; } = false;

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
        if (PseudoModifier.Value > 0)
        {
            Min = (int)Math.Max((1 - NormalizeValue) * PseudoModifier.Value, 0);
        }
        else
        {
            Min = (int)Math.Min((1 + NormalizeValue) * PseudoModifier.Value, 0);
        }
    }

    /// <summary>
    /// Normalize the Max value, +1 and/or NormalizeValue.
    /// </summary>
    private void NormalizeMaxValue()
    {
        if (PseudoModifier.Value > 0)
        {
            Max = (int)Math.Max(Math.Max(PseudoModifier.Value + 1, (1 + NormalizeValue) * PseudoModifier.Value), 0);
        }
        else
        {
            Max = (int)Math.Min(Math.Max(PseudoModifier.Value + 1, (1 - NormalizeValue) * PseudoModifier.Value), 0);
        }
    }

    /// <summary>
    /// Sets the filter to be the exact value.
    /// </summary>
    private void SetExactValue()
    {
        Min = (int)PseudoModifier.Value;
        Max = (int)PseudoModifier.Value;
    }
}
