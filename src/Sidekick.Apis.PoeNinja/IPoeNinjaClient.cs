using System;
using System.Threading.Tasks;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.PoeNinja
{
    public interface IPoeNinjaClient
    {
        Task Initialize();

        Task<NinjaPrice> GetPriceInfo(OriginalItem originalItem, Item item);

        Uri GetDetailsUri(NinjaPrice price);
    }
}
