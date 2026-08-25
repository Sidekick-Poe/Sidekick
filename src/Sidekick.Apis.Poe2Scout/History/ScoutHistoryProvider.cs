using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe2Scout.History.Models;
using Sidekick.Game;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;
using Sidekick.Game.Scout;
namespace Sidekick.Apis.Poe2Scout.History;

public class ScoutHistoryProvider(
    IHttpClientFactory httpClientFactory,
    ScoutProvider scoutProvider,
    LeagueProvider leagueProvider,
    ILogger<ScoutHistoryProvider> logger)
{
    private static readonly Uri ApiBaseUrl = new("https://api.poe2scout.com/");

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public async Task<List<ScoutHistory>> GetHistories(Item item)
    {
        if (item.Definition.ScoutItems == null) return [];

        var results = new List<ScoutHistory>();
        foreach (var scoutItem in item.Definition.ScoutItems)
        {
            var result = await GetItemHistory(item, scoutItem);
            if (result != null) results.Add(result);
        }

        return results;
    }

    private async Task<ScoutHistory?> GetItemHistory(Item item, ScoutItem scoutItem)
    {
        var result = new ScoutHistory()
        {
            Item = scoutItem,
            Exalted = await GetItemLogs(item, scoutItem, scoutProvider.Exalted),
            Chaos = await GetItemLogs(item, scoutItem, scoutProvider.Chaos),
            Divine = await GetItemLogs(item, scoutItem, scoutProvider.Divine),
        };

        if (result.Exalted == null && result.Divine == null && result.Chaos == null)
        {
            logger.LogError("No history found for {ItemId}", scoutItem.ItemId);
            return null;
        }

        if ((result.Exalted?.Count ?? 0) == 0 && (result.Divine?.Count ?? 0) == 0 && (result.Chaos?.Count ?? 0) == 0)
        {
            logger.LogError("No history found for {ItemId}", scoutItem.ItemId);
            return null;
        }

        return result;
    }

    private async Task<List<ScoutHistoryLog>> GetItemLogs(Item item, ScoutItem scoutItem, ScoutItem? currency)
    {
        try
        {
            if (currency == null) return [];

            var realm = GetRealm(item);

            using var httpClient = GetHttpClient();
            var json = await httpClient.GetStringAsync($"{ApiBaseUrl}{realm}/Leagues/{leagueProvider.Current.ScoutValue}/Items/{scoutItem.ItemId}/History?logCount=24&referenceCurrency={currency.ApiId}");
            var result = JsonSerializer.Deserialize<ApiHistoryResult>(json, JsonSerializerOptions);
            if (result == null) return [];

            return result.Logs
                .OrderByDescending(x => x.Time)
                .Take(24)
                .OrderBy(x => x.Time)
                .ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting item history for {ItemId}", scoutItem.ItemId);
        }

        return [];
    }

    private string GetRealm(Item item)
    {
        return item.Game switch
        {
            GameType.Poe2 => "poe2",
            _ => "pc",
        };
    }

    private HttpClient GetHttpClient()
    {
        var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        return client;
    }
}
