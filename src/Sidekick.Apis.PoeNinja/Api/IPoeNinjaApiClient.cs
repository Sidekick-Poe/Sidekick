using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Apis.PoeNinja.Api.Models;
using Sidekick.Apis.PoeNinja.Models;

namespace Sidekick.Apis.PoeNinja.Api
{
    public interface IPoeNinjaApiClient
    {
        Task<List<NinjaPrice>> FetchItems(ItemType itemType);
        Task<List<NinjaPrice>> FetchCurrencies(CurrencyType currencyType);
    }
}
