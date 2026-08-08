using Sidekick.Common.Exceptions;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Extensions;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Texts;
namespace Sidekick.Apis.Poe.Trade.Parser.ItemClasses;

public class ItemClassParser(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    GameTextProvider gameTextProvider
) : IInitializableService
{
    public List<ItemClassDefinition> ItemClasses { get; set; } = [];

    public int Priority => 100;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        ItemClasses = await dataProvider.Read<List<ItemClassDefinition>>(game, DataType.ItemClasses, currentGameLanguage.Language);
    }

    public void Parse(Item item)
    {
        var line = item.Text.Blocks[0].Lines[0].Text;
        line = line.Replace(gameTextProvider.Texts.ItemPropertyItemClass, string.Empty);
        line = line.Trim(':', ' ');

        var itemClass = ItemClasses.FirstOrDefault(x => x.Name == line);
        if (itemClass == null) throw new UnparsableException(item.Text.Text);

        item.ItemClass = itemClass;
    }
}
