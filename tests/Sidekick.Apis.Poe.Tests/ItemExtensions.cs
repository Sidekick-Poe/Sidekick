using System.Linq;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;
using Xunit;

namespace Sidekick.Apis.Poe.Tests
{
    public static class ItemExtensions
    {
        public static void AssertHasModifier(this Item actual, ModifierCategory expectedCategory, string expectedText, params double[] expectedValues)
        {
            var actualModifier = actual?.ModifierLines?.FirstOrDefault(x => expectedText == x.Modifier?.Text);
            Assert.Equal(expectedText, actualModifier?.Modifier?.Text);
            Assert.Equal(expectedCategory, actualModifier?.Modifier?.Category);

            Assert.True(actualModifier.Modifier.Values.Count >= expectedValues.Length);

            for (var i = 0; i < expectedValues.Length; i++)
            {
                Assert.Equal(expectedValues[i], actualModifier.Modifier.Values[i]);
            }
        }

        public static void AssertHasAlternateModifier(this Item actual, ModifierCategory expectedCategory, string expectedText, params double[] expectedValues)
        {
            var alternates = actual?.ModifierLines?.SelectMany(x => x.Alternates);
            var actualModifier = alternates?.FirstOrDefault(x => expectedCategory == x.Category && expectedText == x.Text);
            Assert.Equal(expectedText, actualModifier?.Text);
            Assert.Equal(expectedCategory, actualModifier?.Category);

            Assert.True(actualModifier.Values.Count >= expectedValues.Length);

            for (var i = 0; i < expectedValues.Length; i++)
            {
                Assert.Equal(expectedValues[i], actualModifier.Values[i]);
            }
        }

        public static void AssertHasPseudoModifier(this Item actual, string expectedText, params double[] expectedValues)
        {
            var actualModifier = actual?.PseudoModifiers?.FirstOrDefault(x => expectedText == x.Text);
            Assert.Equal(expectedText, actualModifier.Text);
            Assert.Equal(ModifierCategory.Pseudo, actualModifier.Category);

            Assert.True(actualModifier.Values.Count >= expectedValues.Length);

            for (var i = 0; i < expectedValues.Length; i++)
            {
                Assert.Equal(expectedValues[i], actualModifier.Values[i]);
            }
        }
    }
}
