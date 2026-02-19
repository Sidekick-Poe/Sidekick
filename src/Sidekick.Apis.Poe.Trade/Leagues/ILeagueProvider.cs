using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.Leagues;

public interface ILeagueProvider
{
    /// <summary>
    /// Query to get a list of currently available leagues
    /// </summary>
    /// <param name="fromCache">If true, the leagues will be fetched from the cache if possible; if false, from the API</param>
    Task<List<TradeLeague>> GetList(bool fromCache);
}
