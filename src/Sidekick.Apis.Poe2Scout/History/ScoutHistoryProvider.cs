using Sidekick.Apis.Poe2Scout.Clients;
using Sidekick.Apis.Poe2Scout.History.Models;
using Sidekick.Apis.Poe2Scout.Items;
using Sidekick.Apis.Poe2Scout.Items.Models;
using Sidekick.Common.Game.Items;
namespace Sidekick.Apis.Poe2Scout.History;

public class ScoutHistoryProvider(
    IScoutItemProvider itemProvider,
    IScoutClient scoutClient) : IScoutHistoryProvider
{
    public async Task<ScoutHistory?> GetHistory(Rarity rarity, string? name, string? type)
    {
        name ??= type;
        if (string.IsNullOrEmpty(name)) return null;

        var items = await (rarity == Rarity.Unique ? itemProvider.GetUniqueItems() : itemProvider.GetCurrencyItems());
        var item = items.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name == name);
        item ??= items.FirstOrDefault(x => !string.IsNullOrEmpty(x.Text) && x.Text == name);
        if (item == null) return null;

        return new ScoutHistory()
        {
            Category = item.CategoryApiId,
            Chaos = await GetLogs(item, "chaos"),
            Exalted = await GetLogs(item, "exalted"),
            Divine = await GetLogs(item, "divine"),
        };
    }

    private async Task<List<ScoutHistoryLog>> GetLogs(ScoutItem item, string currency)
    {
        bool hasMore;
        var logs = new List<ScoutHistoryLog>();
        do
        {
            var result = await scoutClient.Fetch<ApiHistoryResult>($"items/{item.ItemId}/history", new Dictionary<string, string?>()
            {
                {
                    "logCount", "336"
                },
                {
                    "referenceCurrency", currency
                },
            });
            if (result == null) return logs;

            logs.AddRange(result.Logs);

            hasMore = result.HasMore;
        } while (hasMore);

        return logs;
    }
}
