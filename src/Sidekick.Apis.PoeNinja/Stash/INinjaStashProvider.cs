using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
namespace Sidekick.Apis.PoeNinja.Stash;

public interface INinjaStashProvider
{
    List<NinjaItemDefinition> GetDefinitions(Item item);
    List<NinjaItemDefinition> GetDefinitions(ItemDefinition item, ApiItem apiItem);
    Task<List<NinjaStash>> GetInfo(Item item);
    Task<List<NinjaStash>> GetInfo(ItemDefinition item, ApiItem apiItem);
}
