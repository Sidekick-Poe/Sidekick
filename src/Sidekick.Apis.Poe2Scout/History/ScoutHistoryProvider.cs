using Sidekick.Apis.Poe2Scout.Clients;
using Sidekick.Apis.Poe2Scout.History.Models;
using Sidekick.Apis.Poe2Scout.Items;
namespace Sidekick.Apis.Poe2Scout.History;

public class ScoutHistoryProvider(
    IScoutClient scoutClient,
    IScoutItemProvider scoutItemProvider) : IScoutHistoryProvider
{
    public async Task<ScoutHistory?> GetItemHistory(int itemId)
    {
        return new ScoutHistory()
        {
            Exalted = await GetItemLogs(itemId, "exalted"),
        };
    }

    private async Task<List<ScoutHistoryLog>> GetItemLogs(int itemId, string currency)
    {
        var result = await scoutClient.Fetch<ApiHistoryResult>($"items/{itemId}/history", new Dictionary<string, string?>()
        {
            {
                "logCount", "24"
            },
            {
                "referenceCurrency", currency
            },
        });
        if (result == null) return [];

        return result.Logs
            .OrderByDescending(x => x.Time)
            .Take(24)
            .OrderBy(x => x.Time)
            .ToList();
    }

    public async Task<ScoutHistory?> GetCurrencyHistory(int itemId)
    {
        var exaltedOrb = await scoutItemProvider.GetExaltedOrb(); // 290
        var chaosOrb = await scoutItemProvider.GetChaosOrb(); // 287
        var divineOrb = await scoutItemProvider.GetDivineOrb(); // 291

        return new ScoutHistory()
        {
            Exalted = await GetCurrencyLogs(itemId, exaltedOrb?.ItemId),
            Chaos = await GetCurrencyLogs(itemId, chaosOrb?.ItemId),
            Divine = await GetCurrencyLogs(itemId, divineOrb?.ItemId),
        };
    }

    private async Task<List<ScoutHistoryLog>> GetCurrencyLogs(int itemId, int? currencyId)
    {
        if (currencyId == null) return [];

        var result = await scoutClient.Fetch<ApiCurrencyHistoryResult>($"currencyExchange/PairHistory", new Dictionary<string, string?>()
        {
            {
                "limit", "24"
            },
            {
                "currencyOneItemId", itemId.ToString()
            },
            {
                "currencyTwoItemId", currencyId.ToString()
            },
        });
        if (result == null) return [];

        return result.History
            .Select(x=> new ScoutHistoryLog()
            {
                Time = x.DateTime,
                Price = x.Value,
                Quantity = x.Quantity,
            })
            .OrderByDescending(x => x.Time)
            .Take(24)
            .OrderBy(x => x.Time)
            .ToList();
    }

}
