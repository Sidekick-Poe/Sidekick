using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Metadata
{
    public interface IMetadataParser : IInitializableService
    {
        string GetLineWithoutSuperiorAffix(string line);

        ItemMetadata? Parse(string? name, string? type);

        ItemMetadata? Parse(ParsingItem parsingItem);
    }
}
