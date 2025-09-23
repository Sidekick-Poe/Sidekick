using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Account.Stash;
using Sidekick.Apis.Poe.Account.Stash.Models;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Models.Items;
using Sidekick.Apis.PoeNinja;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Wealth.Provider;

internal class WealthProvider
(
    ILogger<WealthProvider> logger,
    ISettingsService settingsService,
    IStashService stashService,
    IPoeNinjaClient poeNinjaClient,
    IApiInvariantItemProvider apiInvariantItemProvider,
    DbContextOptions<SidekickDbContext> dbContextOptions
)
{
    public event Action? OnFilterChanged;

    public event Action? OnStatusChanged;

    public event Action? OnRefreshed;

    public WealthRunStatus Status { get; set; } = WealthRunStatus.Stopped;

    public List<string> PendingStashIds { get; } = [];

    public void Start()
    {
        if (Status == WealthRunStatus.Running) return;
        _ = Run();
    }

    public void FiltersChanged()
    {
        OnFilterChanged?.Invoke();
    }

    private async Task Run()
    {
        logger.LogInformation("[WealthProvider] Starting wealth run.");
        Status = WealthRunStatus.Running;
        PendingStashIds.Clear();
        OnStatusChanged?.Invoke();

        try
        {
            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            if (leagueId == null)
            {
                logger.LogError("[WealthProvider] The league id is not set.");
                return;
            }

            await using var database = new SidekickDbContext(dbContextOptions);
            var tabs = await database.WealthStashes.Where(x => x.League == leagueId).ToListAsync();

            PendingStashIds.AddRange(tabs.Select(x => x.Id));
            OnStatusChanged?.Invoke();

            var date = DateTimeOffset.Now;

            foreach (var tab in tabs)
            {
                var stash = await stashService.GetStashDetails(tab.Id);
                if (stash == null) continue;

                logger.LogInformation($"[WealthProvider] Parsing {stash.Name}");
                await ParseStash(database, leagueId, stash);

                logger.LogInformation($"[WealthProvider] Taking stash snapshot {stash.Name}");
                await TakeStashSnapshot(database, leagueId, stash, date);

                logger.LogInformation($"[WealthProvider] Stash completed {stash.Name}");
                PendingStashIds.Remove(stash.Id);
                OnStatusChanged?.Invoke();
            }

            logger.LogInformation("[WealthProvider] Taking full snapshot");
            await TakeFullSnapshot(database, leagueId);

            Status = WealthRunStatus.Completed;
            OnStatusChanged?.Invoke();
            OnRefreshed?.Invoke();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[WealthProvider] Run failed.");
            Status = WealthRunStatus.Failed;
            OnStatusChanged?.Invoke();
        }
    }

    private async Task ParseStash(SidekickDbContext database, string leagueId, StashTab stash)
    {
        var dbStash = await database.WealthStashes.FirstOrDefaultAsync(x => x.Id == stash.Id);
        if (dbStash == null)
        {
            dbStash = new WealthStash()
            {
                Id = stash.Id,
                Name = stash.Name,
                Parent = stash.Parent,
                League = leagueId,
                Type = stash.Type.ToString(),
                Total = 0,
                LastUpdate = DateTimeOffset.Now,
            };
            database.WealthStashes.Add(dbStash);
        }
        else
        {
            dbStash.Name = stash.Name;
            dbStash.Parent = stash.Parent;
            dbStash.League = leagueId;
            dbStash.Type = stash.Type.ToString();
            dbStash.Total = 0;
            dbStash.LastUpdate = DateTimeOffset.Now;
        }

        // Game Item Removed (Traded, Used, Destroyed, etc.)
        var dbItems = await database.WealthItems.Where(x => x.StashId == stash.Id).ToListAsync();
        database.WealthItems.RemoveRange(dbItems);
        await database.SaveChangesAsync();

        if (stash.Items == null) return;

        // Add / Update Items
        var items = new List<WealthItem>();
        foreach (var item in stash.Items)
        {
            var parsedItem = await ParseItem(leagueId, stash, item);
            if (parsedItem == null) continue;
            items.Add(parsedItem);
        }

        dbStash.Total = items.Sum(x => x.Total);
        database.WealthItems.AddRange(items);
        await database.SaveChangesAsync();
    }

    private async Task<WealthItem?> ParseItem(string leagueId, StashTab stash, ApiItem item)
    {
        if (string.IsNullOrEmpty(item.Id))
        {
            logger.LogError("[WealthProvider] Could not parse item due to missing id.");
            return null;
        }

        var name = string.IsNullOrEmpty(item.Name) ? item.Type : item.Name;
        if (string.IsNullOrEmpty(name))
        {
            logger.LogError("[WealthProvider] Could not parse item due to missing name.");
            return null;
        }

        var invariantItem = apiInvariantItemProvider.NameAndTypeDictionary.GetValueOrDefault(name)?.FirstOrDefault();
        if (invariantItem?.Category == null)
        {
            logger.LogError("[WealthProvider] Could not parse item due to missing invariant: " + name);
            return null;
        }

        var dbItem = new WealthItem
        {
            Id = item.Id,
            Count = item.StackSize ?? 1,
            Icon = item.Icon,
            League = leagueId,
            ItemLevel = item.ItemLevel,
            GemLevel = item.GemLevel,
            MapTier = item.MapTier,
            MaxLinks = item.MaxLinks,
            Name = name,
            Category = invariantItem.Category.ToString(),
            StashId = stash.Id,
            Price = await GetItemPrice(invariantItem, item),
        };

        dbItem.Total = dbItem.Count * dbItem.Price;

        return dbItem;
    }

    private async Task<decimal> GetItemPrice(ItemApiInformation invariantItem, ApiItem item)
    {
        if (invariantItem.Category == Category.Unknown)
        {
            logger.LogError($"[WealthProvider] Could not price due to missing category: {item.Name}.");
            return 0;
        }

        var price = await poeNinjaClient.GetPriceInfo(invariantItem.Name,
                                                      invariantItem.Type,
                                                      invariantItem.Category,
                                                      item.GemLevel,
                                                      item.MapTier,
                                                      item.IsRelic,
                                                      item.MaxLinks);
        if (price != null) return price.Price;

        logger.LogError($"[WealthProvider] Could not price: {item.Name}.");
        return 0;
    }

    private static async Task TakeStashSnapshot(SidekickDbContext database, string leagueId, StashTab stash, DateTimeOffset date)
    {
        var totals = await database.WealthItems.Where(x => x.League == leagueId).Where(x => x.StashId == stash.Id).Select(x => x.Total).ToListAsync();

        database.WealthStashSnapshots.Add(new WealthStashSnapshot()
        {
            Date = date,
            League = leagueId,
            StashId = stash.Id,
            Total = totals.Sum(),
        });

        await database.SaveChangesAsync();
    }

    private static async Task TakeFullSnapshot(SidekickDbContext database, string leagueId)
    {
        var totals = await database.WealthItems.Where(x => x.League == leagueId).Select(x => x.Total).ToListAsync();

        database.WealthFullSnapshots.Add(new WealthFullSnapshot()
        {
            Date = DateTimeOffset.Now,
            League = leagueId,
            Total = totals.Sum(),
        });

        await database.SaveChangesAsync();
    }

    public async Task Clear()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        if (leagueId == null)
        {
            logger.LogError("[WealthProvider] The league id is not set.");
            return;
        }

        await using var database = new SidekickDbContext(dbContextOptions);

        var items = await database.WealthItems.Where(x => x.League == leagueId).ToListAsync();
        database.WealthItems.RemoveRange(items);
        await database.SaveChangesAsync();

        var stashes = await database.WealthStashes.Where(x => x.League == leagueId).ToListAsync();
        database.WealthStashes.RemoveRange(stashes);
        await database.SaveChangesAsync();

        var stashSnapshots = await database.WealthStashSnapshots.Where(x => x.League == leagueId).ToListAsync();
        database.WealthStashSnapshots.RemoveRange(stashSnapshots);
        await database.SaveChangesAsync();

        var fullSnapshots = await database.WealthFullSnapshots.Where(x => x.League == leagueId).ToListAsync();
        database.WealthFullSnapshots.RemoveRange(fullSnapshots);
        await database.SaveChangesAsync();

        OnRefreshed?.Invoke();
    }
}
