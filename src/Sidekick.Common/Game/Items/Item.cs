namespace Sidekick.Common.Game.Items;

public class Item
{
    public required ItemHeader? Invariant { get; init; }
    public required ItemHeader Header { get; init; }
    public required ItemProperties Properties { get; init; }
    public required List<ModifierLine> ModifierLines { get; init; }
    public required List<PseudoModifier> PseudoModifiers { get; init; }
    public required string Text { get; init; }
    public object? AdditionalInformation { get; init; }
    public bool CanHaveModifiers => Header.Category switch
    {
        Category.Accessory => true,
        Category.Armour => true,
        Category.Flask => true,
        Category.Gem => true,
        Category.Jewel => true,
        Category.Map => Header.Rarity != Rarity.Currency,
        Category.Weapon => true,
        Category.HeistEquipment => true,
        Category.Contract => true,
        Category.Logbook => true,
        Category.Affliction => true,

        // In PoE2, the uncut gems are currency. But we still want to display the gem levels.
        _ => ModifierLines.Count != 0 || Properties.GemLevel > 0 || Properties.Unidentified,
    };

    /// <inheritdoc />
    public override string? ToString()
    {
        if (!string.IsNullOrEmpty(Header.Name) && !string.IsNullOrEmpty(Header.Type) && Header.Name != Header.Type)
        {
            return $"{Header.Name} - {Header.Type}";
        }

        if (!string.IsNullOrEmpty(Header.Type))
        {
            return Header.Type;
        }

        return Header.Name;
    }
}
