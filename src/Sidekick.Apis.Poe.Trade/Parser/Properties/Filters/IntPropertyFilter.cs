using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class IntPropertyFilter : PropertyFilter
{
    internal IntPropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public required bool NormalizeEnabled { get; init; }

    public required double NormalizeValue { get; set; }

    public string? ValuePrefix { get; init; }

    public string? ValueSuffix { get; init; }

    public required int Value { get; init; }

    public int OriginalValue { get; init; }

    public int? Min { get; set; }

    public int? Max { get; set; }

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

        if (!int.TryParse(Value.ToString(), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Min = (int)Math.Max((1 - (decimal)NormalizeValue) * value, 0);
            return;
        }

        Min = (int)Math.Min((1 + (decimal)NormalizeValue) * value, 0);
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

        if (!int.TryParse(Value.ToString(), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Max = (int)Math.Max(Math.Max(value + 1, (1 + NormalizeValue) * value), 0);
            return;
        }

        Max = (int)Math.Min(Math.Max(value + 1, (1 - NormalizeValue) * value), 0);
    }

    /// <summary>
    /// Sets the filter to be the exact value.
    /// </summary>
    private void SetExactValue()
    {
        if (!int.TryParse(Value.ToString(), out var value))
        {
            return;
        }

        Min = value;
        Max = value;
    }
}
