using Sidekick.Game.Parser.Items;
using Xunit;

namespace Sidekick.Apis.PoeNinja.Tests;

public static class AssertExtensions
{
    public static void AssertHasNinjaExchange(this Item item, string expectedId)
    {
        Assert.NotNull(item.InvariantDefinition?.NinjaExchangeItems);
        Assert.Contains(item.InvariantDefinition.NinjaExchangeItems, x => x.Id == expectedId);
    }
}
