using Xunit;

namespace Sidekick.Apis.Poe.Tests
{
    [CollectionDefinition(Collections.Mediator)]
    public class ParserCollection : ICollectionFixture<ParserFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
