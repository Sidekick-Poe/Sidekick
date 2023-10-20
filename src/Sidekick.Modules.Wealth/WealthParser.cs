using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Apis.Poe.Metadatas;
using Sidekick.Apis.Poe.Stash;
using Sidekick.Apis.Poe.Stash.Models;
using Sidekick.Apis.PoeNinja;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Settings;
using Sidekick.Modules.Wealth.Models;
using static MudBlazor.CategoryTypes;

namespace Sidekick.Modules.Wealth
{


    internal class WealthParser : IDisposable
    {

        private bool Running = false;
        private Thread ParsingThread { get; set; }

        private WealthDbContext Database { get; set; }
        private IAuthenticationService AuthenticationService { get; set; }
        private readonly ISettings Settings;
        private IStashService StashService { get; set; }
        private IPoeNinjaClient PoeNinjaClient { get; set; }
        private IItemMetadataProvider ItemMetadataProvider { get; set; }

        public static event Action<string[]> OnStashParsing;
        public static event Action<string[]> OnStashParsed;
        public static event Action<string[]> OnItemParsing;
        public static event Action<string[]> OnItemParsed;

        public WealthParser(
            IAuthenticationService _authenticationService,
            WealthDbContext _database,
            ISettings _settings,
            IStashService _stashService,
            IPoeNinjaClient _poeNinjaClient,
            IItemMetadataProvider _itemMetadataProvider)
        {
            AuthenticationService = _authenticationService;
            Database = _database;
            Settings = _settings;
            StashService = _stashService;
            PoeNinjaClient = _poeNinjaClient;
            ItemMetadataProvider = _itemMetadataProvider;


            InterprocessService.OnMessage += InterprocessService_CustomProtocolCallback;
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
                if (!AuthenticationService.IsAuthenticated() && !AuthenticationService.IsAuthenticating())
                {
                    await AuthenticationService.Authenticate();
                }
                else
                {
                    foreach (var id in Settings.WealthTrackerTabs)
                    {
                        await ParseStash(await StashService.GetStashTab(id));

                        if (!Running) { break; }

                    }
                }
            }
        }

        private async Task<Models.Stash> ParseStash(APIStashTab stash)
        {

            OnStashParsing?.Invoke(new string[] { stash.id, stash.name });

            var dbStash = Database.Stashes.FirstOrDefault(x => x.Id == stash.id);

            if (dbStash == null)
            {
                dbStash = new Models.Stash();
                dbStash.Id = stash.id;
                dbStash.Name = stash.name;
                dbStash.Type = stash.type;
                dbStash.Total = 0;
                dbStash.CreatedOn = DateTime.Now;
                dbStash.UpdatedOn = dbStash.CreatedOn;

                Database.Stashes.Add(dbStash);
            }
            else
            {
                dbStash.Total = 0;
                dbStash.UpdatedOn = DateTime.Now;
            }

            var items = await StashService.GetStashItems(stash);

            // Game Item Removed (Traded, Used, Destroyed, etc.)
            var itemList = Database.Items.Where(x => x.Stash == stash.id).ToList();
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

            Database.SaveChanges();

            OnStashParsed?.Invoke(new string[] { stash.id, stash.name });

            return dbStash;
        }

        private async Task<Models.Item> ParseItem(APIStashItem item, APIStashTab stash)
        {
            OnItemParsing?.Invoke(new string[] { item.id, item.name });

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
                dbItem.Type = GetItemType(item);
                dbItem.CreatedOn = DateTime.Now;

                Database.Items.Add(dbItem);
            } else {
                dbItem.Removed = false; // Removed a full stack, added it back
            }

            dbItem.Count = GetItemCount(item);
            dbItem.Price = await GetItemPrice(item);
            dbItem.Total = dbItem.Count * dbItem.Price;
            dbItem.UpdatedOn = DateTime.Now;

            Database.SaveChanges();

            OnItemParsed?.Invoke(new string[] { item.id, item.name });

            return dbItem;
        }

        private ItemType GetItemType(APIStashItem item)
        {
            // todo : any other item types
            return ItemType.Currency;
        }

        private async Task<double> GetItemPrice(APIStashItem item)
        {
            var name = item.getFriendlyName();
            var metadata = ItemMetadataProvider.Parse(name, item.typeLine);
            var category = metadata.Category;
            var price = await PoeNinjaClient.GetPriceInfo(name, item.typeLine, category);
 
            return price.Price;
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
            if(item.frameType == FrameType.Currency)
            {
                if(item.getFriendlyName().ToUpper().Contains("SHARD"))
                {
                    //Todo: Implement Shard price tracking... PoeNinjaClient doesnt support shards :(
                    return false;
                }
                if (item.getFriendlyName().ToUpper().Contains("RITUAL SPLINTER"))
                {
                    //Todo: Implement Ritual Splinter tracking... PoeNinjaClient doesnt support shards :(
                    return false;
                }
                if (item.getFriendlyName().ToUpper().Contains("FRAGMENT"))
                {
                    //Todo: Implement Fragment price tracking... PoeNinjaClient doesnt support fragments :(
                    return false;
                }
                if (item.getFriendlyName().ToUpper().Contains("CHAOS ORB"))
                {
                    //Todo: Chaos orb == Chaos orb, need to hard code this somewhere
                    return false;
                }
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
