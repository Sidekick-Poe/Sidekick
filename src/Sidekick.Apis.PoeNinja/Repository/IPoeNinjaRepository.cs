using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Apis.PoeNinja.Api.Models;
using Sidekick.Apis.PoeNinja.Models;

namespace Sidekick.Apis.PoeNinja.Repository
{
    public interface IPoeNinjaRepository
    {
        Task<List<NinjaPrice>> Load(ItemType itemType);
        Task<List<NinjaPrice>> Load(CurrencyType currencyType);

        Task<List<NinjaTranslation>> LoadTranslations(ItemType itemType);
        Task<List<NinjaTranslation>> LoadTranslations(CurrencyType currencyType);

        Task SavePrices(ItemType itemType, List<NinjaPrice> prices);
        Task SavePrices(CurrencyType currencyType, List<NinjaPrice> prices);

        Task SaveTranslations(ItemType itemType, List<NinjaTranslation> translations);
        Task SaveTranslations(CurrencyType currencyType, List<NinjaTranslation> translations);

        Task Clear();
    }
}
