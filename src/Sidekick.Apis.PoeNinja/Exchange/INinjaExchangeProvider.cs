using Sidekick.Data.ItemDefinitions;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    NinjaItemDefinition? GetDefinition(ItemDefinition item);
    Task<NinjaCurrency?> GetInfo(ItemDefinition item);
}
