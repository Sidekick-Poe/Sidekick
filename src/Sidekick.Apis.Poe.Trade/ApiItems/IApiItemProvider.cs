using Sidekick.Common.Initialization;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.ApiItems;

public interface IApiItemProvider : IInitializableService
{
    List<ItemDefinition> Definitions { get; }

    List<ItemDefinition> UniqueItems { get; }

    ItemDefinition? GetById(string? id);

    ItemDefinition? Get(string? name, string? type);
}
