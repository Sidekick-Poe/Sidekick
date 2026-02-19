using Sidekick.Apis.Poe.Items;
using Sidekick.Data.Trade.Models;
using Sidekick.Data.Trade.Models.Raw;

namespace Sidekick.Data.Trade;

public class TradeDataProvider(DataProvider dataProvider)
{
    public async Task<List<RawTradeLeague>> GetLeagues(GameType gameType)
    {
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeLeague>>>(gameType, "trade/raw/leagues.en.json");
        return result.Result;
    }

    public async Task<List<RawTradeItemCategory>> GetItems(GameType gameType, string language)
    {
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeItemCategory>>>(gameType, $"trade/raw/items.{language}.json");
        return result.Result;
    }

    public async Task<List<RawTradeStaticItemCategory>> GetStaticItems(GameType gameType, string language)
    {
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeStaticItemCategory>>>(gameType, $"trade/raw/static.{language}.json");
        return result.Result;
    }

    public async Task<List<RawTradeStatCategory>> GetRawStats(GameType gameType, string language)
    {
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeStatCategory>>>(gameType, $"trade/raw/stats.{language}.json");
        return result.Result;
    }

    public async Task<List<TradeStatDefinition>> GetStats(GameType gameType, string language)
    {
        return await dataProvider.Read<List<TradeStatDefinition>>(gameType, $"trade/stats.{language}.json");
    }

    public async Task<List<RawTradeFilterCategory>> GetFilters(GameType gameType, string language)
    {
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeFilterCategory>>>(gameType, $"trade/raw/filters.{language}.json");
        return result.Result;
    }
}