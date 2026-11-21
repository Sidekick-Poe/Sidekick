using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Xunit;

namespace Sidekick.Game.Parser.Tests;

public static class AssertExtensions
{
    public static void AssertCloseEnough(double expected, double? actual, double tolerance = 0.01)
    {
        var actual2 = actual ?? 0d;
        Assert.True(Math.Abs(actual2 - expected) < tolerance,
                    $"Expected {expected}, but got {actual} which is outside the tolerance of {tolerance}");
    }

    public static void AssertHasNinjaExchange(this Item item, string expectedId)
    {
        Assert.NotNull(item.InvariantDefinition?.NinjaExchangeItems);
        Assert.Contains(item.InvariantDefinition.NinjaExchangeItems, x => x.TradeId == expectedId);
    }

    public static void AssertHasNinjaStash(this Item item, string expectedId)
    {
        Assert.NotNull(item.InvariantDefinition?.NinjaStashItems);
        Assert.Contains(item.InvariantDefinition.NinjaStashItems, x => x.DetailsId == expectedId);
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
