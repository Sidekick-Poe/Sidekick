using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
namespace Sidekick.Apis.PoeNinja.Stash;

public interface INinjaStashProvider
{
    List<NinjaStashDefinition> GetDefinitions(Item item);
    List<NinjaStashDefinition> GetDefinitions(ItemDefinition item, ApiItem apiItem);
    Task<List<NinjaStash>> GetInfo(Item item);
    Task<List<NinjaStash>> GetInfo(ItemDefinition item, ApiItem apiItem);
}
