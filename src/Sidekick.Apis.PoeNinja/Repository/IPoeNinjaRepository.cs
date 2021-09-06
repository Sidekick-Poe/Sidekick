using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Apis.PoeNinja.Api.Models;
using Sidekick.Apis.PoeNinja.Models;

namespace Sidekick.Apis.PoeNinja.Repository
{
    public interface IPoeNinjaRepository
    {
        Task<List<NinjaPrice>> Load(ItemType itemType);

        Task<List<NinjaTranslation>> LoadTranslations(ItemType itemType);

        Task SavePrices(ItemType itemType, List<NinjaPrice> prices);

        Task SaveTranslations(ItemType itemType, List<NinjaTranslation> translations);

        Task Clear();
    }
}
