using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Data.Items;
using Sidekick.Data.Ninja;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    Task<NinjaCurrency?> GetInfo(NinjaExchangeItem item);
    Task<ApiOverviewResult> FetchOverview(GameType game, string type, string? leagueOverride = null);
}
