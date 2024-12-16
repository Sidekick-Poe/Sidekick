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
        decimal? max = null,
        decimal delta = 1)
    {
        Checked = @checked;
        Type = type;
        Text = text;
        Value = value;
        Delta = delta;
        MinimumValueForNormalization = min;
        Max = max;
        NormalizeMinValue();

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

    public decimal Delta { get; }

    private decimal? MinimumValueForNormalization { get; }

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
    /// Normalizes the Min value between a -1 delta or 90%.
    /// </summary>
    public void NormalizeMinValue()
    {
        if (!decimal.TryParse(Value.ToString(), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Min = Math.Max(Math.Min(value - Delta, value * (decimal)0.9), 0);
        }
        else
        {
            Min = Math.Min(Math.Min(value - Delta, value * (decimal)1.1), 0);
        }

        if (MinimumValueForNormalization.HasValue && Min < MinimumValueForNormalization)
        {
            Min = MinimumValueForNormalization;
        }
    }

    /// <summary>
    /// Normalizes the Max value between a +1 delta or 90%.
    /// </summary>
    public void NormalizeMaxValue()
    {
        if (!decimal.TryParse(Value.ToString(), out var value) || value == 0)
        {
            return;
        }

        if (value > 0)
        {
            Max = Math.Max(Math.Max(value + Delta, value * (decimal)1.1), 0);
        }
        else
        {
            Max = Math.Min(Math.Max(value + Delta, value * (decimal)0.9), 0);
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
