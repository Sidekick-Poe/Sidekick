using System.Text.Json;
using Sidekick.Apis.Poe.Account.Clients;
using Sidekick.Apis.Poe.Account.Stash.Models;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Models;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Account.Stash;

public class StashService
(
    IAccountApiClient client,
    ISettingsService settingsService
) : IStashService
{
    public async Task<List<StashTab>> GetStashTabList()
    {
        var league = await settingsService.GetLeague();
        var response = await client.Fetch<StashTabListResult>($"stash/{league}");
        if (response == null || league == null) return [];

        var stashTabs = FlattenStashTabs(response.Tabs);
        return stashTabs.ToList();
    }

    private static IEnumerable<StashTab> FlattenStashTabs(List<StashTab> stashTabs)
    {
        foreach (var stashTab in stashTabs)
        {
            if (stashTab.Type != StashType.Folder) yield return stashTab;

            if (stashTab.Children == null) continue;

            foreach (var child in FlattenStashTabs(stashTab.Children))
            {
                yield return child;
            }
        }
    }

    public async Task<StashTab?> GetStashDetails(string id)
    {
        var league = await settingsService.GetLeague();
        var result = await client.Fetch<StashTabResult>($"stash/{league}/{id}");
        if (result == null || league == null) return null;

        if (result.Stash.Type == StashType.Map)
        {
            result.Stash.Items = await FetchMapStashItems(result.Stash);
        }
        else
        {
            result.Stash.Items = await FetchStashItems(result.Stash);
        }

        return result.Stash;
    }

    private async Task<List<ApiItem>> FetchStashItems(StashTab tab)
    {
        var items = new List<ApiItem>();

        if (tab.Items == null && tab.Children == null)
        {
            var league = await settingsService.GetLeague();
            var uri = string.IsNullOrEmpty(tab.Parent) ? $"stash/{league}/{tab.Id}" : $"stash/{league}/{tab.Parent}/{tab.Id}";

            var wrapper = await client.Fetch<StashTabResult>(uri);
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

    private async Task<List<ApiItem>> FetchMapStashItems(StashTab tab)
    {
        if (tab.Children == null)
        {
            return new();
        }

        var league = await settingsService.GetLeague();
        if (league == null) return [];

        var items = new List<ApiItem>();
        foreach (var childTab in tab.Children)
        {
            if (string.IsNullOrEmpty(childTab.Metadata?.Map?.Section)) continue;

            var name = childTab.Metadata?.Map?.Name;
            if (string.IsNullOrEmpty(name)) continue;

            if (name == "Lair of the Hydra"
                || name == "Maze of the Minotaur"
                || name == "Pit of the Chimera"
                || name == "Forge of the Phoenix")
            {
                name += " Map";
            }

            items.Add(new ApiItem
            {
                Id = childTab.Id,
                TypeLine = name,
                BaseType = name,
                Icon = childTab.Metadata?.Map?.Image,
                StackSize = childTab.Metadata?.Items,
                Properties =
                [
                    new()
                    {
                        Name = "Map Tier",
                        Values =
                        [
                            [
                                JsonDocument.Parse($"\"{childTab.Metadata?.Map?.Tier}\"").RootElement,
                                JsonDocument.Parse($"0").RootElement,
                            ],
                        ]
                    },
                ],
                Rarity = Rarity.Normal
            });
        }

        return items;
    }
}
