using Sidekick.Apis.Poe.Modifiers;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Modifiers
{
    [Collection(Collections.Poe1Parser)]
    public class InvariantModifierProviderTests
    {
        private readonly IInvariantModifierProvider provider;

        public InvariantModifierProviderTests(ParserFixture fixture)
        {
            provider = fixture.InvariantModifierProvider;
        }

        [Fact]
        public void ClusterJewelSmallPassiveCountModifierIdIsDefined()
        {
            Assert.NotNull(provider.ClusterJewelSmallPassiveCountModifierId);
            Assert.NotEqual(string.Empty, provider.ClusterJewelSmallPassiveCountModifierId);
        }

        [Fact]
        public void ClusterJewelSmallPassiveGrantModifierIdIsDefined()
        {
            Assert.NotNull(provider.ClusterJewelSmallPassiveGrantModifierId);
            Assert.NotEqual(string.Empty, provider.ClusterJewelSmallPassiveGrantModifierId);
        }

        [Fact]
        public void ClusterJewelSmallPassiveGrantOptionsIsDefined()
        {
            Assert.NotNull(provider.ClusterJewelSmallPassiveGrantOptions);
            Assert.NotEmpty(provider.ClusterJewelSmallPassiveGrantOptions);
        }
    }
}
