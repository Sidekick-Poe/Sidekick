using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Filters;

namespace Sidekick.Apis.Poe.Trade.AutoSelect;

public interface IFilterAutoSelectService
{
    Task<bool> ShouldCheck(Item item, IAutoSelectableFilter filter);
}
