using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Stash.Models;
namespace Sidekick.Apis.PoeNinja.Stash;

public interface INinjaStashProvider
{
    Task<NinjaStash?> GetUniqueInfo(string? name, int links);
    Task<NinjaStash?> GetGemInfo(string? name, int gemLevel);
    Task<NinjaStash?> GetMapInfo(string? name, int mapTier);
    Task<NinjaStash?> GetClusterInfo(string? grantText, int passiveCount, int itemLevel);
    Task<NinjaStash?> GetBaseTypeInfo(string? name, int itemLevel, Influences influences);

    Task<ApiOverviewResult> FetchOverview(GameType game, string type);
}
