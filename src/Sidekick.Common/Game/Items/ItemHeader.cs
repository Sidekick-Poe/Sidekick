namespace Sidekick.Common.Game.Items;

public class ItemHeader
{
    public string? Name { get; set; }

    public string? Type { get; set; }

    public Rarity Rarity { get; set; } = Rarity.Unknown;

    public Category Category { get; init; } = Category.Unknown;

    public GameType Game { get; init; } = GameType.Unknown;

    public string? ApiItemId { get; init; }

    public string? ApiItemCategory { get; set; }

    public string? ApiName { get; init; }

    public string? ApiType { get; init; }

    public string? ApiDiscriminator { get; init; }

    public string? ApiText { get; init; }

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
