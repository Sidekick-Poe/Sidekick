using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class DivinationCardTests(NinjaTestFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;
}
