using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Languages;
namespace Sidekick.Data.Items;

public class Item
{
    public Item(GameType game, IGameLanguage language, string text)
    {
        Text = new RawText(language, text);
        Game = game;

        if (Text.Blocks[0].Lines.Count >= 2) Type = Text.Blocks[0].Lines[^1].Text;
        if (Text.Blocks[0].Lines.Count >= 4) Name = Text.Blocks[0].Lines[^2].Text;

        Text.Blocks[0].Parsed = true;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public RawText Text { get; }

    public GameType Game { get; }

    public ItemClass ItemClass => Definition.BaseItem?.ItemClass?.Type ?? ItemClass.Unknown;

    public string? Name { get; set; }

    public string? Type { get; set; }

    public ItemDefinition Definition { get; set; } = null!;

    public ItemDefinition Invariant { get; set; } = null!;

    public ItemProperties Properties { get; } = new();

    public bool CanHaveStats => ItemClassConstants.WithStats.Contains(ItemClass) || Properties.AreaLevel > 0;

    public List<Stat> Stats { get; } = [];

    public List<ItemPseudoStat> PseudoStats { get; } = [];

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
