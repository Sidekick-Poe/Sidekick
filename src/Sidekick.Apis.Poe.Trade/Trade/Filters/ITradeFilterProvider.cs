using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Models;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters;

public interface ITradeFilterProvider : IInitializableService
{
    ApiFilter? TypeCategory { get; }
    ApiFilter? Desecrated { get; }
    ApiFilter? Veiled { get; }
    ApiFilter? Fractured { get; }
    ApiFilter? Mirrored { get; }
    ApiFilter? Foulborn { get; }
    ApiFilter? Sanctified { get; }
    ApiFilterCategory? EquipmentCategory { get; }
    ApiFilterCategory? WeaponCategory { get; }
    ApiFilterCategory? ArmourCategory { get; }
    ApiFilterCategory? SocketCategory { get; }
    ApiFilterCategory? RequirementsCategory { get; }
    ApiFilterCategory? MiscellaneousCategory { get; }
    ApiFilterCategory? EndgameCategory { get; }
    ApiFilterCategory? MapCategory { get; }
    ApiFilter? GetApiFilter(string categoryId, string filterId);
    Task<List<TradeFilter>> GetFilters(Item item);
}
