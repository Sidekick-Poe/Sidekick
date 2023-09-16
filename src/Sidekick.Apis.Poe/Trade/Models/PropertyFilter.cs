namespace Sidekick.Apis.Poe.Trade.Models
{
    public class PropertyFilter : ITradeFilter
    {
        public PropertyFilter(
            bool? enabled,
            PropertyFilterType type,
            string text,
            object value,
            double? min = null,
            double delta = 1)
        {
            Enabled = enabled;
            Type = type;
            Text = text;
            Value = value;
            Delta = delta;
            MinimumValueForNormalization = min;
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
            }

            if (min.HasValue)
            {
                Min = min;
            }
        }

        public PropertyFilterType Type { get; init; }

        public bool? Enabled { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }

        public string Text { get; init; }

        public object Value { get; }

        public double Delta { get; }

        private double? MinimumValueForNormalization { get; }

        public FilterValueType ValueType { get; set; }

        /// <summary>
        /// Normalizes the Min value between a -1 delta or 90%.
        /// </summary>
        public void NormalizeMinValue()
        {
            if (!double.TryParse(Value.ToString(), out var value) || value == 0)
            {
                return;
            }

            if (value > 0)
            {
                Min = (int)Math.Max(Math.Min(value - Delta, value * 0.9), 0);
            }
            else
            {
                Min = (int)Math.Min(Math.Min(value - Delta, value * 1.1), 0);
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
            if (!double.TryParse(Value.ToString(), out var value) || value == 0)
            {
                return;
            }

            if (value > 0)
            {
                Max = (int)Math.Max(Math.Max(value + Delta, value * 1.1), 0);
            }
            else
            {
                Max = (int)Math.Min(Math.Max(value + Delta, value * 0.9), 0);
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
}
