using Sidekick.Apis.Poe.Trade.Filters.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Filters;

public interface IInvariantFilterProvider : IInitializableService
{
    FilterDefinition? DesecratedDefinition { get; }
    FilterDefinition? VeiledDefinition { get; }
    FilterDefinition? FracturedDefinition { get; }
    FilterDefinition? MirroredDefinition { get; }
    FilterDefinition? FoulbornDefinition { get; }
    FilterDefinition? SanctifiedDefinition { get; }
}
