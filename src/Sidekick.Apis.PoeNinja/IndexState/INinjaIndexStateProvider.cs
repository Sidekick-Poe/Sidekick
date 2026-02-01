using Sidekick.Apis.PoeNinja.IndexState.Models;
namespace Sidekick.Apis.PoeNinja.IndexState;

public interface INinjaIndexStateProvider
{
    Task<IndexStateLeague?> GetLeague();
}
