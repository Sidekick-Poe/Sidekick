using Sidekick.Data.ItemDefinitions;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    NinjaExchangeDefinition? GetDefinition(ItemDefinition? item);
    Task<NinjaCurrency?> GetInfo(ItemDefinition item);
}
