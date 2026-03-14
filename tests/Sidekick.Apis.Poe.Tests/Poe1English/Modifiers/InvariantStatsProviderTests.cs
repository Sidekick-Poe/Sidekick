using Sidekick.Apis.Poe.Trade.ApiStats;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1English.Modifiers;

[Collection(Collections.Poe1EnglishFixture)]
public class InvariantStatsProviderTests(Poe1EnglishFixture fixture)
{
    private readonly IApiStatsProvider provider = fixture.ApiStatsProvider;

    [Fact]
    public void ClusterJewelSmallPassiveCountModifierIdIsDefined()
    {
        Assert.NotNull(provider.InvariantDetails.ClusterJewelSmallPassiveCountStatId);
        Assert.NotEqual(string.Empty, provider.InvariantDetails.ClusterJewelSmallPassiveCountStatId);
    }

    [Fact]
    public void ClusterJewelSmallPassiveGrantModifierIdIsDefined()
    {
        Assert.NotNull(provider.InvariantDetails.ClusterJewelSmallPassiveGrantStatId);
        Assert.NotEqual(string.Empty, provider.InvariantDetails.ClusterJewelSmallPassiveGrantStatId);
    }

    [Fact]
    public void ClusterJewelSmallPassiveGrantOptionsIsDefined()
    {
        Assert.NotNull(provider.InvariantDetails.ClusterJewelSmallPassiveGrantOptions);
        Assert.NotEmpty(provider.InvariantDetails.ClusterJewelSmallPassiveGrantOptions);
    }
}
