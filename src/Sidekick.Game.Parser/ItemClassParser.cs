using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;

namespace Sidekick.Apis.Poe.Trade.Parser;

public class ItemClassParser(
    GameTextProvider gameTextProvider,
    ItemClassProvider itemClassProvider
)
{
    public void Parse(Item item)
    {
        var line = item.Text.Blocks[0].Lines[0].Text;
        line = line.Replace(gameTextProvider.Texts.ItemPropertyItemClass, string.Empty);
        line = line.Trim(':', ' ');

        // This will fail in the ItemDefinitionParser if the item class is still not set at that point.
        item.ItemClass = itemClassProvider.Definitions.FirstOrDefault(x => x.Name == line)!;
    }
}
