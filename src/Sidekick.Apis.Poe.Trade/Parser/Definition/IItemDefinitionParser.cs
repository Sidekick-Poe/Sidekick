using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Models;
using Sidekick.Common.Initialization;
using Sidekick.Data.ItemDefinitions;
namespace Sidekick.Apis.Poe.Trade.Parser.Definition;

public interface IItemDefinitionParser : IInitializableService
{
    List<ItemDefinition> Definitions { get; }

    List<ItemDefinition> UniqueItems { get; }

    Dictionary<string, ItemDefinition> InvariantDictionary { get; }

    void Parse(Item item);

    ItemDefinition? Get(ApiItem apiItem);

    ItemDefinition? Get(string? name);
}
