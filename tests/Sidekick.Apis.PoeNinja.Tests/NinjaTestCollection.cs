using Xunit;

namespace Sidekick.Apis.PoeNinja.Tests;

[CollectionDefinition(Collections.NinjaTestCollection)]
public class NinjaTestCollection : ICollectionFixture<NinjaTestFixture>
{
}

public static class Collections
{
    public const string NinjaTestCollection = "NinjaTestCollection";
}
