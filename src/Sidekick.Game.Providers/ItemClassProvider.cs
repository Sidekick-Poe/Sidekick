using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Providers.Languages;

namespace Sidekick.Game.Providers;

public class ItemClassProvider(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService
) : IInitializableService
{
    public List<ItemClassDefinition> Definitions { get; private set; } = [];
    public Dictionary<string, ItemClassDefinition> Dictionary { get; private set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Definitions = await dataProvider.Read<List<ItemClassDefinition>>(game, GameDataType.ItemClasses, currentGameLanguage.Language);
        Dictionary = Definitions.ToDictionary(x => x.Id);
    }
}
