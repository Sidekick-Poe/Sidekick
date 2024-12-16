using Xunit;

namespace Sidekick.Apis.Poe.Tests;

public static class AssertHelper
{
    public static void CloseEnough(double expected, double? actual, double tolerance = 0.01)
    {
        var actual2 = actual ?? 0d;
        Assert.True(Math.Abs(actual2 - expected) < tolerance,
                    $"Expected {expected}, but got {actual} which is outside the tolerance of {tolerance}");
    }
}
