using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade.Raw;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters;

public class TradeFilterProvider
(
    DataProvider dataProvider,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    IServiceProvider serviceProvider
) : ITradeFilterProvider
{
    public RawTradeFilter? TypeCategory => GetApiFilter("type_filters", "category");
    public RawTradeFilter? Desecrated => GetApiFilter("misc_filters", "desecrated");
    public RawTradeFilter? Veiled => GetApiFilter("misc_filters", "veiled");
    public RawTradeFilter? Fractured => GetApiFilter("misc_filters", "fractured_item");
    public RawTradeFilter? Mirrored => GetApiFilter("misc_filters", "mirrored");
    public RawTradeFilter? Foulborn => GetApiFilter("misc_filters", "foulborn_item");
    public RawTradeFilter? Sanctified => GetApiFilter("misc_filters", "sanctified");

    public RawTradeFilterCategory? WeaponCategory => GetApiFilterCategory("weapon_filters");
    public RawTradeFilterCategory? ArmourCategory => GetApiFilterCategory("armour_filters");
    public RawTradeFilterCategory? EquipmentCategory => GetApiFilterCategory("equipment_filters");
    public RawTradeFilterCategory? SocketCategory => GetApiFilterCategory("socket_filters");
    public RawTradeFilterCategory? RequirementsCategory => GetApiFilterCategory("req_filters");
    public RawTradeFilterCategory? MiscellaneousCategory => GetApiFilterCategory("misc_filters");
    public RawTradeFilterCategory? TradeCategory => GetApiFilterCategory("trade_filters");
    public RawTradeFilterCategory? EndgameCategory => GetApiFilterCategory("map_filters");
    public RawTradeFilterCategory? MapCategory => GetApiFilterCategory("map_filters");

    private List<RawTradeFilterCategory> Filters { get; set; } = [];

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var result = await dataProvider.Read<RawTradeResult<List<RawTradeFilterCategory>>>(game, DataType.TradeRawFilters, currentGameLanguage.Language);
        Filters = result.Result;
    }

    private RawTradeFilterCategory? GetApiFilterCategory(string categoryId)
    {
        return Filters.FirstOrDefault(x => x.Id == categoryId);
    }

    public RawTradeFilter? GetApiFilter(string categoryId, string filterId)
    {
        return GetApiFilterCategory(categoryId)?.Filters.FirstOrDefault(x => x.Id == filterId);
    }

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        if (TradeCategory?.Title == null) return [];

        var result = new List<TradeFilter>();

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
