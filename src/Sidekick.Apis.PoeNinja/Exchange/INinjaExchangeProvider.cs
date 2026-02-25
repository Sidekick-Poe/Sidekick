using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Ninja.Models;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    Task<NinjaCurrency?> GetInfo(NinjaExchangeItem item);
    Task<ApiOverviewResult> FetchOverview(GameType game, string type, string? leagueOverride = null);
}
