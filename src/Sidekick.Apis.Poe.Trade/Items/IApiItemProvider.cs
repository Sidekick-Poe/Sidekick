using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Items.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Items;

public interface IApiItemProvider : IInitializableService
{
    List<ApiItem> UniqueItems { get; }

    Dictionary<string, List<ApiItem>> NameAndTypeDictionary { get; }

    List<(Regex Regex, ApiItem Item)> NameAndTypeRegex { get; }
}
