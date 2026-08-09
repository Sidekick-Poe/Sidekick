using Sidekick.Common.Initialization;
using Sidekick.Game.ItemDefinitions;
using Sidekick.Game.Items;
namespace Sidekick.Apis.Poe.Trade.Parser.Definition;

public interface IItemDefinitionParser : IInitializableService
{
    List<ItemDefinition> UniqueItems { get; }

    void Parse(Item item);

    ItemDefinition? GetInvariant(string? name);
}
