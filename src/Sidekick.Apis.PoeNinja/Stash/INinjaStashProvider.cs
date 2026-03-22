using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Stash.Models;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Ninja;
namespace Sidekick.Apis.PoeNinja.Stash;

public interface INinjaStashProvider
{
    Task<NinjaStash?> GetInfo(NinjaStashItem item);
}
