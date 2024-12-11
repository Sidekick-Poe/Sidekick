using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Metadata
{
    public interface IInvariantMetadataProvider : IInitializableService
    {
        Dictionary<string, ItemMetadata> IdDictionary { get; }
    }
}
