using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Stash;
using Sidekick.Apis.Poe.Stash.Models;
using Sidekick.Apis.Poe.Trade.Results;
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

        public event Action<Severity, string>? OnLogEvent;

        public event Action<StashTabDetails>? OnStashParsed;

        public event Action<StashTabDetails?>? OnSnapshotTaken;

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
            OnLogEvent?.Invoke(Severity.Info, $"Tracker Started.");
        }

        public void Stop()
        {
            if (CancellationTokenSource == null || !IsRunning())
            {
                return;
            }

            CancellationTokenSource.Cancel();
            OnLogEvent?.Invoke(Severity.Info, $"Tracker Stopped.");
        }

        public bool IsRunning()
        {
            return CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested;
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

                        await ParseStash(database, stash);
                        await TakeStashSnapshot(database, stash);
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
                    OnLogEvent?.Invoke(Severity.Error, $"Exception! Something wrong happened while working on the tracker. Are you authenticated?");
                    Stop();
                }
            }
        }

        private async Task ParseStash(WealthDbContext database, StashTabDetails stash)
        {
            OnLogEvent?.Invoke(Severity.Normal, $"[{stash.Name}] Started Parsing.");

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

            OnLogEvent?.Invoke(Severity.Normal, $"[{stash.Name}] Completed Parsing.");
            OnStashParsed?.Invoke(stash);
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
            OnLogEvent?.Invoke(Severity.Info, $"[{stash.Name}] Snapshot Taken.");
            OnSnapshotTaken?.Invoke(stash);
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
            OnLogEvent?.Invoke(Severity.Info, $"Full Snapshot Taken.");
            OnSnapshotTaken?.Invoke(null);
        }
    }
}
