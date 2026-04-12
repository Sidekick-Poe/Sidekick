using Sidekick.Data.ItemDefinitions;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    Task<NinjaCurrency?> GetInfo(ItemDefinition item);
}
