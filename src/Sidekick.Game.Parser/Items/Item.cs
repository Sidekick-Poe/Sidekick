using Sidekick.Game.ItemClasses;
using Sidekick.Game.ItemDefinitions;

namespace Sidekick.Game.Items;

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

    public ExchangeItem? Exchange { get; set; }

    public NinjaExchangeItem? NinjaExchange { get; set; }

    public TradeItemDefinition? TradeItem { get; set; }

    public TradeItemDefinition? InvariantTradeItem { get; set; }

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
