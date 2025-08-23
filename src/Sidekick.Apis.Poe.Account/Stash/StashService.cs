using System.Text.Json;
using Sidekick.Apis.Poe.Account.Clients;
using Sidekick.Apis.Poe.Account.Stash.Models;
using Sidekick.Apis.Poe.Trade.Models.Items;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
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
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var response = await client.Fetch<StashTabListResult>($"stash/{leagueId.GetUrlSlugForLeague()}");
        if (response == null || leagueId == null) return [];

        return FlattenStashTabs(response.Tabs);
    }

    private static List<StashTab> FlattenStashTabs(List<StashTab> stashTabs)
    {
        var result = new List<StashTab>();

        foreach (var stashTab in stashTabs)
        {
            if (stashTab.Type != StashType.Folder) result.Add(stashTab);

            if (stashTab.Children == null) continue;

            result.AddRange(FlattenStashTabs(stashTab.Children));
        }

        return result;
    }

    public async Task<StashTab?> GetStashDetails(string id)
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var result = await client.Fetch<StashTabResult>($"stash/{leagueId.GetUrlSlugForLeague()}/{id}");
        if (result == null || leagueId == null) return null;

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
            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var uri = string.IsNullOrEmpty(tab.Parent) ? $"stash/{leagueId.GetUrlSlugForLeague()}/{tab.Id}" : $"stash/{leagueId}/{tab.Parent}/{tab.Id}";

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

        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        if (leagueId == null)
        {
            return
            [
            ];
        }

        var items = new List<ApiItem>();
        foreach (var childTab in tab.Children)
        {
            if (childTab.Metadata?.Map?.Section == "special")
            {
                items.AddRange(await FetchStashItems(childTab));
            }
            else
            {
                items.Add(new ApiItem
                {
                    Id = childTab.Id,
                    TypeLine = childTab.Metadata?.Map?.Name,
                    BaseType = childTab.Metadata?.Map?.Name,
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
        }

        return items;
    }
}
