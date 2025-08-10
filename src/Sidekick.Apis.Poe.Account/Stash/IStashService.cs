using Sidekick.Apis.Poe.Account.Stash.Models;

namespace Sidekick.Apis.Poe.Account.Stash;

public interface IStashService
{
    Task<List<StashTab>> GetStashTabList();

    Task<StashTab?> GetStashDetails(string id);
}
