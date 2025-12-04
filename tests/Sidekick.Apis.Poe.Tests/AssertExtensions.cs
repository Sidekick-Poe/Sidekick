using Sidekick.Apis.Poe.Items;
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

    public static void AssertHasModifier(this Item actual, ModifierCategory expectedCategory, string expectedText, params double[] expectedValues)
    {
        var modifiers = actual.Modifiers
            .SelectMany(line => line.ApiInformation.Select(modifier => new
            {
                Line = line,
                Modifier = modifier,
            }));

        var actualModifier = modifiers.FirstOrDefault(x => expectedCategory == x.Modifier.Category && expectedText == x.Modifier.ApiText);
        for (var i = 0; i < expectedValues.Length; i++)
        {
            Assert.Equal(expectedValues[i], actualModifier?.Line.Values[i]);
        }

        Assert.True(actualModifier?.Line.Values.Count == expectedValues.Length);
        Assert.NotNull(actualModifier);
    }

    public static void AssertDoesNotHaveModifier(this Item actual, ModifierCategory expectedCategory, string expectedText)
    {
        var modifiers = actual.Modifiers
            .SelectMany(line => line.ApiInformation.Select(modifier => new
            {
                Line = line,
                Modifier = modifier,
            }));

        var actualModifier = modifiers.FirstOrDefault(x => expectedCategory == x.Modifier.Category && expectedText == x.Modifier.ApiText);
        Assert.Null(actualModifier);
    }

    public static void AssertHasPseudoModifier(this Item actual, string expectedText, double? expectedValue = null)
    {
        var actualModifier = actual.PseudoModifiers.FirstOrDefault(x => expectedText == x.Text);
        Assert.Equal(expectedText, actualModifier?.Text);

        if (expectedValue != null)
        {
            Assert.Equal(expectedValue, actualModifier?.Value);
        }
    }
}
