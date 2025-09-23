namespace Sidekick.Apis.Poe.Items;

public class Item
{
    public Item(GameType game, string? text, string advancedText)
    {
        var normalText = text != null ? new TextItem(text) : null;
        Text = new TextItem(advancedText);
        Game = game;

        if (Text.Blocks[0].Lines.Count >= 3) InvariantType = Text.Blocks[0].Lines[^1].Text;
        if (Text.Blocks[0].Lines.Count >= 4) InvariantName = Text.Blocks[0].Lines[^2].Text;

        Type = normalText?.Blocks[0].Lines.Count >= 3 ? normalText.Blocks[0].Lines[^1].Text : InvariantType;
        Name = normalText?.Blocks[0].Lines.Count >= 4 ? normalText.Blocks[0].Lines[^2].Text : InvariantName;

        Text.Blocks[0].Parsed = true;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public TextItem Text { get; }

    public GameType Game { get; }

    public string? InvariantName { get; set; }

    public string? InvariantType { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public ItemApiInformation Invariant { get; set; } = null!;

    public ItemApiInformation Header { get; set; } = null!;

    public ItemProperties Properties { get; } = new();

    public List<ModifierLine> ModifierLines { get; } = [];

    public List<PseudoModifier> PseudoModifiers { get; } = [];

    /// <inheritdoc />
    public override string? ToString()
    {
        if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Type) && Name != Type)
        {
            return $"{Name} - {Type}";
        }

        return !string.IsNullOrEmpty(Type) ? Type : Name;
    }
}
