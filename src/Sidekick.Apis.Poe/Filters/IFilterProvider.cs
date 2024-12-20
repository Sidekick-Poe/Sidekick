using Sidekick.Apis.Poe.Metadata.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Filters;

public interface IFilterProvider : IInitializableService
{
    List<ApiFilterOption> ApiItemCategories { get; }

    List<ApiFilterOption> PriceOptions { get; }

    string? GetPriceOption(string? price);
}
