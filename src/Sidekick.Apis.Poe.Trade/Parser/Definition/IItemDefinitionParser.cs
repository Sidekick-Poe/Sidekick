using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Parser.Definition;

public interface IItemDefinitionParser
{
    void Parse(Item item);
}
