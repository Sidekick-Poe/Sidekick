using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Data.Trade;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Modifiers;

[Collection(Collections.Poe1EnglishFixture)]
public class InvariantStatsProviderTests(Poe1EnglishFixture fixture)
{
    private readonly TradeInvariantStatProvider provider = fixture.InvariantStatProvider;

    [Fact]
    public void ClusterJewelSmallPassiveCountModifierIdIsDefined()
    {
        Assert.NotNull(provider.ClusterJewelSmallPassiveCountStatId);
        Assert.NotEqual(string.Empty, provider.ClusterJewelSmallPassiveCountStatId);
    }

    [Fact]
    public void ClusterJewelSmallPassiveGrantModifierIdIsDefined()
    {
        Assert.NotNull(provider.ClusterJewelSmallPassiveGrantStatId);
        Assert.NotEqual(string.Empty, provider.ClusterJewelSmallPassiveGrantStatId);
    }

    [Fact]
    public void ClusterJewelSmallPassiveGrantOptionsIsDefined()
    {
        Assert.NotNull(provider.ClusterJewelSmallPassiveGrantOptions);
        Assert.NotEmpty(provider.ClusterJewelSmallPassiveGrantOptions);
    }
}
