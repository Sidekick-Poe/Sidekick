using Sidekick.Apis.Poe.Trade.Items.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Items;

public interface IApiInvariantItemProvider : IInitializableService
{
    Dictionary<string, ApiItem> IdDictionary { get; }

    List<string> UncutGemIds { get; }
}
