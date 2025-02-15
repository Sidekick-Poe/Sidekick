namespace Sidekick.Apis.Poe.Parser.Properties.Filters;

public class IntPropertyFilter : BooleanPropertyFilter
{
    internal IntPropertyFilter(
        PropertyDefinition definition) : base(definition)
    {
    }

    public required bool NormalizeEnabled { get; init; }

    public required double NormalizeValue { get; set; }

    public string? ValuePrefix { get; set; }

    public string? ValueSuffix { get; set; }

    public required int Value { get; set; }

    public int OriginalValue { get; set; }

    public int? Min { get; set; }

    public int? Max { get; set; }

    /// <summary>
    /// Normalize the Min value with NormalizeValue.
    /// </summary>
    public void NormalizeMinValue()
    {
        if (!int.TryParse(Value.ToString(), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Min = (int)Math.Max((1 - (decimal)NormalizeValue) * value, 0);
        }
        else
        {
            Min = (int)Math.Min((1 + (decimal)NormalizeValue) * value, 0);
        }
    }

    /// <summary>
    /// Normalize the Max value, +1 and/or NormalizeValue.
    /// </summary>
    public void NormalizeMaxValue()
    {
        if (!int.TryParse(Value.ToString(), out var value) || value == 0)
        {
            return;
        }

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
    public void SetExactValue()
    {
        if (!int.TryParse(Value.ToString(), out var value))
        {
            return;
        }

        Min = value;
        Max = value;
    }

}
