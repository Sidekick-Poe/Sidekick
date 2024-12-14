using System.Text.Json.Serialization;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class PropertyFilter : ITradeFilter
    {
        public PropertyFilter(
            bool? enabled,
            PropertyFilterType type,
            string text,
            object value,
            decimal? min = null,
            decimal? max = null,
            double delta = 1)
        {
            Enabled = enabled;
            Type = type;
            Text = text;
            Value = value;
            Delta = delta;
            MinimumValueForNormalization = min;
            Max = max;
            NormalizeMinValue();

            switch (value)
            {
                case bool:
                    ValueType = FilterValueType.Boolean;
                    break;

                case int:
                    ValueType = FilterValueType.Int;
                    break;

                case double:
                    ValueType = FilterValueType.Double;
                    break;

                case DamageRange:
                case IEnumerable<DamageRange>:
                    ValueType = FilterValueType.DamageRange;
                    break;
            }

            if (min.HasValue)
            {
                Min = min;
            }
            if (max.HasValue)
            {
                Max = max;
            }
        }

        public PropertyFilterType Type { get; init; }

        public bool? Enabled { get; set; }

        public decimal? Min { get; set; }

        public decimal? Max { get; set; }

        public string Text { get; init; }

        [JsonIgnore]
        public object Value { get; }

        public double Delta { get; }

        private decimal? MinimumValueForNormalization { get; }

        public FilterValueType ValueType { get; set; }

        /// <summary>
        /// Normalizes the Min value between a -1 delta or 90%.
        /// </summary>
        public void NormalizeMinValue()
        {
            if (Value is IEnumerable<DamageRange> ranges)
            {
                if (ranges.Any(x => x.Min > 0 || x.Max > 0))
                {
                    Min = Convert.ToDecimal(ranges.Sum(x => x.Min));
                }
                return;
            }
            else if (Value is DamageRange damageRange)
            {
                if (damageRange.Min > 0 || damageRange.Max > 0)
                {
                    Min = Convert.ToDecimal(damageRange.Min);
                }
                return;
            }

            if (!double.TryParse(Value.ToString(), out var value) || value == 0)
            {
                return;
            }

            if (value > 0)
            {
                Min = Convert.ToDecimal(Math.Max(Math.Min(value - Delta, value * 0.9), 0));
            }
            else
            {
                Min = Convert.ToDecimal(Math.Min(Math.Min(value - Delta, value * 1.1), 0));
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
            if (Value is IEnumerable<DamageRange> ranges)
            {
                if (ranges.Any(x => x.Min > 0 || x.Max > 0))
                {
                    Max = Convert.ToDecimal(ranges.Sum(x => x.Max));
                }
                return;
            }
            else if (Value is DamageRange damageRange)
            {
                if (damageRange.Min > 0 || damageRange.Max > 0)
                {
                    Max = Convert.ToDecimal(damageRange.Max);
                }
                return;
            }

            if (!double.TryParse(Value.ToString(), out var value) || value == 0)
            {
                return;
            }

            if (value > 0)
            {
                Max = Convert.ToDecimal(Math.Max(Math.Max(value + Delta, value * 1.1), 0));
            }
            else
            {
                Max = Convert.ToDecimal(Math.Min(Math.Max(value + Delta, value * 0.9), 0));
            }
        }

        /// <summary>
        /// Sets the filter to be the exact value.
        /// </summary>
        public void SetExactValue()
        {
            if (Value is IEnumerable<DamageRange> ranges)
            {
                if (ranges.Any(x => x.Min > 0 || x.Max > 0))
                {
                    Min = Convert.ToDecimal(ranges.Sum(x => x.Min));
                    Max = Convert.ToDecimal(ranges.Sum(x => x.Max));
                }
                return;
            }
            else if (Value is DamageRange damageRange)
            {
                if (damageRange.Min > 0 || damageRange.Max > 0)
                {
                    Min = Convert.ToDecimal(damageRange.Min);
                    Max = Convert.ToDecimal(damageRange.Max);
                }
                return;
            }

            if (!decimal.TryParse(Value.ToString(), out var value))
            {
                return;
            }

            Min = value;
            Max = value;
        }
    }
}
