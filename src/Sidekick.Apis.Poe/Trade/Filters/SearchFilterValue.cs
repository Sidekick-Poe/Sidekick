using Sidekick.Apis.Poe.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class SearchFilterValue
    {
        internal SearchFilterValue()
        { }

        public SearchFilterValue(PropertyFilter filter)
        {
            Option = filter.Enabled ? "true" : "false";
            Min = filter.Min;
            Max = filter.Max;
        }

        public SearchFilterValue(ModifierFilter filter)
        {
            Option = filter.Line.Modifier?.OptionValue;
            Min = filter.Min;
            Max = filter.Max;
        }

        public object Option { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
    }
}
