using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Metadata.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Metadata
{
    public interface IMetadataProvider : IInitializableService
    {
        Dictionary<string, List<ItemMetadata>> NameAndTypeDictionary { get; }

        List<(Regex Regex, ItemMetadata Item)> NameAndTypeRegex { get; }
    }
}
