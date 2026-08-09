using Sidekick.Apis.Poe2Scout.Items.Models;
using Sidekick.Game.ItemDefinitions;
namespace Sidekick.Apis.Poe2Scout.Items;

public interface IScoutItemProvider
{
    Task<ScoutItem?> GetItem(TradeItemDefinition? tradeItem);
    Task<ScoutItem?> GetItem(string text);
}
