using Sidekick.Apis.Poe.Stash.Models;

namespace Sidekick.Apis.Poe.Stash
{
    public interface IStashService
    {
        Task<List<StashTab>?> GetStashTabList();

        Task<ApiStashTab> GetStashTab(string stash);

        Task<List<APIStashItem>> GetStashItems(ApiStashTab stashTab, bool hasParent = false);

        Task<List<APIStashItem>> GetMapStashItems(ApiStashTab stashTab);
    }
}
