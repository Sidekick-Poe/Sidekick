using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1
{
    [CollectionDefinition(Collections.Poe1Parser)]
    public class Poe1Collection : ICollectionFixture<ParserFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
