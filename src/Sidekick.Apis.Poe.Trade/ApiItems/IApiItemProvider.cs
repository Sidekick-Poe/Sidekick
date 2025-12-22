using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Trade.ApiItems;

public interface IApiItemProvider : IInitializableService
{
    List<(Regex Regex, ItemApiInformation Item)> NamePatterns { get; }

    List<(Regex Regex, ItemApiInformation Item)> TypePatterns { get; }

    List<(Regex Regex, ItemApiInformation Item)> TextPatterns { get; }

    List<ItemApiInformation> UniqueItems { get; }
}
