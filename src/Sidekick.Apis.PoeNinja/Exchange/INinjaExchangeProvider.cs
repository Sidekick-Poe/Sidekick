using Sidekick.Game.ItemDefinitions;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    Task<List<NinjaCurrency>> GetInfo(Item item);
    Task<List<NinjaCurrency>> GetInfo(ItemDefinition? itemDefinition);
}
