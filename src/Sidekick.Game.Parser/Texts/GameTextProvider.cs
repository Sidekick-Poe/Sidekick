using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Game.Languages;

namespace Sidekick.Game.Texts;

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
        Texts = await dataProvider.Read<GameText>(game, GameDataType.Texts, currentGameLanguage.Language);
    }
}
