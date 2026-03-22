using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;
using Sidekick.Data.Items;
using Sidekick.Data.Ninja;
namespace Sidekick.Apis.PoeNinja.Items;

public interface INinjaItemProvider : IInitializableService
{
    // todo remove
    NinjaExchangeItem? GetExchangeItem(ItemDefinition item);

    NinjaStashItem? GetStashItem(Item item);
    NinjaStashItem? GetUniqueItem(ItemDefinition item, int links);
    NinjaStashItem? GetGemItem(ItemDefinition item, int gemLevel, int gemQuality, bool corrupted);
    NinjaStashItem? GetMapItem(ItemDefinition item, int mapTier);
    NinjaStashItem? GetClusterItem(string? grantText, int passiveCount, int itemLevel);
    NinjaStashItem? GetBaseTypeItem(ItemDefinition item, int itemLevel, Influences influences);
}
