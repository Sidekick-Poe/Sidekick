using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Stash.Models;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Stash
{
    public class StashService: IStashService
    {
        private IPoeApiClient _client;

        public StashService(IPoeApiClient PoeApiClient)
        {
            _client = PoeApiClient;
        }

        public async Task<List<APIStashItem>> GetStashItems(APIStashTab stashTab)
        {
            List<APIStashItem> items = new List<APIStashItem>();
 
            if (stashTab.children != null)
            {
                foreach (APIStashTab childStashTab in stashTab.children)
                {
                    APIStashTab childStashDetails = await GetStashTab($"{stashTab.id}/{childStashTab.id}");

                    if (childStashDetails != null && childStashDetails.items != null)
                    {
                        items.AddRange(childStashDetails.items);
                    }
                }
            }

            if (stashTab.items != null)
            {
                items.AddRange(stashTab.items);
            }

            return items;
        }

        public async Task<APIStashTab> GetStashTab(string stashId)
        {
            var wrapper = await _client.Fetch<APIStashTabWrapper>($"stash/Ancestor/{stashId}");

            return wrapper.stash;
        }

        public async Task<APIStashList> GetStashList()
        {
            return await _client.Fetch<APIStashList>($"stash/Ancestor");
        }
    }
}
