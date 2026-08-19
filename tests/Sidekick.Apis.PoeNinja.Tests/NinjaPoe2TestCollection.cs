using Xunit;

namespace Sidekick.Apis.PoeNinja.Tests;

[CollectionDefinition(Collections.NinjaPoe2TestCollection)]
public class NinjaPoe2TestCollection : ICollectionFixture<NinjaPoe2TestFixture>
{
}
