using Xunit;

namespace Sidekick.Apis.PoeNinja.Tests;

[CollectionDefinition(Collections.NinjaPoe1TestCollection)]
public class NinjaPoe1TestCollection : ICollectionFixture<NinjaPoe1TestFixture>
{
}