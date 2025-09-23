using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Items;

public interface IApiInvariantItemProvider : IInitializableService, IBaseItemProvider
{
    ItemApiInformation? UncutSkillGem { get; }
    ItemApiInformation? UncutSupportGem { get; }
    ItemApiInformation? UncutSpiritGem { get; }
}
