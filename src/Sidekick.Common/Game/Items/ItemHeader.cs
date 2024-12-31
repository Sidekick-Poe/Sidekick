namespace Sidekick.Common.Game.Items;

public class ItemHeader
{
    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? ItemCategory { get; set; }

    public Rarity Rarity { get; set; } = Rarity.Unknown;

    public string? ApiItemId { get; init; }

    public string? ApiName { get; init; }

    public string? ApiType { get; init; }

    public string? ApiDiscriminator { get; init; }

    public string? ApiText { get; init; }

    public Category Category { get; init; } = Category.Unknown;

    public GameType Game { get; init; } = GameType.Unknown;

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
