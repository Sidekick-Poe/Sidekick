using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Data.Stats;
namespace Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

public class StatFilterValue
{
    public StatFilterValue() {}

    public StatFilterValue(string? option)
    {
        Option = option;
    }

    public StatFilterValue(IntPropertyFilter filter)
    {
        Disabled = !filter.Checked;
        Min = filter.Min;
        Max = filter.Max;
    }

    public StatFilterValue(DoublePropertyFilter filter)
    {
        Disabled = !filter.Checked;
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
            foreach (var tradeStat in filter.Stat.Definitions
                         .Where(x => x.TradeIds != null)
                         .SelectMany(definition => definition.TradeIds!))
            {
                var option = tradeStat.GetStatOption();
                if (option == null) continue;
                return option;
            }

            return null;
        }
    }

    public object? Option { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public bool Disabled { get; set; }
}
