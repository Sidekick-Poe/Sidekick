using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Metadatas
{
    public interface IItemMetadataProvider : IInitializableService
    {
        Dictionary<string, List<ItemMetadata>> NameAndTypeDictionary { get; }

        List<(Regex Regex, ItemMetadata Item)> NameAndTypeRegex { get; }
    }
}
