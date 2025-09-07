using Sidekick.Apis.Poe2Scout.Items.Models;
namespace Sidekick.Apis.Poe2Scout.Items;

public interface IScoutItemProvider
{
    Task<List<ScoutItem>> GetUniqueItems();
    Task<List<ScoutItem>> GetCurrencyItems();
}
