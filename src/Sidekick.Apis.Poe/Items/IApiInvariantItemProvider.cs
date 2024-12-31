using Sidekick.Apis.Poe.Items.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Items;

public interface IApiInvariantItemProvider : IInitializableService
{
    Dictionary<string, ApiItem> IdDictionary { get; }

    List<string> UncutGemIds { get; }
}
