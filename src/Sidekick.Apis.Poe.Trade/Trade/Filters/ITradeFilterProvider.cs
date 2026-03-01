using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Raw;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters;

public interface ITradeFilterProvider : IInitializableService
{
    RawTradeFilter? TypeCategory { get; }
    RawTradeFilter? Desecrated { get; }
    RawTradeFilter? Veiled { get; }
    RawTradeFilter? Fractured { get; }
    RawTradeFilter? Mirrored { get; }
    RawTradeFilter? Foulborn { get; }
    RawTradeFilter? Sanctified { get; }
    RawTradeFilterCategory? EquipmentCategory { get; }
    RawTradeFilterCategory? WeaponCategory { get; }
    RawTradeFilterCategory? ArmourCategory { get; }
    RawTradeFilterCategory? SocketCategory { get; }
    RawTradeFilterCategory? RequirementsCategory { get; }
    RawTradeFilterCategory? MiscellaneousCategory { get; }
    RawTradeFilterCategory? EndgameCategory { get; }
    RawTradeFilterCategory? MapCategory { get; }
    RawTradeFilter? GetApiFilter(string categoryId, string filterId);
    Task<List<Types.TradeFilter>> GetFilters(Item item);
}
