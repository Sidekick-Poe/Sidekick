namespace Sidekick.Common.Game.Items;

public class Item(
    ItemMetadata metadata,
    ItemMetadata? invariant,
    Header header,
    Properties properties,
    Influences influences,
    List<Socket> sockets,
    List<ModifierLine> modifierLines,
    List<PseudoModifier> pseudoModifiers,
    string text)
{
    public ItemMetadata Metadata { get; init; } = metadata;

    public ItemMetadata? Invariant { get; set; } = invariant;

    public Header Header { get; init; } = header;

    public Properties Properties { get; init; } = properties;

    public Influences Influences { get; init; } = influences;

    public List<Socket> Sockets { get; init; } = sockets;

    public List<ModifierLine> ModifierLines { get; set; } = modifierLines;

    public List<PseudoModifier> PseudoModifiers { get; init; } = pseudoModifiers;

    public string Text { get; set; } = text;

    public object? AdditionalInformation { get; set; }

    public bool CanHaveModifiers => Metadata.Category switch
    {
        Category.Accessory => true,
        Category.Armour => true,
        Category.Flask => true,
        Category.Gem => true,
        Category.Jewel => true,
        Category.Map => Metadata.Rarity != Rarity.Currency,
        Category.Weapon => true,
        Category.HeistEquipment => true,
        Category.Contract => true,
        Category.Logbook => true,
        Category.Affliction => true,
        _ => ModifierLines.Count != 0,
    };

    /// <inheritdoc />
    public override string? ToString()
    {
        if (!string.IsNullOrEmpty(Metadata.Name) && !string.IsNullOrEmpty(Metadata.Type) && Metadata.Name != Metadata.Type)
        {
            return $"{Metadata.Name} - {Metadata.Type}";
        }

        if (!string.IsNullOrEmpty(Metadata.Type))
        {
            return Metadata.Type;
        }

        return Metadata.Name;
    }

    public int GetMaximumNumberOfLinks()
    {
        return Sockets.Count != 0 ?
            Sockets
                .GroupBy(x => x.Group)
                .Max(x => x.Count()) :
            0;
    }
}
