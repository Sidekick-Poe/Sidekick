using Sidekick.Apis.PoePriceInfo.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.PoePriceInfo;

public interface IPoePriceInfoClient
{
    /// <summary>
    /// Predict the price of an item
    /// </summary>
    /// <param name="item">The item to price predict</param>
    Task<PricePrediction?> GetPricePrediction(Item item);
}
