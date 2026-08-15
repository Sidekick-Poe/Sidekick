using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.BaseItems;
namespace Sidekick.Game.Providers;

public class BaseItemProvider(
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    DataProvider dataProvider,
    ItemClassProvider itemClassProvider) : IInitializableService
{
    public List<BaseItemDefinition> Definitions { get; private set; } = [];
    public Dictionary<string, BaseItemDefinition> Dictionary { get; private set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Definitions = await dataProvider.Read<List<BaseItemDefinition>>(game, GameDataType.BaseItems, currentGameLanguage.Language);

        foreach (var definition in Definitions)
        {
            if (definition.ItemClassId == null) continue;
            definition.ItemClass = itemClassProvider.Dictionary.GetValueOrDefault(definition.ItemClassId);
        }

        Dictionary = Definitions.ToDictionary(x => x.Id);
    }
}
