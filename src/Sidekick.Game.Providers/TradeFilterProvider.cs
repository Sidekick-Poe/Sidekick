using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game;
using Sidekick.Game.TradeFilters;
using TradeFilter = Sidekick.Game.TradeFilters.TradeFilter;
namespace Sidekick.Apis.Poe.Trade.Filters;

public class TradeFilterProvider
(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    IServiceProvider serviceProvider
) : IInitializableService
{
    public TradeFilter? TypeCategory => GetApiFilter("type_filters", "category");
    public TradeFilter? Desecrated => GetApiFilter("misc_filters", "desecrated");
    public TradeFilter? Veiled => GetApiFilter("misc_filters", "veiled");
    public TradeFilter? Fractured => GetApiFilter("misc_filters", "fractured_item");
    public TradeFilter? Mirrored => GetApiFilter("misc_filters", "mirrored");
    public TradeFilter? Foulborn => GetApiFilter("misc_filters", "foulborn_item");
    public TradeFilter? Sanctified => GetApiFilter("misc_filters", "sanctified");
    public TradeFilter? Imbued => GetApiFilter("misc_filters", "gem_imbued");
    public TradeFilter? Damage => GetApiFilter("weapon_filters", "damage") ?? GetApiFilter("equipment_filters", "damage");

    public TradeFilterCategory? WeaponCategory => GetApiFilterCategory("weapon_filters");
    public TradeFilterCategory? ArmourCategory => GetApiFilterCategory("armour_filters");
    public TradeFilterCategory? EquipmentCategory => GetApiFilterCategory("equipment_filters");
    public TradeFilterCategory? SocketCategory => GetApiFilterCategory("socket_filters");
    public TradeFilterCategory? RequirementsCategory => GetApiFilterCategory("req_filters");
    public TradeFilterCategory? MiscellaneousCategory => GetApiFilterCategory("misc_filters");
    public TradeFilterCategory? TradeCategory => GetApiFilterCategory("trade_filters");
    public TradeFilterCategory? EndgameCategory => GetApiFilterCategory("map_filters");
    public TradeFilterCategory? MapCategory => GetApiFilterCategory("map_filters");
    public TradeFilterCategory? HeistCategory => GetApiFilterCategory("heist_filters");

    private List<TradeFilterCategory> Filters { get; set; } = [];

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Filters = await dataProvider.Read<List<TradeFilterCategory>>(game, GameDataType.TradeFilters, currentGameLanguage.Language);
    }

    public TradeFilterCategory? GetApiFilterCategory(string categoryId)
    {
        return Filters.FirstOrDefault(x => x.Id == categoryId);
    }

    public TradeFilter? GetApiFilter(string categoryId, string filterId)
    {
        return GetApiFilterCategory(categoryId)?.Filters.FirstOrDefault(x => x.Id == filterId);
    }
}
