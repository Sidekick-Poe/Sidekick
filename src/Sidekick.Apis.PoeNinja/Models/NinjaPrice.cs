using Sidekick.Apis.PoeNinja.Api;

namespace Sidekick.Apis.PoeNinja.Models;

/// <summary>
/// Contains the result of a PriceFromNinjaQuery
/// </summary>
public record NinjaPrice
{
    public string? BaseType { get; init; }

    /// <summary>
    /// The name of the item
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// If the item is corrupted or not
    /// </summary>
    public bool Corrupted { get; init; }

    /// <summary>
    /// If it is a map, indicates the tier of the map
    /// </summary>
    public int MapTier { get; init; }

    /// <summary>
    /// If it is a gem, indicates the level of the gem
    /// </summary>
    public int GemLevel { get; init; }

    /// <summary>
    /// The price in chaos of the item
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// When was the data last updated from PoeNinja
    /// </summary>
    public DateTimeOffset LastUpdated { get; init; }

    public string? DetailsId { get; init; }

    public ItemType ItemType { get; init; }

    public SparkLine? SparkLine { get; init; }

    public bool IsRelic { get; init; }

    public int Links { get; init; }

    public int ItemLevel { get; init; }

    /// <summary>
    /// The number of small passives on a cluster jewel.
    /// </summary>
    public int? SmallPassiveCount { get; init; }

    public bool IsClusterJewel => SmallPassiveCount >= 0;
}
