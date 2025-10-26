using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Items;

public interface IApiItemProvider : IInitializableService
{
    Dictionary<string, List<(Regex Regex, ItemApiInformation Item)>> NameDictionary { get; }

    List<(Regex Regex, ItemApiInformation Item)> TypePatterns { get; }

    List<(Regex Regex, ItemApiInformation Item)> TextPatterns { get; }

    List<ItemApiInformation> UniqueItems { get; }
}
