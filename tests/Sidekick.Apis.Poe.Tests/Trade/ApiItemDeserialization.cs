using System.Text.Json;
using Sidekick.Apis.Poe.Trade.Models;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Trade;

public class ApiItemDeserialization
{
    // Same naming policy as ItemTradeService.
    private static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Theory]
    [InlineData("""{"corrupted":true,"doubleCorrupted":true}""", true, true)]
    [InlineData("""{"corrupted":true}""", true, false)]
    [InlineData("""{}""", false, false)]
    public void CorruptionFlags(string json, bool corrupted, bool twiceCorrupted)
    {
        var item = JsonSerializer.Deserialize<ApiItem>(json, Options);

        Assert.NotNull(item);
        Assert.Equal(corrupted, item.Corrupted);
        Assert.Equal(twiceCorrupted, item.TwiceCorrupted);
    }
}
