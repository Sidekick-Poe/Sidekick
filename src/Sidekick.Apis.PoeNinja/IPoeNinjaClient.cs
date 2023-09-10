using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.PoeNinja
{
    public interface IPoeNinjaClient
    {
        Task<NinjaPrice?> GetPriceInfo(OriginalItem originalItem, Item item);

        Uri GetDetailsUri(NinjaPrice price);
    }
}
