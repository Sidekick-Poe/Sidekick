using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Metadatas
{
    public interface IInvariantMetadataProvider : IInitializableService
    {
        Dictionary<string, ItemMetadata> IdDictionary { get; }
    }
}
