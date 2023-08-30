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

        public async Task<List<NinjaPrice>> Load(ItemType itemType)
        {
            return (await cacheProvider.Get<List<NinjaPrice>>(GetCacheKey(itemType))) ?? new List<NinjaPrice>();
        }

        public Task SavePrices(ItemType itemType, List<NinjaPrice> prices)
        {
            prices = prices
                .GroupBy(x => (x.Name,
                               x.Corrupted,
                               x.MapTier,
                               x.GemLevel,
                               x.Links))
                .Select(x => x.OrderBy(x => x.Price).First())
                .ToList();
            return cacheProvider.Set(GetCacheKey(itemType), prices);
        }

        public async Task Clear()
        {
            foreach (var value in Enum.GetValues<ItemType>())
            {
                cacheProvider.Delete(GetCacheKey(value));
            }

            await settingsService.Save(nameof(ISettings.PoeNinja_LastClear), DateTimeOffset.Now);
        }

        private string GetCacheKey(ItemType itemType) => $"PoeNinja_{itemType}";
    }
}
