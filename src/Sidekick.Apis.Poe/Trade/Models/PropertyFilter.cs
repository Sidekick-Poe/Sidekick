namespace Sidekick.Apis.Poe.Trade.Models
{
    public class PropertyFilter
    {
        public PropertyFilter(
            bool? enabled,
            PropertyFilterType type,
            string text,
            FilterValueType valueType,
            object value,
            double? min,
            double? max)
        {
            Enabled = enabled;
            Type = type;
            Text = text;
            ValueType = valueType;
            Value = value;
            Min = min;
            Max = max;
        }

        public PropertyFilterType Type { get; set; }

        public bool? Enabled { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }

        public string Text { get; }

        public object Value { get; set; }

        public FilterValueType ValueType { get; set; }
    }
}
