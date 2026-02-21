using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Items.Models;

namespace Sidekick.Data.Ninja;

public class NinjaDataProvider(DataProvider dataProvider)
{
    public async Task<List<NinjaExchangeItem>> GetExchange(GameType gameType)
    {
        return await dataProvider.Read<List<NinjaExchangeItem>>(gameType, "ninja/exchange.json");
    }

    public async Task<List<NinjaStashItem>> GetStash(GameType gameType)
    {
        return await dataProvider.Read<List<NinjaStashItem>>(gameType, "ninja/stash.json");
    }
}