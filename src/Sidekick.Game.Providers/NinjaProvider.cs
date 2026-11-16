using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Game.Ninja;
using Sidekick.Game.Providers.Languages;
namespace Sidekick.Game.Providers;

public class NinjaProvider(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService
) : IInitializableService
{
    public Dictionary<string, List<NinjaExchangeItem>> ExchangeItems { get; private set; } = [];
    public Dictionary<string, List<NinjaStashItem>> StashItems { get; private set; } = [];

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        var exchangeItems = await dataProvider.Read<List<NinjaExchangeItem>>(game, GameDataType.NinjaExchangeItems, currentGameLanguage.Language);
        ExchangeItems = exchangeItems
            .GroupBy(x => x.DetailsId)
            .ToDictionary(x => x.First().DetailsId, x => x.ToList());

        var stashItems = await dataProvider.Read<List<NinjaStashItem>>(game, GameDataType.NinjaStashItems, currentGameLanguage.Language);
        StashItems = stashItems
            .GroupBy(x => x.DetailsId)
            .ToDictionary(x => x.First().DetailsId, x => x.ToList());
    }
}
