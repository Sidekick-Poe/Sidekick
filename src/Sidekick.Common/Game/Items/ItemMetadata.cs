namespace Sidekick.Common.Game.Items;

public class ItemMetadata
{
    public required string Id { get; init; }

    public string? Name { get; set; }

    public string? Type { get; init; }

    /// <summary>
    /// String that identifies variants of the same item name on the API.
    /// </summary>
    public string? ApiTypeDiscriminator { get; init; }

    /// <summary>
    /// String that identifies the type to be used when communicating with the API.
    /// </summary>
    public string? ApiType { get; init; }

    public required Rarity Rarity { get; set; }

    public required Category Category { get; init; }
}
