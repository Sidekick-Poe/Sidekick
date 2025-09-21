namespace Sidekick.Apis.Poe.Items;

public class Item
{
    public Guid Id { get; } = Guid.NewGuid();
    public required ItemHeader? Invariant { get; init; }
    public required ItemHeader Header { get; init; }
    public required ItemProperties Properties { get; init; }
    public required List<ModifierLine> ModifierLines { get; init; }
    public required List<PseudoModifier> PseudoModifiers { get; init; }
    public required string Text { get; init; }

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
