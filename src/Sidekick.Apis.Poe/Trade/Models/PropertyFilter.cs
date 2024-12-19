using Sidekick.Common.Exceptions;

namespace Sidekick.Apis.Poe.Trade.Models;

public class PropertyFilter : ITradeFilter
{
    public PropertyFilter(
        bool? @checked,
        PropertyFilterType type,
        string text,
        object value,
        decimal? min = null,
        decimal? max = null)
    {
        Checked = @checked;
        Type = type;
        Text = text;
        Value = value;
        Max = max;

        if (min.HasValue)
        {
            Min = min;
        }

        if (max.HasValue)
        {
            Max = max;
        }
    }

    public PropertyFilterType Type { get; }

    public bool? Checked { get; set; }

    public decimal? Min { get; set; }

    public decimal? Max { get; set; }

    public string Text { get; }

    public object Value { get; }
    public double NormalizeValue { get; set; }

    public FilterValueType ValueType
    {
        get
        {
            switch (Value)
            {
                case bool: return FilterValueType.Boolean;
                case int: return FilterValueType.Int;
                case double: return FilterValueType.Double;
                default : throw new SidekickException("[PropertyFilter] Unknown value type: " + Value.GetType().Name);
            }
        }
    }

    public bool ShowCheckbox
    {
        get
        {
            return Type != PropertyFilterType.Weapon_ChaosDps;
        }
    }

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
            Min = Math.Max((1 - (decimal)NormalizeValue) * value, 0);
        }
        else
        {
            Min = Math.Min((1 + (decimal)NormalizeValue) * value, 0);
        }
    }

    /// <summary>
    /// Normalize the Max value, +1 and/or NormalizeValue.
    /// </summary>
    public void NormalizeMaxValue()
    {
        if (!decimal.TryParse(Value.ToString(), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Max = Math.Max(Math.Max(value + 1, (1 + (decimal)NormalizeValue) * value), 0);
        }
        else
        {
            Max = Math.Min(Math.Max(value + 1, (1 - (decimal)NormalizeValue) * value), 0);
        }
    }

    /// <summary>
    /// Sets the filter to be the exact value.
    /// </summary>
    public void SetExactValue()
    {
        if (!decimal.TryParse(Value.ToString(), out var value))
        {
            return;
        }

        Min = value;
        Max = value;
    }
}
