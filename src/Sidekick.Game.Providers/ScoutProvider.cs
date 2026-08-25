using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Scout;
namespace Sidekick.Game.Providers;

public class ScoutProvider(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService
) : IInitializableService
{
    public Dictionary<int, ScoutItem> Items { get; private set; } = [];

    public ScoutItem? Exalted { get; private set; }
    public ScoutItem? Chaos { get; private set; }
    public ScoutItem? Divine { get; private set; }

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        var exchangeItems = await dataProvider.Read<List<ScoutItem>>(game, GameDataType.ScoutItems, currentGameLanguage.Language);
        Items = exchangeItems.ToDictionary(x => x.ItemId);

        Exalted = Items.Values.FirstOrDefault(x => x.Text == "Exalted Orb");
        Chaos = Items.Values.FirstOrDefault(x => x.Text == "Chaos Orb");
        Divine = Items.Values.FirstOrDefault(x => x.Text == "Divine Orb");
    }
}
