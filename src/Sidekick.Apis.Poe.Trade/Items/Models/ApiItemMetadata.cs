using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Items.Models;

public class ApiItemMetadata2
{
    public string? Name { get; set; }

    public string? Type { get; set; }

    /// <summary>
    /// String that identifies variants of the same item name on the API.
    /// </summary>
    public string? ApiTypeDiscriminator { get; init; }

    /// <summary>
    /// String that identifies the type to be used when communicating with the API.
    /// </summary>
    public string? ApiType { get; init; }

    public required Category Category { get; init; }

    public required Rarity Rarity { get; set; }
}
