namespace Sidekick.Common.Game.Items;

public class Item(
    Header? invariant,
    Header header,
    Properties properties,
    Influences influences,
    List<Socket> sockets,
    List<ModifierLine> modifierLines,
    List<PseudoModifier> pseudoModifiers,
    string text)
{
    public Header? Invariant { get; set; } = invariant;

    public Header Header { get; init; } = header;

    public Properties Properties { get; init; } = properties;

    public Influences Influences { get; init; } = influences;

    public List<Socket> Sockets { get; init; } = sockets;

    public List<ModifierLine> ModifierLines { get; set; } = modifierLines;

    public List<PseudoModifier> PseudoModifiers { get; init; } = pseudoModifiers;

    public string Text { get; set; } = text;

    public object? AdditionalInformation { get; set; }

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
        _ => ModifierLines.Count != 0 || Properties.GemLevel > 0,
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

    public int GetMaximumNumberOfLinks()
    {
        return Sockets.Count != 0 ?
            Sockets
                .GroupBy(x => x.Group)
                .Max(x => x.Count()) :
            0;
    }
}
