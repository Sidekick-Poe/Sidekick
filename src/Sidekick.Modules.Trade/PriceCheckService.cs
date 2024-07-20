using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe.Bulk;
using Sidekick.Apis.Poe.Bulk.Models;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Trade;

public class PriceCheckService(
    ISettingsService settingsService,
    IBulkTradeService bulkTradeService,
    IItemParser itemParser,
    ITradeFilterService tradeFilterService,
    ITradeSearchService tradeSearchService)
{
    public event Action? LoadingChanged;

    public event Action? FilterLoadingChanged;

    public Item? Item { get; set; }

    public TradeMode CurrentMode { get; set; }

    public PropertyFilters? PropertyFilters { get; set; }

    public List<ModifierFilter> ModifierFilters { get; set; } =
    [
    ];

    public List<PseudoModifierFilter> PseudoFilters { get; set; } =
    [
    ];

    public bool IsLoading { get; set; }

    public bool IsFilterLoading { get; set; }

    public TradeSearchResult<string>? ItemTradeResult { get; set; }

    public List<TradeItem>? TradeItems { get; set; }

    public BulkResponseModel? BulkTradeResult { get; set; }

    public bool ShowSearchButton => Item?.Metadata.Category != Category.Currency && Item?.Metadata.Category != Category.DivinationCard;

    public async Task Initialize(string itemText)
    {
        IsFilterLoading = true;
        FilterLoadingChanged?.Invoke();

        Item = await itemParser.ParseItemAsync(itemText.DecodeBase64Url() ?? string.Empty);

        PropertyFilters = tradeFilterService.GetPropertyFilters(Item);
        ModifierFilters = tradeFilterService
                          .GetModifierFilters(Item)
                          .ToList();
        PseudoFilters = tradeFilterService
                        .GetPseudoModifierFilters(Item)
                        .ToList();

        IsFilterLoading = false;
        FilterLoadingChanged?.Invoke();

        if (Item.Metadata.Rarity != Rarity.Rare && Item.Metadata.Rarity != Rarity.Magic)
        {
            await Search();
        }
    }

    public async Task Search()
    {
        CurrentMode = TradeMode.Item;
        if (bulkTradeService.SupportsBulkTrade(Item))
        {
            CurrentMode = await settingsService.GetEnum<TradeMode>(SettingKeys.PriceCheckCurrencyMode) ?? TradeMode.Item;
        }

        if (CurrentMode == TradeMode.Bulk)
        {
            await BulkSearch();
        }
        else
        {
            await ItemSearch();
        }
    }

    private async Task ItemSearch()
    {
        if (Item == null)
        {
            return;
        }

        TradeItems = new List<TradeItem>();
        IsLoading = true;
        LoadingChanged?.Invoke();

        var tradeCurrency = await settingsService.GetEnum<TradeCurrency>(SettingKeys.PriceCheckItemCurrency) ?? TradeCurrency.Chaos;
        ItemTradeResult = await tradeSearchService.Search(
            Item,
            tradeCurrency,
            PropertyFilters,
            ModifierFilters,
            PseudoFilters);

        IsLoading = false;
        await LoadMoreItems();

        LoadingChanged?.Invoke();
    }

    public async Task LoadMoreItems()
    {
        if (IsLoading || ItemTradeResult?.Result == null || ItemTradeResult?.Id == null)
        {
            return;
        }

        var ids = ItemTradeResult
                  .Result.Skip(TradeItems?.Count ?? 0)
                  .Take(10)
                  .ToList();
        if (ids.Count == 0)
        {
            return;
        }

        IsLoading = true;
        LoadingChanged?.Invoke();

        var result = await tradeSearchService.GetResults(ItemTradeResult.Id, ids, PseudoFilters);
        TradeItems?.AddRange(result);

        IsLoading = false;
        LoadingChanged?.Invoke();
    }

    private async Task BulkSearch()
    {
        if (Item == null)
        {
            return;
        }

        BulkTradeResult = null;
        IsLoading = true;
        LoadingChanged?.Invoke();

        var currency = await settingsService.GetEnum<TradeCurrency>(SettingKeys.PriceCheckBulkCurrency);
        var minStock = await settingsService.GetInt(SettingKeys.PriceCheckBulkMinimumStock);
        BulkTradeResult = await bulkTradeService.SearchBulk(Item, currency ?? TradeCurrency.Divine, minStock);

        IsLoading = false;
        LoadingChanged?.Invoke();
    }
}
