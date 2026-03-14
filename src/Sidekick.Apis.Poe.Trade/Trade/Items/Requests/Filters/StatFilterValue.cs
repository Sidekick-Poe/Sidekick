using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
namespace Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

public class StatFilterValue
{
    public StatFilterValue() {}

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
        Option = GetOption();
        Min = filter.Min;
        Max = filter.Max;

        return;

        int? GetOption()
        {
            foreach (var tradeStat in filter.Stat.Definitions.SelectMany(definition => definition.TradeStats))
            {
                if (tradeStat.Option == null) continue;
                return tradeStat.Option.Id;
            }

            return null;
        }
    }

    public object? Option { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }
}
