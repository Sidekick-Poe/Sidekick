using Sidekick.Common.Initialization;
using Sidekick.Data.Items;
using Sidekick.Data.Trade;
namespace Sidekick.Apis.Poe.Trade.Filters;

public interface ITradeFilterProvider : IInitializableService
{
    TradeFilter? TypeCategory { get; }
    TradeFilter? Desecrated { get; }
    TradeFilter? Veiled { get; }
    TradeFilter? Fractured { get; }
    TradeFilter? Mirrored { get; }
    TradeFilter? Foulborn { get; }
    TradeFilter? Sanctified { get; }
    TradeFilter? Imbued { get; }
    TradeFilterCategory? EquipmentCategory { get; }
    TradeFilterCategory? WeaponCategory { get; }
    TradeFilterCategory? ArmourCategory { get; }
    TradeFilterCategory? SocketCategory { get; }
    TradeFilterCategory? RequirementsCategory { get; }
    TradeFilterCategory? MiscellaneousCategory { get; }
    TradeFilterCategory? EndgameCategory { get; }
    TradeFilterCategory? MapCategory { get; }
    TradeFilterCategory? HeistCategory { get; }
    TradeFilter? GetApiFilter(string categoryId, string filterId);
    Task<List<Types.TradeFilter>> GetFilters(Item item);
}
