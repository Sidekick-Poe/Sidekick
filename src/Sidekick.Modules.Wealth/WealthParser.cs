using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Clients.Exceptions;
using Sidekick.Apis.Poe.Stash;
using Sidekick.Apis.Poe.Stash.Models;
using Sidekick.Apis.PoeNinja;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Wealth
{
    internal class WealthParser(
        DbContextOptions<SidekickDbContext> dbContextOptions,
        ISettingsService settingsService,
        IStashService stashService,
        IPoeNinjaClient poeNinjaClient,
        ILogger<WealthParser> logger)
    {
        public event Action? OnLogsChanged;
        public event Action? OnStashParsed;
        public event Action? OnSnapshotTaken;

        public Queue<(Guid Id, DateTimeOffset Date, string Icon, string Color, string Message)> Logs { get; set; } = new();

        private Thread? RunningThread { get; set; }
        private CancellationTokenSource? CancellationTokenSource { get; set; }

        public void Start()
        {
            if (IsRunning())
            {
                return;
            }

            CancellationTokenSource = new CancellationTokenSource();
            RunningThread = new Thread(ParseLoop);
            RunningThread.Start();
            Log("Icons.Material.Filled.PlayCircle", "Color.Success", $"Tracker Started.");
        }

        public void Stop()
        {
            if (CancellationTokenSource == null || !IsRunning())
            {
                return;
            }

            CancellationTokenSource.Cancel();
            Log("Icons.Material.Filled.StopCircle", "Color.Warning", $"Tracker Stopped.");
        }

        public bool IsRunning() => CancellationTokenSource?.IsCancellationRequested == false;

        private void Log(string icon, string color, string message)
        {
            Logs.Enqueue((Guid.NewGuid(), DateTimeOffset.Now, icon, color, message));
            if (Logs.Count > 50)
            {
                Logs.Dequeue();
            }

            OnLogsChanged?.Invoke();
        }

        private async void ParseLoop()
        {
            if (CancellationTokenSource == null)
            {
                return;
            }

            var lastRun = DateTimeOffset.Now;
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    await using var database = new SidekickDbContext(dbContextOptions);
                    var tabs = await settingsService.GetString(SettingKeys.WealthTrackedTabs);

                    foreach (var id in tabs?.Split(',') ?? [])
                    {
                        if (CancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }

                        var stash = await stashService.GetStashDetails(id);
                        if (stash == null || CancellationTokenSource.IsCancellationRequested)
                        {
                            continue;
                        }

                        Log("Icons.Material.Filled.HourglassTop", "Color.Info", $"[{stash.Name}] Updating...");
                        await ParseStash(database, stash);
                        await TakeStashSnapshot(database, stash);
                        Log("Icons.Material.Filled.HourglassBottom", "Color.Info", $"[{stash.Name}] Updated.");
                    }

                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    await TakeFullSnapshot(database);
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    var delay = (lastRun + TimeSpan.FromMinutes(10)) - DateTimeOffset.Now;
                    if (delay.TotalMilliseconds > 0)
                    {
                        await Task.Delay(delay);
                    }

                    lastRun = DateTimeOffset.Now;
                }
                catch (PoeApiException)
                {
                    Log("Icons.Material.Filled.Error", "Color.Error", $"Exception! Something wrong happened while working on the tracker. Are you authenticated?");
                    Stop();
                }
            }
        }

        private async Task ParseStash(SidekickDbContext database, StashTabDetails stash)
        {
            var dbStash = database.WealthStashes.FirstOrDefault(x => x.Id == stash.Id);
            if (dbStash == null)
            {
                dbStash = new WealthStash()
                {
                    Id = stash.Id,
                    Name = stash.Name,
                    Parent = stash.Parent,
                    League = stash.League,
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
                dbStash.League = stash.League;
                dbStash.Type = stash.Type.ToString();
                dbStash.Total = 0;
                dbStash.LastUpdate = DateTimeOffset.Now;
            }

            // Game Item Removed (Traded, Used, Destroyed, etc.)
            var dbItems = database.WealthItems.Where(x => x.StashId == stash.Id);
            database.WealthItems.RemoveRange(dbItems);
            await database.SaveChangesAsync();

            // Add / Update Items
            var items = new List<WealthItem>();
            foreach (var item in stash.Items)
            {
                items.Add(await ParseItem(item));
            }

            dbStash.Total = items.Sum(x => x.Total);
            database.WealthItems.AddRange(items);
            await database.SaveChangesAsync();

            OnStashParsed?.Invoke();
        }

        private async Task<WealthItem> ParseItem(StashItem item)
        {
            var dbItem = new WealthItem
            {
                Id = item.Id,
                Category = item.Category.ToString(),
                Count = item.Count,
                Icon = item.Icon,
                League = item.League,
                ItemLevel = item.ItemLevel,
                GemLevel = item.GemLevel,
                MapTier = item.MapTier,
                MaxLinks = item.MaxLinks,
                Name = item.Name,
                StashId = item.Stash,
                Price = await GetItemPrice(item, item.Category),
            };

            dbItem.Total = dbItem.Count * dbItem.Price;

            return dbItem;
        }

        private async Task<decimal> GetItemPrice(StashItem item, Category category)
        {
            var price = await poeNinjaClient.GetPriceInfo(
                item.Name,
                item.Name,
                category,
                item.GemLevel,
                item.MapTier,
                null,
                item.MaxLinks
            );

            if (price != null)
            {
                return price.Price;
            }

            logger.LogError($"{nameof(WealthParser)}.{nameof(GetItemPrice)}() : Could not price: {item.Name}.");
            return 0;
        }

        private async Task TakeStashSnapshot(SidekickDbContext database, StashTabDetails stash)
        {
            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            if (leagueId == null)
            {
                logger.LogError($"{nameof(WealthParser)}.{nameof(TakeStashSnapshot)}() : The league id is not set.");
                return;
            }

            var totalPrice = await database.WealthItems
                .Where(x => x.League == leagueId)
                .Where(x => x.StashId == stash.Id)
                .SumAsync(x => x.Total);

            database.WealthStashSnapshots.Add(new WealthStashSnapshot()
            {
                Date = DateTimeOffset.Now,
                League = leagueId,
                StashId = stash.Id,
                Total = totalPrice,
            });

            await database.SaveChangesAsync();
            OnSnapshotTaken?.Invoke();
        }

        private async Task TakeFullSnapshot(SidekickDbContext database)
        {
            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            if (leagueId == null)
            {
                logger.LogError($"{nameof(WealthParser)}.{nameof(TakeFullSnapshot)}() : The league id is not set.");
                return;
            }

            var totalPrice = await database.WealthItems
                .Where(x => x.League == leagueId)
                .SumAsync(x => x.Total);

            database.WealthFullSnapshots.Add(new WealthFullSnapshot()
            {
                Date = DateTimeOffset.Now,
                League = leagueId,
                Total = totalPrice,
            });

            await database.SaveChangesAsync();
            Log("Icons.Material.Filled.PhotoCamera", "Color.Success", $"Snapshot Taken.");
            OnSnapshotTaken?.Invoke();

            var oneHourAgo = DateTimeOffset.Now.AddHours(-1);
            var thirtyMinutesAgo = DateTimeOffset.Now.AddMinutes(-30);
            var oneHourAgoSnapshot = await database.WealthFullSnapshots
                .Where(x => x.Date > oneHourAgo)
                .Where(x => x.Date < thirtyMinutesAgo)
                .OrderBy(x => x.Date)
                .FirstOrDefaultAsync();

            if (oneHourAgoSnapshot?.Total != null && Math.Abs(oneHourAgoSnapshot.Total - totalPrice) < (decimal)0.001)
            {
                Log("Icons.Material.Filled.Warning", "Color.Warning", $"Wealth tracker was automatically stopped due to inactivity.");
                Stop();
            }
        }
    }
}
