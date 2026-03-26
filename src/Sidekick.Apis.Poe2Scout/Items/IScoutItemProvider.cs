using Sidekick.Apis.Poe2Scout.Items.Models;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe2Scout.Items;

public interface IScoutItemProvider
{
    Task<ScoutItem?> GetItem(ItemDefinition item);
    Task<ScoutItem?> GetItem(string text);
}
