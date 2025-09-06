using Sidekick.Apis.Poe2Scout.Api;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe2Scout.Models;

public record Poe2ScoutPrice
{
    public string? Name { get; init; }
    public string? Type { get; init; }
    public decimal Price { get; init; }

    public bool IsChanceable { get; init; }

    public Category? CategoryApiId { get; init; }
    public List<Poe2ScoutPriceLog>? PriceLogs { get; init; }
    public DateTimeOffset LastUpdated { get; init; }
}
