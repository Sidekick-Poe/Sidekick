namespace Sidekick.Apis.Poe.Parser.Properties.Filters;

public class DoublePropertyFilter : BooleanPropertyFilter
{
    internal DoublePropertyFilter(
        PropertyDefinition definition) : base(definition)
    {
    }

    public required bool NormalizeEnabled { get; init; }

    public required double NormalizeValue { get; set; }

    public string? ValuePrefix { get; set; }

    public string? ValueSuffix { get; set; }

    public required double Value { get; set; }

    public double OriginalValue { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    /// <summary>
    /// Normalize the Min value with NormalizeValue.
    /// </summary>
    public void NormalizeMinValue()
    {
        if (!decimal.TryParse(Value.ToString(), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Min = Math.Round((double)Math.Max((1 - (decimal)NormalizeValue) * value, 0), 2);
        }
        else
        {
            Min = Math.Round((double)Math.Min((1 + (decimal)NormalizeValue) * value, 0), 2);
        }
    }

    /// <summary>
    /// Normalize the Max value, +1 and/or NormalizeValue.
    /// </summary>
    public void NormalizeMaxValue()
    {
        if (!double.TryParse(Value.ToString(), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Max = Math.Round(Math.Max(Math.Max(value + 1, (1 + (double)NormalizeValue) * value), 0), 2);
        }
        else
        {
            Max = Math.Round(Math.Min(Math.Max(value + 1, (1 - (double)NormalizeValue) * value), 0), 2);
        }
    }

    /// <summary>
    /// Sets the filter to be the exact value.
    /// </summary>
    public void SetExactValue()
    {
        if (!double.TryParse(Value.ToString(), out var value))
        {
            return;
        }

        Min = value;
        Max = value;
    }

}
