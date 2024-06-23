using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Stash.Models;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Stash
{
    public class StashService(
        IPoeApiClient client,
        ISettingsService settingsService,
        IItemMetadataParser itemMetadataParser,
        ILogger<StashService> logger) : IStashService
    {
        public async Task<List<StashTab>?> GetStashTabList()
        {
            try
            {
                var settings = settingsService.GetSettings();
                var response = await client.Fetch<ApiStashTabList>($"stash/{settings.LeagueId}");
                if (response == null)
                {
                    return null;
                }

                var result = new List<StashTab>();
                FillStashTabs(result, response.StashTabs);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "The GetStashTabList() method failed to fetch data successfully.");
                return null;
            }
        }

        private string GetLeagueId() => settingsService.GetSettings().LeagueId ?? throw new SidekickException("Could not find a valid league.");

        private void FillStashTabs(List<StashTab> list, List<ApiStashTab> apiStashTabs)
        {
            foreach (var stashTab in apiStashTabs)
            {
                if (stashTab.StashType != StashType.Folder)
                {
                    list.Add(new()
                    {
                        Name = stashTab.Name,
                        Id = stashTab.Id,
                        League = GetLeagueId(),
                        Type = stashTab.StashType,
                    });
                }

                if (stashTab.Children == null)
                {
                    continue;
                }

                FillStashTabs(list, stashTab.Children);
            }
        }

        public async Task<StashTabDetails?> GetStashDetails(string id)
        {
            try
            {
                var wrapper = await client.Fetch<ApiStashTabWrapper>($"stash/{GetLeagueId()}/{id}");
                if (wrapper == null)
                {
                    return null;
                }

                var details = new StashTabDetails()
                {
                    Id = wrapper.Stash.Id,
                    Parent = wrapper.Stash.Parent,
                    League = GetLeagueId(),
                    Name = wrapper.Stash.Name,
                    Type = wrapper.Stash.StashType
                };

                List<APIStashItem> items;
                if (details.Type == StashType.Map)
                {
                    items = await FetchMapStashItems(wrapper.Stash);
                }
                else
                {
                    items = await FetchStashItems(wrapper.Stash);
                }

                ParseItems(details, items);

                return details;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "The GetStashDetails() method failed to fetch data successfully.");
                return null;
            }
        }

        private async Task<List<APIStashItem>> FetchStashItems(ApiStashTab tab)
        {
            var items = new List<APIStashItem>();

            if (tab.Items == null && tab.Children == null)
            {
                var uri = string.IsNullOrEmpty(tab.Parent) ?
                    $"stash/{GetLeagueId()}/{tab.Id}" :
                    $"stash/{GetLeagueId()}/{tab.Parent}/{tab.Id}";

                var wrapper = await client.Fetch<ApiStashTabWrapper>(uri);
                if (wrapper?.Stash.Items != null)
                {
                    tab = wrapper.Stash;
                }
            }

            if (tab.Items != null)
            {
                items.AddRange(tab.Items);
            }

            if (tab.Children == null)
            {
                return items;
            }

            foreach (var childTab in tab.Children)
            {
                items.AddRange(await FetchStashItems(childTab));
            }

            return items;
        }

        private async Task<List<APIStashItem>> FetchMapStashItems(ApiStashTab tab)
        {
            if (tab.Children == null)
            {
                return new();
            }

            var items = new List<APIStashItem>();
            foreach (var childTab in tab.Children)
            {
                if (childTab.Metadata?.map?.section == "special")
                {
                    items.AddRange(await FetchStashItems(childTab));
                }
                else
                {
                    items.Add(new APIStashItem
                    {
                        id = childTab.Id,
                        typeLine = childTab.Metadata?.map?.name,
                        baseType = childTab.Metadata?.map?.name,
                        name = "",
                        icon = childTab.Metadata?.map?.image,
                        league = GetLeagueId(),
                        ilvl = -1,
                        stackSize = childTab.Metadata?.items,
                        properties =
                        [
                            new()
                            {
                                name = "Map Tier",
                                values =
                                [
                                    [
                                        childTab.Metadata?.map?.tier,
                                    ],
                                ]
                            },
                        ],
                        frameType = FrameType.Normal
                    });
                }
            }

            return items;
        }

        private void ParseItems(StashTabDetails details, List<APIStashItem> items)
        {
            foreach (var item in items)
            {
                details.Items.Add(new()
                {
                    Id = item.id,
                    Name = item.getFriendlyName(),
                    Stash = details.Id,
                    Icon = item.icon,
                    League = details.League,
                    ItemLevel = item.ilvl,
                    GemLevel = item.getGemLevel(),
                    MapTier = item.getMapTier(),
                    MaxLinks = item.getLinkCount(),
                    Count = item.stackSize ?? 1,
                    Category = GetItemCategory(item),
                });
            }
        }

        private Category GetItemCategory(APIStashItem item)
        {
            var name = item.getFriendlyName();
            var metadata = itemMetadataParser.Parse(name, item.typeLine);
            if (metadata != null)
            {
                return metadata.Category;
            }

            logger.LogError($"[Stash] Could not retrieve metadeta: {item.getFriendlyName()}");
            return Category.Unknown;

        }
    }
}
