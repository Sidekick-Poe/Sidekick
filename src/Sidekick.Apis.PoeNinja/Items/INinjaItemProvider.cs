using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Items.Models;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.PoeNinja.Items;

public interface INinjaItemProvider : IInitializableService
{
    NinjaExchangeItem? GetExchangeItem(string? invariant);

    NinjaStashItem? GetStashItem(Item item);
    NinjaStashItem? GetUniqueItem(string? name, int links);
    NinjaStashItem? GetGemItem(string? name, int gemLevel, int gemQuality, bool corrupted);
    NinjaStashItem? GetMapItem(string? name, int mapTier);
    NinjaStashItem? GetClusterItem(string? grantText, int passiveCount, int itemLevel);
    NinjaStashItem? GetBaseTypeItem(string? name, int itemLevel, Influences influences);
}
