using System.Linq;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests
{
    public static class ItemExtensions
    {
        public static void AssertHasModifier(this Item actual, ModifierCategory expectedCategory, string expectedText, params double[] expectedValues)
        {
            var modifiers = actual?.ModifierLines
                .SelectMany(line => line.Modifiers.Select(modifier => new
                {
                    Line = line,
                    Modifier = modifier,
                }));

            var actualModifier = modifiers.FirstOrDefault(x => expectedText == x.Modifier.Text);
            Assert.Equal(expectedText, actualModifier?.Modifier.Text);
            Assert.Equal(expectedCategory, actualModifier?.Modifier.Category);

            Assert.True(actualModifier.Line.Values.Count == expectedValues.Length);

            for (var i = 0; i < expectedValues.Length; i++)
            {
                Assert.Equal(expectedValues[i], actualModifier.Line.Values[i]);
            }
        }

        public static void AssertHasAlternateModifier(this Item actual, ModifierCategory expectedCategory, string expectedText, params double[] expectedValues)
        {
            var modifiers = actual?.ModifierLines
                .SelectMany(line => line.Modifiers.Select(modifier => new
                {
                    Line = line,
                    Modifier = modifier,
                }));

            var actualModifier = modifiers?.FirstOrDefault(x => expectedCategory == x.Modifier.Category && expectedText == x.Modifier.Text);
            Assert.Equal(expectedText, actualModifier?.Modifier.Text);
            Assert.Equal(expectedCategory, actualModifier?.Modifier.Category);

            Assert.True(actualModifier.Line.Values.Count >= expectedValues.Length);

            for (var i = 0; i < expectedValues.Length; i++)
            {
                Assert.Equal(expectedValues[i], actualModifier.Line.Values[i]);
            }
        }

        public static void AssertHasPseudoModifier(this Item actual, string expectedText, double? expectedValue = null)
        {
            var actualModifier = actual?.PseudoModifiers?.FirstOrDefault(x => expectedText == x.Text);
            Assert.Equal(expectedText, actualModifier.Text);

            if (expectedValue != null)
            {
                Assert.Equal(expectedValue, actualModifier.Value);
            }
        }
    }
}
