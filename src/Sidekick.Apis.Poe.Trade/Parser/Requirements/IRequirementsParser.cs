using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Parser.Requirements;

public interface IRequirementsParser
{
    void Parse(TextItem textItem);
}
