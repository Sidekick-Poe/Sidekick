using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Models;
using TradeFilter = Sidekick.Data.Trade.Models.TradeFilter;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters;

public interface ITradeFilterProvider : IInitializableService
{
    TradeFilter? TypeCategory { get; }
    TradeFilter? Desecrated { get; }
    TradeFilter? Veiled { get; }
    TradeFilter? Fractured { get; }
    TradeFilter? Mirrored { get; }
    TradeFilter? Foulborn { get; }
    TradeFilter? Sanctified { get; }
    TradeFilterCategory? EquipmentCategory { get; }
    TradeFilterCategory? WeaponCategory { get; }
    TradeFilterCategory? ArmourCategory { get; }
    TradeFilterCategory? SocketCategory { get; }
    TradeFilterCategory? RequirementsCategory { get; }
    TradeFilterCategory? MiscellaneousCategory { get; }
    TradeFilterCategory? EndgameCategory { get; }
    TradeFilterCategory? MapCategory { get; }
    TradeFilter? GetApiFilter(string categoryId, string filterId);
    Task<List<Types.TradeFilter>> GetFilters(Item item);
}
