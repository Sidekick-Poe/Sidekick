using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Texts;
namespace Sidekick.Game.Parser.Texts;

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
