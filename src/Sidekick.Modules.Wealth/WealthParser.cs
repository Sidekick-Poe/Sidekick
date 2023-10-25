using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Markup;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Apis.Poe.Metadatas;
using Sidekick.Apis.Poe.Stash;
using Sidekick.Apis.Poe.Stash.Models;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Settings;
using Sidekick.Modules.Wealth.Models;
using static MudBlazor.CategoryTypes;
using Timer = System.Timers.Timer;

namespace Sidekick.Modules.Wealth
{


    internal class WealthParser : IDisposable
    {

        private bool Running = false;
        private Thread ParsingThread { get; set; }

        private WealthDbContext Database { get; set; }
        private DbContextOptions<WealthDbContext> Options { get; set; }
        private IAuthenticationService AuthenticationService { get; set; }
        private readonly ISettings Settings;
        private IStashService StashService { get; set; }
        private IPoeNinjaClient PoeNinjaClient { get; set; }
        private IItemMetadataProvider ItemMetadataProvider { get; set; }
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
            IItemMetadataProvider _itemMetadataProvider,
            ILogger<WealthParser> _logger)
        {
            AuthenticationService = _authenticationService;
            Database = new WealthDbContext(_options);
            Options = _options;
            Settings = _settings;
            StashService = _stashService;
            PoeNinjaClient = _poeNinjaClient;
            ItemMetadataProvider = _itemMetadataProvider;
            Logger = _logger;

            InterprocessService.OnMessage += InterprocessService_CustomProtocolCallback;
        }

        public async Task Start()
        {

            if (!Running)
            {
                if(!AuthenticationService.IsAuthenticated()) {
                    await AuthenticationService.Authenticate();
                }

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
                if (AuthenticationService.IsAuthenticated())
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

                } else if (!AuthenticationService.IsAuthenticating()) {
                    Running = false;
                    OnParserStopped?.Invoke(new string[] { });
                }

                await Task.Delay(TimeSpan.FromSeconds(1));

            }
        }

        private async Task<Models.Stash> ParseStash(APIStashTab stash)
        {

            OnStashParsing?.Invoke(new string[] { stash.id, stash.name });

            var dbStash = Database.Stashes.FirstOrDefault(x => x.Id == stash.id);

            if (dbStash == null) {
                dbStash = new Models.Stash();
                dbStash.Id = stash.id;
                dbStash.Name = stash.name ?? "";
                dbStash.Type = stash.type ?? "";
                dbStash.Total = 0;
                dbStash.CreatedOn = DateTime.Now;
                dbStash.UpdatedOn = dbStash.CreatedOn;

                Database.Stashes.Add(dbStash);
            } else {          
                dbStash.Total = 0; 
            }

            dbStash.UpdatedOn = DateTime.Now;

            List<APIStashItem> items;
            if (stash.type != null && stash.type.ToUpper() == "MAPSTASH") {
                items = await StashService.GetMapStashItems(stash);
            } else {
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

        private async Task<Models.Item> ParseItem(APIStashItem item, APIStashTab stash)
        {
            var dbItem = Database.Items.FirstOrDefault(x => x.Id == item.id);

            if (dbItem == null) {
                dbItem = new Models.Item();
                dbItem.Id = item.id;
                dbItem.Name = item.getFriendlyName();
                dbItem.Stash = stash.id;
                dbItem.Icon = item.icon;
                dbItem.League = item.league;
                dbItem.Level = item.ilvl;
                dbItem.Type = GetItemType(item);
                dbItem.CreatedOn = DateTime.Now;

                Database.Items.Add(dbItem);
            } else {
                dbItem.Removed = false; // Removed a full stack, added it back
            }

            dbItem.Count = GetItemCount(item);
            dbItem.Price = item.getFriendlyName().ToUpper() == "CHAOS ORB" ? 1 : await GetItemPrice(item);
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

        private ItemType GetItemType(APIStashItem item)
        {

            switch (item.frameType)
            {
                case FrameType.Currency:
                    return ItemType.Currency;

                case FrameType.Normal:
                case FrameType.Magic:
                case FrameType.Rare:
                    if (item.name.ToUpper().EndsWith("MAP")) {
                        return ItemType.Map;
                    }
                    return ItemType.Other;
                case FrameType.Unique:
                    return ItemType.Unique;
                case FrameType.DivinationCard:
                    return ItemType.Unique;
            }

            return ItemType.Other;
        }

        private async Task<double> GetItemPrice(APIStashItem item)
        {
            var name = item.getFriendlyName(false);
            var metadata = ItemMetadataProvider.Parse(name, item.typeLine);
            if(metadata == null)
            {
                Logger.LogError($"Could not retrieve metadeta: {item.getFriendlyName()}");
                return 0;
            }

            var category = metadata.Category;
            var price = await PoeNinjaClient.GetPriceInfo(
                name,
                item.typeLine,
                category,
                null,
                item.getMapTier(),
                null,
                item.getLinkCount());

            if(price == null)
            {
                Logger.LogError($"Could not price: {item.getFriendlyName()}");
            }

            return price?.Price ?? 0;
        }

        private int GetItemCount(APIStashItem item)
        {
            if(item.stackSize == null)
            {
                return 1;
            }
            return (int)item.stackSize;
        }

        private bool CanParse(APIStashItem item)
        {
            var name = item.getFriendlyName().ToUpper();

            switch(item.frameType)
            {
                case FrameType.Currency:

                    string[] exceptions = {
                        "SHARD", "RITUAL SPLINTER", "FRAGMENT"};

                    if(exceptions.Any(name.Contains)) {
                        return false;
                    }
                    return true;

                case FrameType.Normal:
                case FrameType.Magic:
                case FrameType.Rare:
                    string[] allowed = {
                        "DIVINE VESSEL", "OFFERING OF THE GODDESS", "SCARAB", "MAP" };

                    if (allowed.Any(name.Contains)) {
                        return true;
                    }
                    return false;
                case FrameType.Unique:
                case FrameType.DivinationCard:
                    return true;
            }
            return false;
        }

        public void InterprocessService_CustomProtocolCallback(string[] obj)
        {
            if (obj.Length > 0 && obj[0].ToUpper().StartsWith("SIDEKICK://OAUTH/POE"))
            {
                var queryDictionary = System.Web.HttpUtility.ParseQueryString(new System.Uri(obj[0]).Query);

                AuthenticationService.AuthenticationCallback(
                    queryDictionary["code"],
                    queryDictionary["state"]
                );
            }
        }

        public void Dispose()
        {
            InterprocessService.OnMessage -= InterprocessService_CustomProtocolCallback;
            Running = false;
        }
    }
}
