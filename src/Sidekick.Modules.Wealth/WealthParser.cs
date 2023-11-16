using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Apis.Poe.Stash;
using Sidekick.Apis.Poe.Stash.Models;
using Sidekick.Apis.PoeNinja;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Settings;
using Sidekick.Modules.Wealth.Models;

namespace Sidekick.Modules.Wealth
{
    internal class WealthParser
    {
        private bool Running = false;
        private Thread ParsingThread { get; set; }

        private WealthDbContext Database { get; set; }
        private DbContextOptions<WealthDbContext> Options { get; set; }
        private readonly ISettings Settings;
        private readonly IItemMetadataParser itemMetadataParser;

        private IStashService StashService { get; set; }
        private IPoeNinjaClient PoeNinjaClient { get; set; }
        private ILogger<WealthParser> Logger { get; set; }

        public static event Action<string[]> OnStashParsing;

        public static event Action<string[]> OnStashParsed;

        public static event Action<string[]> OnSnapshotTaken;

        public static event Action<string[]> OnParserStopped;

        public WealthParser(
            IAuthenticationService _authenticationService,
            DbContextOptions<WealthDbContext> _options,
            ISettings _settings,
            IStashService _stashService,
            IPoeNinjaClient _poeNinjaClient,
            IItemMetadataParser itemMetadataParser,
            ILogger<WealthParser> _logger,
            IInterprocessService interprocessService)
        {
            Database = new WealthDbContext(_options);
            Options = _options;
            Settings = _settings;
            StashService = _stashService;
            PoeNinjaClient = _poeNinjaClient;
            this.itemMetadataParser = itemMetadataParser;
            Logger = _logger;
        }

        public async Task Start()
        {
            if (!Running)
            {
                Running = true;
                ParsingThread = new Thread(ParseLoop);
                ParsingThread.Start();
            }
        }

        public void Stop()
        {
            if (Running)
            {
                Running = false;
            }
        }

        public bool IsRunning()
        {
            return Running;
        }

