using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Stash.Models;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Stash
{
    public class StashService : IStashService
    {
        private IPoeApiClient client;
        private readonly ISettings settings;
        private ILogger<IStashService> _logger;

        public StashService(
            IPoeApiClient client,
            ISettings settings)
        {
            this.client = client;
            this.settings = settings;
        }

        public async Task<List<APIStashItem>> GetStashItems(ApiStashTab stashTab, bool hasParent = false)
        {
            List<APIStashItem> items = new List<APIStashItem>();

            if (stashTab.children != null)
            {
                foreach (ApiStashTab childStashTab in stashTab.children)
                {
                    ApiStashTab childStashDetails = await GetStashTab($"{stashTab.id}/{childStashTab.id}");

                    if (childStashDetails != null && childStashDetails.items != null)
                    {
                        items.AddRange(childStashDetails.items);
                    }
                }
            }

            if (hasParent && !String.IsNullOrEmpty(stashTab.parent))
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

        public async Task<List<APIStashItem>> GetMapStashItems(ApiStashTab stashTab)
        {
            List<APIStashItem> items = new List<APIStashItem>();

            if (stashTab.children != null)
            {
                foreach (ApiStashTab childStashTab in stashTab.children)
                {
                    if (childStashTab.metadata.map.section == "special")
                    {
                        items.AddRange(await GetStashItems(childStashTab, true));
                    }
                    else
                    {
                        items.Add(new APIStashItem
                        {
                            id = childStashTab.id,
                            typeLine = childStashTab.metadata.map.name,
                            baseType = childStashTab.metadata.map.name,
                            name = "",
                            icon = childStashTab.metadata.map.image,
                            league = settings.LeagueId,
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

        public async Task<ApiStashTab> GetStashTab(string stashId)
        {
            var wrapper = await client.Fetch<ApiStashTabWrapper>($"stash/{settings.LeagueId}/{stashId}");
            return wrapper.stash;
        }

        public async Task<List<StashTab>?> GetStashTabList()
        {
            try
            {
                var response = await client.Fetch<ApiStashTabList>($"stash/{settings.LeagueId}");

                var result = new List<StashTab>();
                FillStashTabs(result, response.StashTabs);

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void FillStashTabs(List<StashTab> list, List<ApiStashTab> apiStashTabs)
        {
            foreach (var stashTab in apiStashTabs)
            {
                if (!stashTab.IsFolder)
                {
                    list.Add(new()
                    {
                        Name = stashTab.name,
                        Id = stashTab.id,
                    });
                }

                if (stashTab.children == null)
                {
                    continue;
                }

                FillStashTabs(list, stashTab.children);
            }
        }
    }
}
