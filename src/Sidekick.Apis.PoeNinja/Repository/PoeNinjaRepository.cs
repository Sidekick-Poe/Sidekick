using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sidekick.Apis.PoeNinja.Api.Models;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoeNinja.Repository
{
    public class PoeNinjaRepository : IPoeNinjaRepository
    {
        private readonly ICacheProvider cacheProvider;
        private readonly ISettingsService settingsService;

        public PoeNinjaRepository(
            ICacheProvider cacheProvider,
            ISettingsService settingsService)
        {
            this.cacheProvider = cacheProvider;
            this.settingsService = settingsService;
        }

        public Task<List<NinjaPrice>> Load(ItemType itemType)
        {
            return cacheProvider.Get<List<NinjaPrice>>(GetCacheKey(itemType));
        }

        public Task<List<NinjaTranslation>> LoadTranslations(ItemType itemType)
        {
            return cacheProvider.Get<List<NinjaTranslation>>(GetTranslationCacheKey(itemType));
        }

        public Task SavePrices(ItemType itemType, List<NinjaPrice> prices)
        {
            prices = prices
                .GroupBy(x => (x.Name, x.Corrupted, x.MapTier, x.GemLevel))
                .Select(x => x.OrderBy(x => x.Price).First())
                .ToList();
            return cacheProvider.Set(GetCacheKey(itemType), prices);
        }

        public Task SaveTranslations(ItemType itemType, List<NinjaTranslation> translations)
        {
            translations = translations
                .GroupBy(x => x.English)
                .Select(x => x.First())
                .ToList();

            return cacheProvider.Set(GetTranslationCacheKey(itemType), translations);
        }

        public async Task Clear()
        {
            foreach (var value in Enum.GetValues<ItemType>())
            {
                cacheProvider.Delete(GetCacheKey(value));
                cacheProvider.Delete(GetTranslationCacheKey(value));
            }

            await settingsService.Save(nameof(ISettings.PoeNinja_LastClear), DateTimeOffset.Now);
        }

        private string GetCacheKey(ItemType itemType) => $"PoeNinja_{itemType}";
        private string GetTranslationCacheKey(ItemType itemType) => $"PoeNinja_{itemType}_Translation";
    }
}
