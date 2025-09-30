using System.Globalization;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class DoublePropertyFilter : PropertyFilter
{
    internal DoublePropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public required bool NormalizeEnabled { get; init; }

    public required double NormalizeValue { get; set; }

    public string? ValuePrefix { get; set; }

    public string? ValueSuffix { get; init; }

    public required double Value { get; init; }

    public double OriginalValue { get; init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

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
        if (!NormalizeEnabled)
        {
            Min = Value;
            return;
        }

        if (!decimal.TryParse(Value.ToString(CultureInfo.InvariantCulture), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Min = Math.Round((double)Math.Max((1 - (decimal)NormalizeValue) * value, 0), 2);
            return;
        }

        Min = Math.Round((double)Math.Min((1 + (decimal)NormalizeValue) * value, 0), 2);
    }

    /// <summary>
    /// Normalize the Max value, +1 and/or NormalizeValue.
    /// </summary>
    private void NormalizeMaxValue()
    {
        if (!NormalizeEnabled)
        {
            Max = Value;
            return;
        }

        if (!double.TryParse(Value.ToString(CultureInfo.InvariantCulture), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Max = Math.Round(Math.Max(Math.Max(value + 1, (1 + NormalizeValue) * value), 0), 2);
            return;
        }

        Max = Math.Round(Math.Min(Math.Max(value + 1, (1 - NormalizeValue) * value), 0), 2);
    }

    /// <summary>
    /// Sets the filter to be the exact value.
    /// </summary>
    private void SetExactValue()
    {
        if (!double.TryParse(Value.ToString(CultureInfo.InvariantCulture), out var value))
        {
            return;
        }

        Min = value;
        Max = value;
    }
}
