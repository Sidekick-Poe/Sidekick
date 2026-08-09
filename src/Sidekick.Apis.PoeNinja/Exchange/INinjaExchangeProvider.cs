using Sidekick.Game.ItemDefinitions;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    Task<NinjaCurrency?> GetInfo(NinjaExchangeItem? exchange);
}
