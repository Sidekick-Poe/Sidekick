using Sidekick.Apis.Poe.Trade.Trade.Items.Models;
using Sidekick.Common.Initialization;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.ApiItems;

public interface IApiItemProvider : IInitializableService
{
    ItemDefinition? ExaltedOrb { get; }

    ItemDefinition? ChaosOrb { get; }

    ItemDefinition? DivineOrb { get; }

    List<ItemDefinition> Definitions { get; }

    List<ItemDefinition> UniqueItems { get; }

    Dictionary<string, ItemDefinition> InvariantDictionary { get; }

    ItemDefinition? Get(ApiItem apiItem);
}
