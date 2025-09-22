namespace Sidekick.Apis.Poe.Items;

public class Item
{
    public Item(GameType game, string itemText, string? advancedItemText = null)
    {
        Text = new TextItem(itemText);
        AdvancedText = advancedItemText != null ? new(advancedItemText) : null;
        Game = game;

        if (Text.Blocks[0].Lines.Count >= 3)
        {
            Type = Text.Blocks[0].Lines[^1].Text;
            Text.Blocks[0].Lines[^1].Parsed = true;
        }

        if (Text.Blocks[0].Lines.Count >= 4)
        {
            Name = Text.Blocks[0].Lines[^2].Text;
            Text.Blocks[0].Lines[^2].Parsed = true;
        }
    }

    public Guid Id { get; } = Guid.NewGuid();

    public TextItem Text { get; }

    public TextItem? AdvancedText { get; }

    public GameType Game { get; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public ItemHeader? Invariant { get; set; }

    public ItemHeader Header { get; } = new();

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
