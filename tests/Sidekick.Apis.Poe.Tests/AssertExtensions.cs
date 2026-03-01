using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Data.Items;
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

    public static void AssertDoesNotHaveModifier(this Item actual, StatCategory expectedCategory, string expectedText)
    {
        var actualModifier = actual.Stats
            .SelectMany(stat => stat.Definitions.Select(pattern => new
            {
                Stat = stat,
                Pattern = pattern,
            }))
            .FirstOrDefault(x => expectedCategory == x.Stat.Category && expectedText == x.Pattern.Text);

        Assert.Null(actualModifier);
    }

    public static void AssertHasPseudoModifier(this Item actual, string expectedText, double? expectedValue = null)
    {
        var actualModifier = actual.PseudoStats.FirstOrDefault(x => expectedText == x.Text);
        Assert.Equal(expectedText, actualModifier?.Text);

        if (expectedValue != null)
        {
            Assert.Equal(expectedValue, actualModifier?.Value);
        }
    }

    public static IEnumerable<TradeFilter> FlattenFilters(this List<TradeFilter> filters)
    {
        foreach (var filter in filters)
        {
            yield return filter;

            if (filter is not ExpandableFilter expandable) yield break;

            foreach (var subFilter in expandable.Filters)
            {
                yield return subFilter;
            }
        }
    }
}
