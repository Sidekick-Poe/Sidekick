using Sidekick.Apis.Poe.Parser;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe
{
    public interface IItemMetadataProvider : IInitializableService
    {
        ItemMetadata Parse(ParsingItem parsingItem);
    }
}
