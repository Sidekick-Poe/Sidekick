using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sidekick.Apis.PoeNinja.Api.Models;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoeNinja.Repository
{
    public class PoeNinjaRepository : IPoeNinjaRepository
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly ISettingsService settingsService;

        public PoeNinjaRepository(
            ICacheProvider cacheProvider,
            IGameLanguageProvider gameLanguageProvider,
            ISettingsService settingsService)
        {
            this.cacheProvider = cacheProvider;
            this.gameLanguageProvider = gameLanguageProvider;
            this.settingsService = settingsService;
        }

        public Task<List<NinjaPrice>> Load(ItemType itemType)
        {
            return cacheProvider.Get<List<NinjaPrice>>(GetCacheKey(itemType));
        }

        public Task<List<NinjaPrice>> Load(CurrencyType currencyType)
        {
            return cacheProvider.Get<List<NinjaPrice>>(GetCacheKey(currencyType));
        }

        public Task<List<NinjaTranslation>> LoadTranslations(ItemType itemType)
        {
            return cacheProvider.Get<List<NinjaTranslation>>(GetTranslationCacheKey(itemType));
        }

        public Task<List<NinjaTranslation>> LoadTranslations(CurrencyType currencyType)
        {
            return cacheProvider.Get<List<NinjaTranslation>>(GetTranslationCacheKey(currencyType));
        }

        public Task SavePrices(ItemType itemType, List<NinjaPrice> prices)
        {
            prices = prices
                .GroupBy(x => (x.Name, x.Corrupted, x.MapTier, x.GemLevel))
                .Select(x => x.OrderBy(x => x.Price).First())
                .ToList();
            return cacheProvider.Set(GetCacheKey(itemType), prices);
        }

        public Task SavePrices(CurrencyType currencyType, List<NinjaPrice> prices)
        {
            prices = prices
                   .GroupBy(x => (x.Name, x.Corrupted, x.MapTier, x.GemLevel))
                   .Select(x => x.OrderBy(x => x.Price).First())
                   .ToList();
            return cacheProvider.Set(GetCacheKey(currencyType), prices);
        }

        public Task SaveTranslations(ItemType itemType, List<NinjaTranslation> translations)
        {
            translations = translations
                .GroupBy(x => x.English)
                .Select(x => x.First())
                .ToList();

            return cacheProvider.Set(GetTranslationCacheKey(itemType), translations);
        }

        public Task SaveTranslations(CurrencyType currencyType, List<NinjaTranslation> translations)
        {
            translations = translations
                .GroupBy(x => x.English)
                .Select(x => x.First())
                .ToList();

            return cacheProvider.Set(GetTranslationCacheKey(currencyType), translations);
        }

        public async Task Clear()
        {
            foreach (var value in Enum.GetValues<ItemType>())
            {
                cacheProvider.Delete(GetCacheKey(value));
                cacheProvider.Delete(GetTranslationCacheKey(value));
            }

            foreach (var value in Enum.GetValues<CurrencyType>())
            {
                cacheProvider.Delete(GetCacheKey(value));
                cacheProvider.Delete(GetTranslationCacheKey(value));
            }

            await settingsService.Save(nameof(ISettings.PoeNinja_LastClear), DateTimeOffset.Now);
        }

        private string GetCacheKey(ItemType itemType) => $"PoeNinja_{gameLanguageProvider.Language.LanguageCode}_{itemType}";
        private string GetCacheKey(CurrencyType currencyType) => $"PoeNinja_{gameLanguageProvider.Language.LanguageCode}_{currencyType}";
        private string GetTranslationCacheKey(ItemType itemType) => $"PoeNinja_{gameLanguageProvider.Language.LanguageCode}_{itemType}_Translation";
        private string GetTranslationCacheKey(CurrencyType currencyType) => $"PoeNinja_{gameLanguageProvider.Language.LanguageCode}_{currencyType}_Translation";
    }
}
