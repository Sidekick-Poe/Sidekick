using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Xunit;

namespace Sidekick.Apis.Poe.Tests;

public static class AssertExtensions
{
    public static void AssertCloseEnough(double expected, double? actual, double tolerance = 0.01)
    {
        var actual2 = actual ?? 0d;
        Assert.True(Math.Abs(actual2 - expected) < tolerance,
                    $"Expected {expected}, but got {actual} which is outside the tolerance of {tolerance}");
    }

    public static List<TradeFilter> Flatten(this List<TradeFilter> filters)
    {
        var results = new List<TradeFilter>();
        foreach (var filter in filters)
        {
            switch (filter)
            {
                case SeparatorFilter:
                    continue;
                case ExpandableFilter expandableFilter:
                    results.AddRange(expandableFilter.Filters.Flatten());
                    continue;
                default:
                    results.Add(filter);
                    break;
            }
        }

        return results;
    }

}
