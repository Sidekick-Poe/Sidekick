using Sidekick.Apis.Poe.Stash.Models;

namespace Sidekick.Apis.Poe.Stash;

public interface IStashService
{
    Task<List<StashTab>?> GetStashTabList();

    Task<StashTabDetails?> GetStashDetails(string id);
}
