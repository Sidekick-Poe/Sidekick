using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
namespace Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

public class StatFilterValue
{
    public StatFilterValue()
    {
    }

    public StatFilterValue(string? option)
    {
        Option = option;
    }

    public StatFilterValue(IntPropertyFilter filter)
    {
        Option = filter.Checked ? "true" : "false";
        Min = filter.Min;
        Max = filter.Max;
    }

    public StatFilterValue(DoublePropertyFilter filter)
    {
        Option = filter.Checked ? "true" : "false";
        Min = filter.Min;
        Max = filter.Max;
    }

    public StatFilterValue(StatFilter filter)
    {
        Option = filter.Stat.OptionValue;
        Min = filter.Min;
        Max = filter.Max;
    }

    public object? Option { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public double? Weight { get; set; }
}
