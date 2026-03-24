using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Data.Items;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    Task<NinjaCurrency?> GetInfo(ItemDefinition item);
}
