using Sidekick.Apis.Poe.Trade.TradeStats;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1English.Modifiers;

[Collection(Collections.Poe1EnglishFixture)]
public class InvariantStatsProviderTests(Poe1EnglishFixture fixture)
{
    private readonly ITradeStatsProvider provider = fixture.TradeStatsProvider;

    [Fact]
    public void ClusterJewelSmallPassiveCountModifierIdIsDefined()
    {
        Assert.NotNull(provider.InvariantStats.ClusterJewelSmallPassiveCountStatId);
        Assert.NotEqual(string.Empty, provider.InvariantStats.ClusterJewelSmallPassiveCountStatId);
    }

    [Fact]
    public void ClusterJewelSmallPassiveGrantModifierIdIsDefined()
    {
        Assert.NotNull(provider.InvariantStats.ClusterJewelSmallPassiveGrantStatId);
        Assert.NotEqual(string.Empty, provider.InvariantStats.ClusterJewelSmallPassiveGrantStatId);
    }

    [Fact]
    public void ClusterJewelSmallPassiveGrantOptionsIsDefined()
    {
        Assert.NotNull(provider.InvariantStats.ClusterJewelSmallPassiveGrantOptions);
        Assert.NotEmpty(provider.InvariantStats.ClusterJewelSmallPassiveGrantOptions);
    }
}
