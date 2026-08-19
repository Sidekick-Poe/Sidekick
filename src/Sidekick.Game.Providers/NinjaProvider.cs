using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Ninja;
namespace Sidekick.Game.Providers;

public class NinjaProvider(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService
) : IInitializableService
{
    public Dictionary<string, NinjaExchangeItem> ExchangeItems { get; private set; } = [];
    public Dictionary<string, NinjaStashItem> StashItems { get; private set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        var exchangeItems = await dataProvider.Read<List<NinjaExchangeItem>>(game, GameDataType.NinjaExchangeItems, currentGameLanguage.Language);
        ExchangeItems = exchangeItems.ToDictionary(x => x.DetailsId);

        var stashItems = await dataProvider.Read<List<NinjaStashItem>>(game, GameDataType.NinjaStashItems, currentGameLanguage.Language);
        StashItems = stashItems.ToDictionary(x => x.DetailsId);
    }
}
