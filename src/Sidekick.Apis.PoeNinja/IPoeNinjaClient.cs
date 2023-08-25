using System;
using System.Threading.Tasks;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.PoeNinja
{
    public interface IPoeNinjaClient : IInitializableService
    {
        Task<NinjaPrice> GetPriceInfo(OriginalItem originalItem, Item item);

        Uri GetDetailsUri(NinjaPrice price);
    }
}
