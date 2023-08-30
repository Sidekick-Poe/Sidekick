using Sidekick.Apis.PoeNinja.Api.Models;
using Sidekick.Apis.PoeNinja.Models;

namespace Sidekick.Apis.PoeNinja.Api
{
    public interface IPoeNinjaApiClient
    {
        Task<List<NinjaPrice>> FetchPrices(ItemType itemType);
    }
}
