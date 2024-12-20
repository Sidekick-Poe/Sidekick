using Sidekick.Apis.Poe.Metadata.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Filters;

public interface IFilterProvider : IInitializableService
{
    List<ApiFilterOption> ApiItemCategories { get; set; }
}
