using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1English.Modifiers;

[Collection(Collections.Poe1EnglishFixture)]
public class StatParserTests(Poe1EnglishFixture fixture)
{
    private readonly IStatParser statParser = fixture.StatParser;

    [Fact]
    public void ClusterJewelSmallPassiveCountModifierIdIsDefined()
    {
        Assert.NotNull(statParser.InvariantDetails.ClusterJewelSmallPassiveCountStatId);
        Assert.NotEqual(string.Empty, statParser.InvariantDetails.ClusterJewelSmallPassiveCountStatId);
    }

    [Fact]
    public void ClusterJewelSmallPassiveGrantModifierIdIsDefined()
    {
        Assert.NotNull(statParser.InvariantDetails.ClusterJewelSmallPassiveGrantStatId);
        Assert.NotEqual(string.Empty, statParser.InvariantDetails.ClusterJewelSmallPassiveGrantStatId);
    }

    [Fact]
    public void ClusterJewelSmallPassiveGrantOptionsIsDefined()
    {
        Assert.NotNull(statParser.InvariantDetails.ClusterJewelSmallPassiveGrantOptions);
        Assert.NotEmpty(statParser.InvariantDetails.ClusterJewelSmallPassiveGrantOptions);
    }
}
