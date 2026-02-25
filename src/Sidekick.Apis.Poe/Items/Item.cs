using Sidekick.Data.Items.Models;
namespace Sidekick.Apis.Poe.Items;

public class Item
{
    public Item(GameType game, string text)
    {
        Text = new TextItem(text);
        Game = game;

        if (Text.Blocks[0].Lines.Count >= 2) Type = Text.Blocks[0].Lines[^1].Text;
        if (Text.Blocks[0].Lines.Count >= 4) Name = Text.Blocks[0].Lines[^2].Text;

        Text.Blocks[0].Parsed = true;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public TextItem Text { get; }

    public GameType Game { get; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public ItemApiInformation ApiInformation { get; set; } = null!;

    public ItemProperties Properties { get; } = new();

    public List<Stat> Stats { get; } = [];

    public List<PseudoStat> PseudoStats { get; } = [];

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