        private async void ParseLoop()
        {
            while (Running)
            {
                Database = new WealthDbContext(Options);

                foreach (var id in Settings.WealthTrackerTabs)
                {
                    await ParseStash(await StashService.GetStashTab(id));

                    Database.SaveChanges();

                    if (!Running) { break; }
                }

                TakeSnapshot();

                Database.SaveChanges();
                Database.Dispose();

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task<Models.Stash> ParseStash(ApiStashTab stash)
        {
            OnStashParsing?.Invoke(new string[] { stash.id, stash.name });

            var dbStash = Database.Stashes.FirstOrDefault(x => x.Id == stash.id);

            if (dbStash == null)
            {
                dbStash = new Models.Stash();
                dbStash.Id = stash.id;
                dbStash.Name = stash.name ?? "";
                dbStash.Type = stash.type ?? "";
                dbStash.Total = 0;
                dbStash.CreatedOn = DateTime.Now;
                dbStash.UpdatedOn = dbStash.CreatedOn;

                Database.Stashes.Add(dbStash);
            }
            else
            {
                dbStash.Total = 0;
            }

            dbStash.UpdatedOn = DateTime.Now;

            List<APIStashItem> items;
            if (stash.type != null && stash.type.ToUpper() == "MAPSTASH")
            {
                items = await StashService.GetMapStashItems(stash);
            }
            else
            {
                items = await StashService.GetStashItems(stash);
            }

            // Game Item Removed (Traded, Used, Destroyed, etc.)
            var itemList = Database.Items.Where(x => x.Stash == stash.id);
            foreach (var dbItem in itemList)
            {
                if (items.FirstOrDefault(x => x.id == dbItem.Id) == null)
                {
                    dbItem.Removed = true;
                    dbItem.UpdatedOn = DateTime.Now;
                }
            }

            // Add / Update Items
            foreach (var item in items)
            {
                if (CanParse(item))
                {
                    var dbItem = await ParseItem(item, stash);
                    dbStash.Total += dbItem.Total;
                }
            }

            OnStashParsed?.Invoke(new string[] { stash.id, stash.name ?? "" });

            return dbStash;
        }

        private async Task<Models.Item> ParseItem(APIStashItem item, ApiStashTab stash)
        {
            var dbItem = Database.Items.FirstOrDefault(x => x.Id == item.id);

            if (dbItem == null)
            {
                dbItem = new Models.Item();
                dbItem.Id = item.id;
                dbItem.Name = item.getFriendlyName();
                dbItem.Stash = stash.id;
                dbItem.Icon = item.icon;
                dbItem.League = item.league;
                dbItem.Level = item.ilvl;
                dbItem.Category = GetItemCategory(item);
                dbItem.CreatedOn = DateTime.Now;

                Database.Items.Add(dbItem);
            }
            else
            {
                dbItem.Removed = false; // Removed a full stack, added it back
            }

            dbItem.Count = GetItemCount(item);
            dbItem.Price = item.getFriendlyName().ToUpper() == "CHAOS ORB" ? 1 : await GetItemPrice(item, dbItem.Category);
            dbItem.Total = dbItem.Count * dbItem.Price;
            dbItem.UpdatedOn = DateTime.Now;

            return dbItem;
        }

        private void TakeSnapshot()
        {
            var BatchId = 0;

            var Snapshots = Database.Snapshots.ToList();

            if (Snapshots.Count() > 0)
            {
                BatchId = Snapshots.Select(x => x.BatchId).Max() + 1;
            }

            var stashes = Database.Stashes.ToList();
            var total = 0.0;

            foreach (var stash in stashes)
            {
                Database.Snapshots.Add(new Snapshot
                {
                    BatchId = BatchId,
                    StashId = stash.Id,
                    Total = stash.Total,
                    CreatedOn = DateTime.Now
                });

                total += stash.Total;
            }

            Database.Snapshots.Add(new Snapshot
            {
                BatchId = BatchId,
                StashId = "SUMMARY",
                Total = total,
                CreatedOn = DateTime.Now
            });

            OnSnapshotTaken?.Invoke(new string[] { BatchId.ToString() });
        }

        private Category GetItemCategory(APIStashItem item)
        {
            var name = item.getFriendlyName(false);
            var metadata = itemMetadataParser.Parse(name, item.typeLine);
            if (metadata == null)
            {
                Logger.LogError($"Could not retrieve metadeta: {item.getFriendlyName(false)}");
                return 0;
            }
            return metadata.Category;
        }

        private async Task<double> GetItemPrice(APIStashItem item, Category category)
        {
            var name = item.getFriendlyName(false);
            var price = await PoeNinjaClient.GetPriceInfo(
            name,
            item.typeLine,
            category,
            item.getGemLevel(),
            item.getMapTier(),
            null,
            item.getLinkCount());
            if (price == null)
            {
                Logger.LogError($"Could not price: {item.getFriendlyName()}");
            }
            return price?.Price ?? 0;
        }

        private int GetItemCount(APIStashItem item)
        {
            if (item.stackSize == null)
            {
                return 1;
            }
            return (int)item.stackSize;
        }

        private bool CanParse(APIStashItem item)
        {
            var category = GetItemCategory(item);

            if (category != Category.Unknown)
            {
                switch (item.frameType)
                {
                    case FrameType.Currency:
                        if (category == Category.Currency)
                        {
                            return true;
                        }
                        return false;

                    case FrameType.Normal:
                    case FrameType.Magic:
                    case FrameType.Rare:
                        Category[] valid = { Category.Currency, Category.Map };
                        if (valid.Contains(category))
                        {
                            return true;
                        }
                        return false;

                    case FrameType.Unique:
                    case FrameType.DivinationCard:
                    case FrameType.Gem:
                        return true;
                }
            }
            return false;
        }
    }
}
