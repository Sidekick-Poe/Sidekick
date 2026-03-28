using Sidekick.Apis.Poe.Trade.Trade.Items.Models;
using Sidekick.Common.Initialization;
using Sidekick.Data.ItemDefinitions;
namespace Sidekick.Apis.Poe.Trade.ApiItems;

public interface IApiItemProvider : IInitializableService
{
    List<ItemDefinition> Definitions { get; }

    List<ItemDefinition> UniqueItems { get; }

    Dictionary<string, ItemDefinition> InvariantDictionary { get; }

    ItemDefinition? Get(ApiItem apiItem);

    ItemDefinition? Get(string? name);
}
