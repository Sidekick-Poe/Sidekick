using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Data.Extensions;
using Sidekick.Data.Languages;

namespace Sidekick.Data.Texts;

public class GameTextProvider(
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    DataProvider dataProvider) : IInitializableService
{
    public int Priority => 100;

    public GameText Texts { get; private set; } = new();

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Texts = await dataProvider.Read<GameText>(game, DataType.Texts, currentGameLanguage.Language);
    }
}
