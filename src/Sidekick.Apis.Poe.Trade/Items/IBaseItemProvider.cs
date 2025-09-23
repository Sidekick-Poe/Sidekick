using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Items.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Items;

public interface IBaseItemProvider
{
    Dictionary<string, List<ItemApiInformation>> NameAndTypeDictionary { get; }

    List<(Regex Regex, ItemApiInformation Item)> NameAndTypeRegex { get; }

    ItemApiInformation? GetApiItem(Rarity rarity, ItemClass itemClass, string? name, string? type);
}
