using Sidekick.Data.ItemClasses;
using Sidekick.Data.ItemDefinitions;

namespace Sidekick.Data.Items;

public class Item
{
    public Item(GameType game, OriginalText text)
    {
        Text = text;
        Game = game;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public OriginalText Text { get; }

    public GameType Game { get; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public ItemClassDefinition ItemClass { get; set; } = null!;

    public ItemDefinition Definition { get; set; } = null!;

    public ItemDefinition? Invariant { get; set; }

    public ItemProperties Properties { get; } = new();

    public List<Stat> Stats { get; set; } = [];

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
