using Sidekick.Apis.Poe.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters;

internal class StatFilterValue
{
    internal StatFilterValue()
    {
    }

    public StatFilterValue(string? option)
    {
        Option = option;
    }

    public StatFilterValue(PropertyFilter filter)
    {
        Option = filter.Checked == true ? "true" : "false";
        Min = filter.Min;
        Max = filter.Max;
    }

    public StatFilterValue(ModifierFilter filter)
    {
        Option = filter.Line.OptionValue;
        Min = filter.Min;
        Max = filter.Max;
    }

    public object? Option { get; set; }

    public decimal? Min { get; set; }

    public decimal? Max { get; set; }

    public double? Weight { get; set; }
}
