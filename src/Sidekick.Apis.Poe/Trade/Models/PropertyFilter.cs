namespace Sidekick.Apis.Poe.Trade.Models
{
    public class PropertyFilter
    {
        public PropertyFilterType Type { get; set; }

        public bool? Enabled { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }

        public string Text { get; set; }

        public object Value { get; set; }

        public FilterValueType ValueType { get; set; }
    }
}
