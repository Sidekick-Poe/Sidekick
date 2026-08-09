using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Game.ItemDefinitions;
using Sidekick.Game.Items;
namespace Sidekick.Apis.PoeNinja.Stash;

public interface INinjaStashProvider
{
    List<NinjaStashDefinition> GetDefinitions(Item item);
    List<NinjaStashDefinition> GetDefinitions(TradeItemDefinition item, ApiItem apiItem);
    Task<List<NinjaStash>> GetInfo(Item item);
    Task<List<NinjaStash>> GetInfo(TradeItemDefinition item, ApiItem apiItem);
}
