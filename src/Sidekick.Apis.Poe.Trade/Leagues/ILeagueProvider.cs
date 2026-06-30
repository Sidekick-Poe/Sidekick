using Sidekick.Data.Leagues;
namespace Sidekick.Apis.Poe.Trade.Leagues;

public interface ILeagueProvider
{
    /// <summary>
    /// Query to get a list of currently available leagues
    /// </summary>
    Task<List<League>> GetList();
}
