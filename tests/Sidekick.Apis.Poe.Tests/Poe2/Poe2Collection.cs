using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2
{
    [CollectionDefinition(Collections.Poe2Parser)]
    public class Poe2Collection : ICollectionFixture<ParserFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
