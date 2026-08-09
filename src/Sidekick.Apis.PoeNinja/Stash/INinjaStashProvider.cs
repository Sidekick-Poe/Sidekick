using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Game.ItemDefinitions;
using Sidekick.Game.Items;
namespace Sidekick.Apis.PoeNinja.Stash;

public interface INinjaStashProvider
{
    List<NinjaStashItem> GetDefinitions(Item item);
    List<NinjaStashItem> GetDefinitions(ItemDefinition item, ApiItem apiItem);
    Task<List<NinjaStash>> GetInfo(Item item);
    Task<List<NinjaStash>> GetInfo(ItemDefinition item, ApiItem apiItem);
}
