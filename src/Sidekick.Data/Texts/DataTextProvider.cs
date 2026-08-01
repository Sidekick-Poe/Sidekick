using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Data.Extensions;
using Sidekick.Data.Languages;

namespace Sidekick.Data.Texts;

public class DataTextProvider(
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    DataProvider dataProvider) : IInitializableService
{
    public int Priority => 100;

    public DataText Texts { get; private set; } = new();

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Texts = await dataProvider.Read<DataText>(game, DataType.Texts, currentGameLanguage.Language);
    }
}
