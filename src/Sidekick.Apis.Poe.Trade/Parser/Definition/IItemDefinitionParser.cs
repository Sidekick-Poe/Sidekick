using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Common.Initialization;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.Parser.Definition;

public interface IItemDefinitionParser : IInitializableService
{
    List<ItemDefinition> UniqueItems { get; }

    Dictionary<string, ItemDefinition> InvariantDictionary { get; }

    void Parse(Item item);

    ItemDefinition? Get(ApiItem apiItem);

    ItemDefinition? Get(string? name);
}
