using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Stash;
using Sidekick.Apis.Poe.Stash.Models;
using Sidekick.Apis.PoeNinja;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;
using Sidekick.Modules.Wealth.Models;

namespace Sidekick.Modules.Wealth
{
    internal class WealthParser
    {
        private readonly DbContextOptions<WealthDbContext> dbContextOptions;
        private readonly ISettings settings;
        private readonly IStashService stashService;
        private readonly IPoeNinjaClient poeNinjaClient;
        private readonly ILogger<WealthParser> logger;

        public event Action? OnLogsChanged;

        public event Action? OnStashParsed;

        public event Action? OnSnapshotTaken;

        public WealthParser(
            DbContextOptions<WealthDbContext> dbContextOptions,
            ISettings settings,
            IStashService stashService,
            IPoeNinjaClient poeNinjaClient,
            ILogger<WealthParser> logger)
        {
            this.dbContextOptions = dbContextOptions;
            this.settings = settings;
            this.stashService = stashService;
            this.poeNinjaClient = poeNinjaClient;
            this.logger = logger;
        }

        public Queue<(Guid Id, DateTimeOffset Date, string Icon, Color Color, string Message)> Logs { get; set; } = new();

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
            Log(Icons.Material.Filled.PlayCircle, Color.Success, $"Tracker Started.");
        }

        public void Stop()
        {
            if (CancellationTokenSource == null || !IsRunning())
            {
                return;
            }

            CancellationTokenSource.Cancel();
            Log(Icons.Material.Filled.StopCircle, Color.Warning, $"Tracker Stopped.");
        }

        public bool IsRunning()
        {
            return CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested;
        }

        private void Log(string icon, Color color, string message)
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

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    using var database = new WealthDbContext(dbContextOptions);

                    foreach (var id in settings.WealthTrackerTabs)
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

                        Log(Icons.Material.Filled.HourglassTop, Color.Info, $"[{stash.Name}] Updating...");
                        await ParseStash(database, stash);
                        await TakeStashSnapshot(database, stash);
                        Log(Icons.Material.Filled.HourglassBottom, Color.Info, $"[{stash.Name}] Updated.");
                    }

                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    await TakeFullSnapshot(database);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                catch (PoeApiException)
                {
                    Log(Icons.Material.Filled.Error, Color.Error, $"Exception! Something wrong happened while working on the tracker. Are you authenticated?");
                    Stop();
                }
            }
        }

        private async Task ParseStash(WealthDbContext database, StashTabDetails stash)
        {
            var dbStash = database.Stashes.FirstOrDefault(x => x.Id == stash.Id);
            if (dbStash == null)
            {
                dbStash = new Models.Stash()
                {
                    Id = stash.Id,
                    Name = stash.Name,
                    Parent = stash.Parent,
                    League = stash.League,
                    Type = stash.Type,
                    Total = 0,
                    LastUpdate = DateTimeOffset.Now,
                };
                database.Stashes.Add(dbStash);
            }
            else
            {
                dbStash.Name = stash.Name;
                dbStash.Parent = stash.Parent;
                dbStash.League = stash.League;
                dbStash.Type = stash.Type;
                dbStash.Total = 0;
                dbStash.LastUpdate = DateTimeOffset.Now;
            }

            // Game Item Removed (Traded, Used, Destroyed, etc.)
            var dbItems = database.Items.Where(x => x.StashId == stash.Id);
            database.Items.RemoveRange(dbItems);
            await database.SaveChangesAsync();

            // Add / Update Items
            var items = new List<Models.Item>();
            foreach (var item in stash.Items)
            {
                items.Add(await ParseItem(item));
            }

            dbStash.Total = items.Sum(x => x.Total);
            database.Items.AddRange(items);
            await database.SaveChangesAsync();

            OnStashParsed?.Invoke();
        }

        private async Task<Models.Item> ParseItem(StashItem item)
        {
            var dbItem = new Models.Item()
            {
                Id = item.Id,
                Category = item.Category,
                Count = item.Count,
                Icon = item.Icon,
                League = item.League,
                ItemLevel = item.ItemLevel,
                GemLevel = item.GemLevel,
                MapTier = item.MapTier,
                MaxLinks = item.MaxLinks,
                Name = item.Name,
                StashId = item.Stash,
            };

            dbItem.Price = await GetItemPrice(item, dbItem.Category);
            dbItem.Total = dbItem.Count * dbItem.Price;

            return dbItem;
        }

        private async Task<double> GetItemPrice(StashItem item, Category category)
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

            if (price == null)
            {
                logger.LogError($"[Wealth] Could not price: {item.Name}.");
            }

            return price?.Price ?? 0;
        }

        private async Task TakeStashSnapshot(WealthDbContext database, StashTabDetails stash)
        {
            var totalPrice = await database.Items
                .Where(x => x.League == settings.LeagueId)
                .Where(x => x.StashId == stash.Id)
                .SumAsync(x => x.Total);

            database.StashSnapshots.Add(new StashSnapshot()
            {
                Date = DateTimeOffset.Now,
                League = settings.LeagueId,
                StashId = stash.Id,
                Total = totalPrice,
            });

            await database.SaveChangesAsync();
            OnSnapshotTaken?.Invoke();
        }

        private async Task TakeFullSnapshot(WealthDbContext database)
        {
            var totalPrice = await database.Items
                .Where(x => x.League == settings.LeagueId)
                .SumAsync(x => x.Total);

            database.FullSnapshots.Add(new FullSnapshot()
            {
                Date = DateTimeOffset.Now,
                League = settings.LeagueId,
                Total = totalPrice,
            });

            await database.SaveChangesAsync();
            Log(Icons.Material.Filled.PhotoCamera, Color.Success, $"Snapshot Taken.");
            OnSnapshotTaken?.Invoke();

            var oneHourAgo = DateTimeOffset.Now.AddHours(-1);
            var thirtyMinutesAgo = DateTimeOffset.Now.AddMinutes(-30);
            var oneHourAgoSnapshot = await database.FullSnapshots
                .Where(x => x.Date > oneHourAgo)
                .Where(x => x.Date < thirtyMinutesAgo)
                .OrderBy(x => x.Date)
                .FirstOrDefaultAsync();

            if (oneHourAgoSnapshot?.Total == totalPrice)
            {
                Log(Icons.Material.Filled.Warning, Color.Warning, $"Wealth tracker was automatically stopped due to inactivity.");
                Stop();
            }
        }
    }
}
