using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Exchange.Models;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    Task<NinjaCurrency?> GetInfo(string? invariantId);
    Task<ApiOverviewResult> FetchOverview(GameType game, string type, string? leagueOverride = null);
}
