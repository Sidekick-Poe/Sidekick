namespace Sidekick.Common.Game.Items;

public class Header
{
    public string? Name { get; init; }

    public string? Type { get; init; }

    public string? ItemCategory { get; init; }

    public required Rarity Rarity { get; set; }

    public string? ApiItemId { get; init; }

    public string? ApiName { get; init; }

    public string? ApiType { get; init; }

    public string? ApiTypeDiscriminator { get; init; }

    public required Category Category { get; init; }

    public required GameType Game { get; init; }

    /// <inheritdoc />
    public override string? ToString()
    {
        if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Name))
        {
            return $"{Type} - {Name}";
        }

        return !string.IsNullOrEmpty(Type) ? Type : Name;
    }
}
