using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Data.Extensions;
using Sidekick.Data.Languages;
namespace Sidekick.Data.ItemClasses;

public class ItemClassProvider(
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    DataProvider dataProvider) : IInitializableService
{
    public int Priority => 100;

    public List<ItemClassDefinition> ItemClasses { get; set; } = [];
    public Dictionary<string, ItemClassDefinition> ById { get; set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        ItemClasses = await dataProvider.Read<List<ItemClassDefinition>>(game, DataType.ItemClasses, currentGameLanguage.Language);
        ById = ItemClasses.ToDictionary(x => x.Id ?? string.Empty, x => x);
    }
}
