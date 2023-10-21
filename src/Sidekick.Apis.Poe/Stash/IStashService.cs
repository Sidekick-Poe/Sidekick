using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sidekick.Apis.Poe.Stash.Models;

namespace Sidekick.Apis.Poe.Stash
{
    public interface IStashService
    {

        Task<APIStashList> GetStashList();
        Task<APIStashTab> GetStashTab(string stash);
        Task<List<APIStashItem>> GetStashItems(APIStashTab stashTab);
        Task<List<APIStashItem>> GetMapStashItems(APIStashTab stashTab);
    }
}
