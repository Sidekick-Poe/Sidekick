using Sidekick.Game.Ninja;
namespace Sidekick.Apis.PoeNinja.Exchange;

public interface INinjaExchangeProvider
{
    Task<NinjaCurrency?> GetInfo(NinjaExchangeItem? exchange);
}
