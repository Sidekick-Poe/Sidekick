using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Game;
using Sidekick.Game.ItemDefinitions;
using Sidekick.Game.Ninja;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Apis.PoeNinja.Stash;

public interface INinjaStashProvider
{
    List<NinjaStashItem> GetDefinitions(Item item);
    List<NinjaStashItem> GetDefinitions(GameType game, ItemDefinition item, ApiItem apiItem);
    Task<List<NinjaStash>> GetInfo(Item item);
    Task<List<NinjaStash>> GetInfo(GameType game, ItemDefinition item, ApiItem apiItem);
}
