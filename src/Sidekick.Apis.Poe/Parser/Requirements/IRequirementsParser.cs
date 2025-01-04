using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Requirements;

public interface IRequirementsParser : IInitializableService
{
    void Parse(ParsingItem parsingItem);
}
