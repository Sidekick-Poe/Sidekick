using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Texts;
namespace Sidekick.Apis.Poe.Trade.Parser;

public class ItemClassParser(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    GameTextProvider gameTextProvider
) : IInitializableService
{
    public List<ItemClassDefinition> Definitions { get; set; } = [];

    public int Priority => 100;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Definitions = await dataProvider.Read<List<ItemClassDefinition>>(game, GameDataType.ItemClasses, currentGameLanguage.Language);
    }

    public void Parse(Item item)
    {
        var line = item.Text.Blocks[0].Lines[0].Text;
        line = line.Replace(gameTextProvider.Texts.ItemPropertyItemClass, string.Empty);
        line = line.Trim(':', ' ');

        // This will fail in the ItemDefinitionParser if the item class is still not set at that point.
        item.ItemClass = Definitions.FirstOrDefault(x => x.Name == line)!;
    }
}
