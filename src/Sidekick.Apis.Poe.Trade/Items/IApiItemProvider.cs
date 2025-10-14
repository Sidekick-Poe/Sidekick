using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Items;

public interface IApiItemProvider : IInitializableService
{
    Dictionary<string, List<ItemApiInformation>> NameAndTypeDictionary { get; }

    ItemApiInformation? GetApiItem(Rarity rarity, string? name, string? type);

    List<ItemApiInformation> UniqueItems { get; }
}
