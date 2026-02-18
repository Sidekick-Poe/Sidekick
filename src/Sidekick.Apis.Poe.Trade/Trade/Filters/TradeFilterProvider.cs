using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Settings;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models;
using TradeFilter = Sidekick.Data.Trade.Models.TradeFilter;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters;

public class TradeFilterProvider
(
    TradeDataProvider tradeDataProvider,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    IServiceProvider serviceProvider
) : ITradeFilterProvider
{
    public TradeFilter? TypeCategory => GetApiFilter("type_filters", "category");
    public TradeFilter? TradePrice => GetApiFilter("trade_filters", "price");
    public TradeFilter? TradeIndexed => GetApiFilter("trade_filters", "indexed");
    public TradeFilter? Desecrated => GetApiFilter("misc_filters", "desecrated");
    public TradeFilter? Veiled => GetApiFilter("misc_filters", "veiled");
    public TradeFilter? Fractured => GetApiFilter("misc_filters", "fractured_item");
    public TradeFilter? Mirrored => GetApiFilter("misc_filters", "mirrored");
    public TradeFilter? Foulborn => GetApiFilter("misc_filters", "foulborn_item");
    public TradeFilter? Sanctified => GetApiFilter("misc_filters", "sanctified");

    public TradeFilterCategory? WeaponCategory => GetApiFilterCategory("weapon_filters");
    public TradeFilterCategory? ArmourCategory => GetApiFilterCategory("armour_filters");
    public TradeFilterCategory? EquipmentCategory => GetApiFilterCategory("equipment_filters");
    public TradeFilterCategory? SocketCategory => GetApiFilterCategory("socket_filters");
    public TradeFilterCategory? RequirementsCategory => GetApiFilterCategory("req_filters");
    public TradeFilterCategory? MiscellaneousCategory => GetApiFilterCategory("misc_filters");
    public TradeFilterCategory? TradeCategory => GetApiFilterCategory("trade_filters");
    public TradeFilterCategory? EndgameCategory => GetApiFilterCategory("map_filters");
    public TradeFilterCategory? MapCategory => GetApiFilterCategory("map_filters");

    private List<TradeFilterCategory> Filters { get; set; } = [];

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        Filters = await tradeDataProvider.GetFilters(game, gameLanguageProvider.Language.Code);
    }

    public TradeFilterCategory? GetApiFilterCategory(string categoryId)
    {
        return Filters.FirstOrDefault(x => x.Id == categoryId);
    }

    public TradeFilter? GetApiFilter(string categoryId, string filterId)
    {
        return GetApiFilterCategory(categoryId)?.Filters.FirstOrDefault(x => x.Id == filterId);
    }

    public async Task<List<Types.TradeFilter>> GetFilters(Item item)
    {
        if (TradeCategory?.Title == null) return [];

        var result = new List<Types.TradeFilter>();

        var statusFilter = await serviceProvider.GetRequiredService<PlayerStatusFilterFactory>().GetFilter(item);
        if (statusFilter != null)
        {
            result.Add(statusFilter);
            await statusFilter.Initialize(item, settingsService);
        }

        var currencyFilter = await serviceProvider.GetRequiredService<CurrencyFilterFactory>().GetFilter(item);
        if (currencyFilter != null)
        {
            result.Add(currencyFilter);
            await currencyFilter.Initialize(item, settingsService);
        }

        if (result.Count == 0) return [];
        return [new ExpandableFilter(TradeCategory.Title, result.ToArray())];
    }
}
