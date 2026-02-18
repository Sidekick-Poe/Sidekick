using Sidekick.Apis.Poe.Items;
using Sidekick.Data.Trade.Models;

namespace Sidekick.Data.Trade;

public class TradeDataProvider(DataProvider dataProvider)
{
    public async Task<List<TradeLeague>> GetLeagues(GameType gameType)
    {
        var result = await dataProvider.Read<TradeResult<List<TradeLeague>>>(gameType, "trade/leagues.en.json");
        return result.Result;
    }

    public async Task<List<TradeItemCategory>> GetItems(GameType gameType, string language)
    {
        var result = await dataProvider.Read<TradeResult<List<TradeItemCategory>>>(gameType, $"trade/items.{language}.json");
        return result.Result;
    }

    public async Task<List<TradeStaticItemCategory>> GetStaticItems(GameType gameType, string language)
    {
        var result = await dataProvider.Read<TradeResult<List<TradeStaticItemCategory>>>(gameType, $"trade/static.{language}.json");
        return result.Result;
    }

    public async Task<List<TradeStatCategory>> GetStats(GameType gameType, string language)
    {
        var result = await dataProvider.Read<TradeResult<List<TradeStatCategory>>>(gameType, $"trade/stats.{language}.json");
        return result.Result;
    }

    public async Task<List<TradeFilterCategory>> GetFilters(GameType gameType, string language)
    {
        var result = await dataProvider.Read<TradeResult<List<TradeFilterCategory>>>(gameType, $"trade/filters.{language}.json");
        return result.Result;
    }
}