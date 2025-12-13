using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Items.Models;
using Sidekick.Apis.PoeNinja.Stash.Models;
namespace Sidekick.Apis.PoeNinja.Stash;

public interface INinjaStashProvider
{
    Task<NinjaStash?> GetInfo(NinjaStashItem item);
    Task<ApiOverviewResult> FetchOverview(GameType game, string type, string? leagueOverride = null);
}
