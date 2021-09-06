using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.PoeNinja.Api;
using Sidekick.Apis.PoeNinja.Api.Models;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Apis.PoeNinja.Repository;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoeNinja
{
    /// <summary>
    /// https://poe.ninja/swagger
    /// </summary>
    public class PoeNinjaClient : IPoeNinjaClient
    {
        private readonly ISettings settings;
        private readonly IPoeNinjaRepository repository;
        private readonly IPoeNinjaApiClient poeNinjaApiClient;

        public PoeNinjaClient(
            ILogger<PoeNinjaClient> logger,
            IGameLanguageProvider gameLanguageProvider,
            ISettings settings,
            IPoeNinjaRepository repository,
            IPoeNinjaApiClient poeNinjaApiClient)
        {
            this.settings = settings;
            this.repository = repository;
            this.poeNinjaApiClient = poeNinjaApiClient;
        }

        public async Task Initialize()
        {
            if (!settings.PoeNinja_LastClear.HasValue || settings.PoeNinja_LastClear < DateTimeOffset.Now.AddHours(-8))
            {
                await repository.Clear();
            }
        }

        public async Task<NinjaPrice> GetPriceInfo(Item item)
        {
            var name = item.Original.Name;
            var type = item.Original.Type;

            foreach (var itemType in GetItemTypes(item))
            {
                var repositoryItems = await repository.Load(itemType);
                if (repositoryItems == null)
                {
                    repositoryItems = await poeNinjaApiClient.FetchPrices(itemType);
                }

                if (repositoryItems == null)
                {
                    continue;
                }

                var translations = await repository.LoadTranslations(itemType);
                if (translations.Any(x => x.Translation == name))
                {
                    name = translations.First(x => x.Translation == name).English;
                }

                var query = repositoryItems.AsQueryable().Where(x => x.Name == name || x.Name == type);

                if (item.Properties != null)
                {
                    query = query.Where(x => x.Corrupted == item.Properties.Corrupted && x.MapTier == item.Properties.MapTier && x.GemLevel == item.Properties.GemLevel);
                }

                if (query.Any())
                {
                    return query.FirstOrDefault();
                }
            }

            return null;
        }

        private List<ItemType> GetItemTypes(Item item)
        {
            var result = new List<ItemType>();

            if (item.Metadata.Category == Category.Currency)
            {
                result.Add(ItemType.Currency);
                result.Add(ItemType.Fragment);

                result.Add(ItemType.Incubator);
                result.Add(ItemType.Oil);
                result.Add(ItemType.Incubator);
                result.Add(ItemType.Scarab);
                result.Add(ItemType.Fossil);
                result.Add(ItemType.Resonator);
                result.Add(ItemType.Essence);
                result.Add(ItemType.Resonator);
            }
            else if (item.Metadata.Rarity == Rarity.Unique)
            {
                switch (item.Metadata.Category)
                {
                    case Category.Accessory: result.Add(ItemType.UniqueAccessory); break;
                    case Category.Armour: result.Add(ItemType.UniqueArmour); break;
                    case Category.Flask: result.Add(ItemType.UniqueFlask); break;
                    case Category.Jewel: result.Add(ItemType.UniqueJewel); break;
                    case Category.Map: result.Add(ItemType.UniqueMap); break;
                    case Category.Weapon: result.Add(ItemType.UniqueWeapon); break;
                    case Category.ItemisedMonster: result.Add(ItemType.Beast); break;
                }
            }
            else
            {
                switch (item.Metadata.Category)
                {
                    case Category.DivinationCard: result.Add(ItemType.DivinationCard); break;
                    case Category.Map: result.Add(ItemType.Map); break;
                    case Category.Gem: result.Add(ItemType.SkillGem); break;
                    case Category.Prophecy: result.Add(ItemType.Prophecy); break;
                    case Category.ItemisedMonster: result.Add(ItemType.Beast); break;
                }
            }

            return result;
        }
    }
}
