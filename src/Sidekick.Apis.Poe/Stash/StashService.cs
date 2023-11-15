using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Stash.Models;

namespace Sidekick.Apis.Poe.Stash
{
    public class StashService: IStashService
    {
        private IPoeApiClient _client;
        private ILogger<IStashService> _logger;

        public StashService(IPoeApiClient PoeApiClient)
        {
            _client = PoeApiClient;
        }

        public async Task<List<APIStashItem>> GetStashItems(APIStashTab stashTab, bool hasParent = false)
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

            if(hasParent && !String.IsNullOrEmpty(stashTab.parent))
            {
                var stashDetails = await GetStashTab($"{stashTab.parent}/{stashTab.id}");

                if (stashDetails != null && stashDetails.items != null)
                {
                    items.AddRange(stashDetails.items);
                }
            }

            if (stashTab.items != null)
            {
                items.AddRange(stashTab.items);
            }

            return items;
        }


        public async Task<List<APIStashItem>> GetMapStashItems(APIStashTab stashTab)
        {
            List<APIStashItem> items = new List<APIStashItem>();

            if (stashTab.children != null)
            {
                foreach (APIStashTab childStashTab in stashTab.children)
                {
                    if(childStashTab.metadata.map.section == "special") {
                        items.AddRange(await GetStashItems(childStashTab, true));
                    } else {
                        items.Add(new APIStashItem
                        {
                            id = childStashTab.id,
                            typeLine = childStashTab.metadata.map.name,
                            baseType = childStashTab.metadata.map.name,
                            name = "",
                            icon = childStashTab.metadata.map.image,
                            league = "Ancestor",
                            ilvl = -1,
                            stackSize = childStashTab.metadata.items,
                            properties = new List<APIItemProperty>() {
                            new APIItemProperty() {
                                name = "Map Tier",
                                values = new List<List<object>>() { new List<object>() { childStashTab.metadata.map.tier } }
                        } },
                            frameType = FrameType.Normal
                        });
                    }
                }
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